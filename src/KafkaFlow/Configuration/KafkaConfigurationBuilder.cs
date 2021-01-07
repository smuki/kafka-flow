namespace KafkaFlow.Configuration
{
    using KafkaFlow.Consumers;
    using KafkaFlow.Dependency;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;
        private readonly List<ClusterConfigurationBuilder> clusters = new List<ClusterConfigurationBuilder>();

        public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
        }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration();
            return configuration;
        }

        public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.dependencyConfigurator);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }
    }
}
