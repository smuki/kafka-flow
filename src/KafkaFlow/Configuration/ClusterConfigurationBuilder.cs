namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ClusterConfigurationBuilder : IClusterConfigurationBuilder
    {
        //private readonly List<ProducerConfigurationBuilder> producers = new List<ProducerConfigurationBuilder>();

        private IEnumerable<string> brokers;
        private Func<SecurityInformation> securityInformationHandler;

        public ClusterConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public ClusterConfiguration Build()
        {
            var configuration = new ClusterConfiguration(
                this.securityInformationHandler);

            return configuration;
        }
    }
}
