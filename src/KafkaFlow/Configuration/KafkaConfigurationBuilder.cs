namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;

    public class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;
        private readonly List<ClusterConfigurationBuilder> clusters = new List<ClusterConfigurationBuilder>();

        public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
        }

        //public KafkaConfiguration Build()
        //{
        //    var configuration = new KafkaConfiguration();
        //    return configuration;
        //}

        public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.dependencyConfigurator);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }
    }
}
