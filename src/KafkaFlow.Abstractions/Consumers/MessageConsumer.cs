namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    public class MessageConsumer : IMessageConsumer
    {
        private readonly IConsumerWorkerPool workerPool;
        private readonly ConsumerSetting configuration;
        private readonly ILogHandler logHandler;
        private readonly IConsumerClient consumerClient;

        public MessageConsumer(
                        IConsumerClient consumerClient,
                        IConsumerWorkerPool workerPool,
                        ConsumerSetting configuration,
                        ILogHandler logHandler)
        { 
            this.workerPool = workerPool;
            this.configuration = configuration;
            this.logHandler = logHandler;
            this.consumerClient = consumerClient;
        }

        public string ConsumerName
        {
            get
            {
                return this.configuration.ConsumerName;
            }
        }
        public string GroupId
        {
            get
            {
                return this.configuration.GroupId;
            }
        }
        public int WorkerCount
        {
            get
            {
                return this.configuration.WorkerCount;
            }
        }

        public async Task OverrideOffsetsAndRestartAsync(IReadOnlyCollection<XXXTopicPartitionOffset> offsets)
        {
            if (offsets is null)
            {
                throw new ArgumentNullException(nameof(offsets));
            }

            try
            {
                this.consumerClient.Pause(this.consumerClient.Assignment);
                await this.workerPool.StopAsync().ConfigureAwait(false);

                this.consumerClient.Commit(offsets);

                await this.InternalRestart().ConfigureAwait(false);

                this.logHandler.Info("Kafka offsets overridden", GetOffsetsLogData(offsets));
            }
            catch (Exception e)
            {
                this.logHandler.Error(
                    "Error overriding offsets",
                    e,
                    GetOffsetsLogData(offsets));
                throw;
            }
        }

        private static object GetOffsetsLogData(IEnumerable<XXXTopicPartitionOffset> offsets) => offsets
            .GroupBy(x => x.Topic)
            .Select(
                x => new
                {
                    x.First().Topic,
                    Partitions = x.Select(
                        y => new
                        {
                            Partition = y.Partition,
                            Offset = y.Offset
                        })
                });

        public async Task ChangeWorkerCountAndRestartAsync(int workerCount)
        {
            this.configuration.WorkerCount = workerCount;
            await this.InternalRestart().ConfigureAwait(false);

            this.logHandler.Info("KafkaFlow consumer workers changed", new { workerCount });
        }

        public async Task RestartAsync()
        {
            await this.InternalRestart().ConfigureAwait(false);
            this.logHandler.Info("KafkaFlow consumer manually restarted", null);
        }

        private async Task InternalRestart()
        {
            await this.consumerClient.StopAsync().ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
            await this.consumerClient.StartAsync().ConfigureAwait(false);
        }

        public IReadOnlyList<string> Subscription => this.consumerClient.Subscription;

        public IReadOnlyList<XXXTopicPartition> Assignment => this.consumerClient.Assignment;

        public string MemberId => this.consumerClient.MemberId;

        public string ClientInstanceName => this.consumerClient.Name;

        public void Pause(IEnumerable<XXXTopicPartition> topicPartitions) =>
            this.consumerClient.Pause(topicPartitions);

        public void Resume(IEnumerable<XXXTopicPartition> topicPartitions) =>
            this.consumerClient.Resume(topicPartitions);

        public long GetPosition(XXXTopicPartition topicPartition) =>
            this.consumerClient.Position(topicPartition);

        public IOffsetsWatermark GetWatermarkOffsets(XXXTopicPartition topicPartition) =>
            this.consumerClient.GetWatermarkOffsets(topicPartition);

        public IOffsetsWatermark QueryWatermarkOffsets(XXXTopicPartition topicPartition, TimeSpan timeout) =>
            this.consumerClient.QueryWatermarkOffsets(topicPartition, timeout);

        public List<XXXTopicPartitionOffset> OffsetsForTimes(IEnumerable<XXXTopicPartitionTimestamp> topicPartitions,TimeSpan timeout) =>
            this.consumerClient.OffsetsForTimes(topicPartitions, timeout);
    }
}
