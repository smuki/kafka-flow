namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public class ConsumerConfiguration
    {
        private readonly ConsumerConfig consumerConfig;
        private int workerCount;

        public ConsumerConfiguration(
            ConsumerConfig consumerConfig,
            IEnumerable<string> topics,
            string consumerName,
            int workerCount,
            int bufferSize,
            Factory<IDistributionStrategy> distributionStrategyFactory,
            MiddlewareConfiguration middlewareConfiguration,
            bool autoStoreOffsets,
            TimeSpan autoCommitInterval,
            IReadOnlyList<Action<string>> statisticsHandlers)
        {
            this.consumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));

            if (string.IsNullOrEmpty(this.consumerConfig.GroupId))
            {
                throw new ArgumentNullException(nameof(consumerConfig.GroupId));
            }

            this.DistributionStrategyFactory =
                distributionStrategyFactory ?? throw new ArgumentNullException(nameof(distributionStrategyFactory));
            this.MiddlewareConfiguration = middlewareConfiguration ?? throw new ArgumentNullException(nameof(middlewareConfiguration));
            this.AutoStoreOffsets = autoStoreOffsets;
            this.AutoCommitInterval = autoCommitInterval;
            this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
            this.ConsumerName = consumerName ?? Guid.NewGuid().ToString();
            this.WorkerCount = workerCount;
            this.StatisticsHandlers = statisticsHandlers;

            this.BufferSize = bufferSize > 0 ?
                bufferSize :
                throw new ArgumentOutOfRangeException(
                    nameof(bufferSize),
                    bufferSize,
                    "The value must be greater than 0");
        }

        public Factory<IDistributionStrategy> DistributionStrategyFactory { get; }

        public MiddlewareConfiguration MiddlewareConfiguration { get; }

        public IEnumerable<string> Topics { get; }

        public string ConsumerName { get; }

        public int WorkerCount
        {
            get => this.workerCount;
            set =>
                this.workerCount = value > 0 ?
                    value :
                    throw new ArgumentOutOfRangeException(
                        nameof(this.WorkerCount),
                        this.WorkerCount,
                        $"The {nameof(this.WorkerCount)} value must be greater than 0");
        }

        public string GroupId => this.consumerConfig.GroupId;

        public int BufferSize { get; }

        public bool AutoStoreOffsets { get; }

        public TimeSpan AutoCommitInterval { get; }

        public IReadOnlyList<Action<string>> StatisticsHandlers { get; }

        public ConsumerConfig GetKafkaConfig()
        {
            return this.consumerConfig;
        }
    }
}
