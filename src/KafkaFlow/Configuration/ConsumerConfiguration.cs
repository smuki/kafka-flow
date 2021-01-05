namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public class ConsumerConfiguration:ConsumerSetting
    {
        private readonly ConsumerConfig consumerConfig;
//        private int workerCount;

        public ConsumerConfiguration(
            ConsumerConfig consumerConfig,
            IEnumerable<string> topics,
            string consumerName,
            int workerCount,
            int bufferSize,
            Factory<IDistributionStrategy> distributionStrategyFactory,
            bool autoStoreOffsets,
            TimeSpan autoCommitInterval,
            IReadOnlyList<Action<string>> statisticsHandlers)
        {
            this.consumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));

            if (string.IsNullOrEmpty(this.consumerConfig.GroupId))
            {
                throw new ArgumentNullException(nameof(consumerConfig.GroupId));
            }

            base.DistributionStrategyFactory =
                distributionStrategyFactory ?? throw new ArgumentNullException(nameof(distributionStrategyFactory));
            this.AutoStoreOffsets = autoStoreOffsets;
            this.AutoCommitInterval = autoCommitInterval;
            this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
            this.ConsumerName = consumerName ?? Guid.NewGuid().ToString();
            this.WorkerCount = workerCount;
            this.StatisticsHandlers = statisticsHandlers;
            this.GroupId = this.consumerConfig.GroupId;

            this.BufferSize = bufferSize > 0 ?
                bufferSize :
                throw new ArgumentOutOfRangeException(
                    nameof(bufferSize),
                    bufferSize,
                    "The value must be greater than 0");
        }

        public IEnumerable<string> Topics { get; }

        public IReadOnlyList<Action<string>> StatisticsHandlers { get; }

        public ConsumerConfig GetKafkaConfig()
        {
            return this.consumerConfig;
        }
    }
}
