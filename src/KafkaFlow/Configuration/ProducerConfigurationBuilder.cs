namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;

    public class ProducerConfigurationBuilder : IProducerConfigurationBuilder
    {
        private readonly string name;
        private readonly List<Action<string>> statisticsHandlers = new List<Action<string>>();

        private string topic;
        private ProducerConfig producerConfig;
        private Acks? acks;
        private int statisticsInterval;

        public ProducerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator, string name)
        {
            this.name = name;
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public IProducerConfigurationBuilder DefaultTopic(string topic)
        {
            this.topic = topic;
            return this;
        }

        public IProducerConfigurationBuilder WithProducerConfig(ProducerConfig config)
        {
            this.producerConfig = config;
            return this;
        }

        public IProducerConfigurationBuilder WithAcks(Acks acks)
        {
            this.acks = acks;
            return this;
        }

        public IProducerConfigurationBuilder WithStatisticsHandler(Action<string> statisticsHandler)
        {
            this.statisticsHandlers.Add(statisticsHandler);
            return this;
        }

        public IProducerConfigurationBuilder WithStatisticsIntervalMs(int statisticsIntervalMs)
        {
            this.statisticsInterval = statisticsIntervalMs;
            return this;
        }
    }
}
