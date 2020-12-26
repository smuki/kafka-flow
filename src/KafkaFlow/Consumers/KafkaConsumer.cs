namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;

    public class KafkaConsumer: IConsumerClient
    {
        private ConsumerSetting configuration;
        private readonly IConsumerManager consumerManager;
        private readonly ILogHandler logHandler;
        private IConsumerWorkerPool workerPool;
        private readonly CancellationToken busStopCancellationToken;

        private ConsumerBuilder<byte[], byte[]> consumerBuilder;

        private CancellationTokenSource stopCancellationTokenSource;
        private Task backgroundTask;
        private IConsumer<byte[], byte[]> consumer;
        public KafkaConsumer(
            IConsumerManager consumerManager,
            ILogHandler logHandler,
            CancellationToken busStopCancellationToken)
        {
            this.consumerManager = consumerManager;
            this.logHandler = logHandler;
            this.busStopCancellationToken = busStopCancellationToken;


        }
        public List<XXXTopicPartition> Assignment { get { return Util.TopicPartition(this.consumer.Assignment).ToList(); } }
        public string Name { get { return this.consumer.Name; } }
        public string ConsumerName {
            get {
                if (this.Parameter==null)
                {
                    return this.GetType().FullName;
                }
                else
                {
                    return this.Parameter.ConsumerName;
                }
            } 
        }
        public ConsumerSetting Parameter { get { return null; } }
        public string MemberId { get { return this.consumer.MemberId; } }
        public IReadOnlyList<string> Subscription { get { return this.consumer.Subscription; } }
        public void Initialize(IConsumerWorkerPool consumerWorkerPool, ConsumerSetting eventConsumer)
        {
            this.workerPool = consumerWorkerPool;
            this.configuration = eventConsumer;
            //var kafkaConfig = configuration.GetKafkaConfig();

            this.consumerBuilder = new ConsumerBuilder<byte[], byte[]>(kafkaConfig);

            this.consumerBuilder
                .SetPartitionsAssignedHandler((consumer, partitions) => this.OnPartitionAssigned(consumer, partitions))
                .SetPartitionsRevokedHandler((consumer, partitions) => this.OnPartitionRevoked(partitions))
                .SetErrorHandler((p, error) =>
                {
                    if (error.IsFatal)
                    {
                        this.logHandler.Error("Kafka Consumer Fatal Error", null, new { Error = error });
                    }
                    else
                    {
                        this.logHandler.Warning("Kafka Consumer Error", new { Error = error });
                    }
                })
                .SetStatisticsHandler((consumer, statistics) =>
                {
                   // foreach (var handler in configuration.StatisticsHandlers)
                   // {
                   //     handler.Invoke(statistics);
                   // }
                });
        }
        public long Position(XXXTopicPartition offsets)
        {
            Console.WriteLine("Position...");
           return this.consumer.Position(Util.TopicPartition(offsets)).Value;
        }
        public IOffsetsWatermark GetWatermarkOffsets(XXXTopicPartition offsets)
        {
            Console.WriteLine("GetWatermarkOffsets...");
            var wm = this.consumer.GetWatermarkOffsets(Util.TopicPartition(offsets));
            return new OffsetsWatermark(wm.High,wm.Low);
        }
        public IOffsetsWatermark QueryWatermarkOffsets(XXXTopicPartition offsets, TimeSpan timeout)
        {
            var wm = this.consumer.QueryWatermarkOffsets(Util.TopicPartition(offsets), timeout);
            Console.WriteLine("GetWatermarkOffsets...");
            return new OffsetsWatermark(wm.High, wm.Low);
        }
        public List<XXXTopicPartitionOffset> OffsetsForTimes(IEnumerable<XXXTopicPartitionTimestamp> timestampsToSearch, TimeSpan timeout)
        {
            var tps = this.consumer.OffsetsForTimes(Util.TopicPartitionTimestamp(timestampsToSearch), timeout);
            return Util.TopicPartitionOffset(tps).ToList();
            Console.WriteLine("OffsetsForTimes...");
        }
        
        public void Commit(IEnumerable<XXXTopicPartitionOffset> offsets)
        {
            Console.WriteLine("Commit...");
            this.consumer.Commit(Util.TopicPartitionOffset(offsets));
        }
        public void Pause(IEnumerable<XXXTopicPartition> offsets)
        {
            Console.WriteLine("Pause...");
            this.consumer.Pause(Util.TopicPartition(offsets));
        }
        public void Resume(IEnumerable<XXXTopicPartition> offsets)
        {
            Console.WriteLine("Resume...");
            this.consumer.Resume(Util.TopicPartition(offsets));
        }
      
        public void Dispose()
        {
            //_consumerClient?.Dispose();
        }
        private void OnPartitionRevoked(IReadOnlyCollection<TopicPartitionOffset> topicPartitions)
        {
            this.logHandler.Warning("Partitions revoked", this.GetConsumerLogInfo(topicPartitions.Select(x => x.TopicPartition)));

            this.workerPool.StopAsync().GetAwaiter().GetResult();
        }

        private void OnPartitionAssigned(IConsumer<byte[], byte[]> consumer, IReadOnlyCollection<TopicPartition> partitions)
        {
            this.logHandler.Info("Partitions assigned", this.GetConsumerLogInfo(partitions));

            this.workerPool.StartAsync(this, Util.TopicPartition(partitions), this.stopCancellationTokenSource.Token).GetAwaiter().GetResult();
        }

        private object GetConsumerLogInfo(IEnumerable<TopicPartition> partitions) => new
        {
            this.configuration.GroupId,
            this.configuration.ConsumerName,
            Topics = partitions.GroupBy(x => x.Topic).Select(
                    x => new
                    {
                        x.First().Topic,
                        PartitionsCount = x.Count(),
                        Partitions = x.Select(y => y.Partition.Value)
                    })
        };

        public Task StartAsync()
        {
            this.stopCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.busStopCancellationToken);

            this.CreateBackgroundTask();

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            await this.workerPool.StopAsync().ConfigureAwait(false);

            if (this.stopCancellationTokenSource.Token.CanBeCanceled)
            {
                this.stopCancellationTokenSource.Cancel();
            }

            await this.backgroundTask.ConfigureAwait(false);
            this.backgroundTask.Dispose();
        }

        private void CreateBackgroundTask()
        {
            consumer = this.consumerBuilder.Build();
            this.consumerManager.AddOrUpdate(new MessageConsumer(this, this.workerPool, this.configuration, this.logHandler));

            consumer.Subscribe(this.configuration.Topics);

            this.backgroundTask = Task.Factory.StartNew(
                async () =>
                {
                    using (consumer)
                    {
                        while (!this.stopCancellationTokenSource.Token.IsCancellationRequested)
                        {
                            try
                            {
                                var message = consumer.Consume(this.stopCancellationTokenSource.Token);

                                var headers = new MessageHeaders();
                                foreach (var header in message.Message.Headers)
                                {
                                    headers.Add(header.Key, header.GetValueBytes());
                                }

                                var intermediateMessage = new IntermediateMessage(headers, message.Message.Value);
                                intermediateMessage.Topic = message.Topic;
                                intermediateMessage.Partition = message.Partition;
                                intermediateMessage.Offset = message.Offset;

                                await this.workerPool.EnqueueAsync(intermediateMessage, this.stopCancellationTokenSource.Token).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                // Ignores the exception
                            }
                            catch (KafkaException ex) when (ex.Error.IsFatal)
                            {
                                this.logHandler.Error("Kafka fatal error occurred. Trying to restart in 5 seconds", ex, null);

                                await this.workerPool.StopAsync().ConfigureAwait(false);
                                _ = Task
                                    .Delay(5000, this.stopCancellationTokenSource.Token)
                                    .ContinueWith(t => this.CreateBackgroundTask());

                                break;
                            }
                            catch (Exception ex)
                            {
                                this.logHandler.Warning("Error consuming message from Kafka", ex);
                            }
                        }

                        consumer.Close();
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }
    }
}
