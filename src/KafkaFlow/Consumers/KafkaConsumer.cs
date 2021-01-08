namespace MessagePipeline.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Schema;
    using Confluent.Kafka;
    using MessagePipeline.Configuration;
    using Volte.Data.VolteDi;
    using Volte.Utils;

    [Injection(InjectionType = InjectionType.Both)]
    public class KafkaConsumer: IConsumerClient
    {

        private MessageConsumerSettting configuration;
        private readonly IConsumerManager consumerManager;
        private IConsumerWorkerPool workerPool;
        private CancellationToken busStopCancellationToken;

        private ConsumerBuilder<byte[], byte[]> consumerBuilder;

        private CancellationTokenSource stopCancellationTokenSource;
        private Task backgroundTask;
        private IConsumer<byte[], byte[]> consumer;
        public KafkaConsumer(
            IConsumerManager consumerManager)
        {
            this.consumerManager = consumerManager;
        }
        public List<XXXTopicPartition> Assignment { get { return XXXUtil.TopicPartition(this.consumer.Assignment).ToList(); } }
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
        public string this[string name]
        {
            get
            {
                return _Parameter.TryGetValue(name, out var o) ? o : null;
            }
        }
        private readonly Dictionary<string, string> _Parameter = new Dictionary<string, string>();
        public MessageConsumerSettting Parameter { get { return this.configuration; } }
        public string MemberId { get { return this.consumer.MemberId; } }
        public IReadOnlyList<string> Subscription { get { return this.consumer.Subscription; } }
        public void Initialize(IConsumerWorkerPool consumerWorkerPool, MessageConsumerSettting eventConsumer, CancellationToken busStopCancellationToken)
        {

            this.workerPool = consumerWorkerPool;
            this.configuration = eventConsumer;
            this.busStopCancellationToken = busStopCancellationToken;

            ConsumerConfig consumerConfig = new ConsumerConfig();
            consumerConfig.BootstrapServers ??= eventConsumer.Brokers;
            consumerConfig.GroupId ??= "print-console-handler";

            if (eventConsumer["AutoOffsetReset"] == "Earliest")
            {
                consumerConfig.AutoOffsetReset ??= AutoOffsetReset.Earliest;
            }
            else if (eventConsumer["AutoOffsetReset"] == "Error")
            {
                consumerConfig.AutoOffsetReset ??= AutoOffsetReset.Error;
            }
            else
            {
                consumerConfig.AutoOffsetReset ??= AutoOffsetReset.Latest;
            }

            if (eventConsumer.ContainsKey("MaxPollIntervalMs"))
            {
                consumerConfig.MaxPollIntervalMs ??= Util.ToInt(eventConsumer["MaxPollIntervalMs"]);
            }
            else
            {
                consumerConfig.MaxPollIntervalMs ??= 10000;
            }

            if (eventConsumer.ContainsKey("StatisticsIntervalMs"))
            {
                consumerConfig.StatisticsIntervalMs ??= Util.ToInt(eventConsumer["StatisticsIntervalMs"]);
            }
            else
            {
                consumerConfig.StatisticsIntervalMs ??= 10000;
            }

            consumerConfig.EnableAutoOffsetStore = false;
            consumerConfig.EnableAutoCommit = false;

            this.consumerBuilder = new ConsumerBuilder<byte[], byte[]>(consumerConfig);

            this.consumerBuilder
                .SetPartitionsAssignedHandler((consumer, partitions) => this.OnPartitionAssigned(consumer, partitions))
                .SetPartitionsRevokedHandler((consumer, partitions) => this.OnPartitionRevoked(partitions))
                .SetErrorHandler((p, error) =>
                {
                    if (error.IsFatal)
                    {
                        NLogger.Error($"Kafka Consumer Fatal Error {error}");
                    }
                    else
                    {
                        NLogger.Warn($"Kafka Consumer Error {error}");
                    }
                })
                .SetStatisticsHandler((consumer, statistics) =>
                {
                    //foreach (var handler in configuration.StatisticsHandlers)
                    //{
                    //    handler.Invoke(statistics);
                    //}
                });
        }
        public long Position(XXXTopicPartition offsets)
        {
           return this.consumer.Position(XXXUtil.TopicPartition(offsets)).Value;
        }
        public IOffsetsWatermark GetWatermarkOffsets(XXXTopicPartition offsets)
        {
            var wm = this.consumer.GetWatermarkOffsets(XXXUtil.TopicPartition(offsets));
            return new OffsetsWatermark(wm.High,wm.Low);
        }
        public IOffsetsWatermark QueryWatermarkOffsets(XXXTopicPartition offsets, TimeSpan timeout)
        {
            var wm = this.consumer.QueryWatermarkOffsets(XXXUtil.TopicPartition(offsets), timeout);
            return new OffsetsWatermark(wm.High, wm.Low);
        }
        public List<XXXTopicPartitionOffset> OffsetsForTimes(IEnumerable<XXXTopicPartitionTimestamp> timestampsToSearch, TimeSpan timeout)
        {
            var tps = this.consumer.OffsetsForTimes(XXXUtil.TopicPartitionTimestamp(timestampsToSearch), timeout);
            return XXXUtil.TopicPartitionOffset(tps).ToList();
        }
        
        public void Commit(IEnumerable<XXXTopicPartitionOffset> offsets)
        {
            this.consumer.Commit(XXXUtil.TopicPartitionOffset(offsets));
        }
        public void Pause(IEnumerable<XXXTopicPartition> offsets)
        {
            this.consumer.Pause(XXXUtil.TopicPartition(offsets));
        }
        public void Resume(IEnumerable<XXXTopicPartition> offsets)
        {
            this.consumer.Resume(XXXUtil.TopicPartition(offsets));
        }
      
        public void Dispose()
        {
            //_consumerClient?.Dispose();
        }
        private void OnPartitionRevoked(IReadOnlyCollection<TopicPartitionOffset> topicPartitions)
        {
            NLogger.Info($"Partition Revoked {JsonSerializer.Serialize(this.GetConsumerLogInfo(topicPartitions.Select(x => x.TopicPartition)))}");
            this.workerPool.StopAsync().GetAwaiter().GetResult();
        }

        private void OnPartitionAssigned(IConsumer<byte[], byte[]> consumer, IReadOnlyCollection<TopicPartition> partitions)
        {
            int nPartition = partitions.Count;
            NLogger.Info($"Partition : {nPartition}");
            NLogger.Info($"Partition Assigned { JsonSerializer.Serialize(this.GetConsumerLogInfo(partitions))}");
            this.workerPool.StartAsync(this, XXXUtil.TopicPartition(partitions), this.stopCancellationTokenSource.Token).GetAwaiter().GetResult();
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
            this.consumerManager.AddOrUpdate(new MessageConsumer(this, this.workerPool, this.configuration));
            NLogger.Info("Topic : " + this.configuration.Topic);
            List<string> xx = new List<string>();
            xx.Add(configuration.Topic);
            consumer.Subscribe(xx.AsEnumerable<string>());

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
                                NLogger.Info("Kafka fatal error occurred.Trying to restart in 5 seconds", ex);

                                NLogger.Error("Kafka fatal error occurred. Trying to restart in 5 seconds", ex, null);

                                await this.workerPool.StopAsync().ConfigureAwait(false);
                                _ = Task
                                    .Delay(5000, this.stopCancellationTokenSource.Token)
                                    .ContinueWith(t => this.CreateBackgroundTask());

                                break;
                            }
                            catch (Exception ex)
                            {
                                NLogger.Warn("Error consuming message from Kafka", ex);
                                NLogger.Info("Error consuming message from Kafka", ex);
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
