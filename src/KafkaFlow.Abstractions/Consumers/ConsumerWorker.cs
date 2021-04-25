namespace MessagePipeline.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using MessagePipeline.Configuration;
    using MessagePipeline.Middleware;
    using Volte.Utils;

    internal class ConsumerWorker : IConsumerWorker
    {
        private readonly MessageConsumerSettting configuration;
        private readonly IOffsetManager offsetManager;
        private readonly IMiddlewareExecutor middlewareExecutor;

        private CancellationTokenSource stopCancellationTokenSource;

        private readonly Channel<IntermediateMessage> messagesBuffer;
        private Task backgroundTask;
        private Action onMessageFinishedHandler;
        private IConsumerClient consumerClient;

        public ConsumerWorker(
            IConsumerClient consumerClient,
            int workerId,
            MessageConsumerSettting configuration,
            IOffsetManager offsetManager,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.Id = workerId;
            this.consumerClient = consumerClient;
            this.configuration = configuration;
            this.offsetManager = offsetManager;
            this.middlewareExecutor = middlewareExecutor;
            this.messagesBuffer = Channel.CreateBounded<IntermediateMessage>(configuration.BufferSize);
        }

        public int Id { get; }

        public ValueTask EnqueueAsync(
            IntermediateMessage message,
            CancellationToken stopCancellationToken = default)
        {
            return this.messagesBuffer.Writer.WriteAsync(message, stopCancellationToken);
        }

        public Task StartAsync(CancellationToken stopCancellationToken)
        {
            this.stopCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stopCancellationToken);

            this.backgroundTask = Task.Factory.StartNew(
                async () =>
                {
                    while (!this.stopCancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await this.messagesBuffer.Reader.ReadAsync(this.stopCancellationTokenSource.Token).ConfigureAwait(false);
                            
                            var context = new ConsumerMessageContext(
                                new MessageContextConsumer(this.consumerClient, this.offsetManager, message, this.stopCancellationTokenSource.Token),
                                message,
                                this.Id,
                                this.configuration.GroupId);

                            try
                            {
                                await this.middlewareExecutor.Execute(context, con => Task.CompletedTask).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                NLogger.Error("Error executing consumer", ex, context);
                            }
                            finally
                            {
                                if (this.configuration.AutoStoreOffsets && context.Consumer.ShouldStoreOffset)
                                {
                                    this.offsetManager.StoreOffset(message.TopicPartitionOffset);
                                }

                                this.onMessageFinishedHandler?.Invoke();
                            }
                        }
                        catch (OperationCanceledException ex)
                        {
                            NLogger.Warn(ex);
                            // Ignores the exception
                        }
                        catch (Exception ex)
                        {
                            NLogger.Error(ex);
                            //**TODO** Ignores the exception
                        }
                    }
                    NLogger.Info("Stop-ConsumerWorker");
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public Task StartAsync() => this.StartAsync(default);

        public async Task StopAsync()
        {
            if (this.stopCancellationTokenSource != null && !this.stopCancellationTokenSource.IsCancellationRequested)
            {
                this.stopCancellationTokenSource.Cancel();
                this.stopCancellationTokenSource.Dispose();
            }
            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }

        public void OnTaskCompleted(Action handler)
        {
            this.onMessageFinishedHandler = handler;
        }
    }
}
