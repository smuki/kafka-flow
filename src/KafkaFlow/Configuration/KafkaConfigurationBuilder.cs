namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;

    public class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;
        private readonly List<ClusterConfigurationBuilder> clusters = new List<ClusterConfigurationBuilder>();
        private Type logHandler = typeof(NullLogHandler);

        public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
        }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration();

            configuration.AddClusters(this.clusters.Select(x => x.Build(configuration)));

            this.dependencyConfigurator.AddSingleton<IProducerAccessor>(
                resolver => new ProducerAccessor(
                    configuration.Clusters
                        .SelectMany(x => x.Producers)
                        .Select(
                            producer => new MessageProducer(
                                resolver,
                                producer))));

            var consumerManager = new ConsumerManager();

            this.dependencyConfigurator
                .AddTransient(typeof(ILogHandler), this.logHandler)
                .AddSingleton<IConsumerAccessor>(consumerManager)
                .AddSingleton<IConsumerManager>(consumerManager);

            return configuration;
        }

        public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.dependencyConfigurator);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }

        public IKafkaConfigurationBuilder UseLogHandler<TLogHandler>() where TLogHandler : ILogHandler
        {
            this.logHandler = typeof(TLogHandler);
            return this;
        }
    }
}
