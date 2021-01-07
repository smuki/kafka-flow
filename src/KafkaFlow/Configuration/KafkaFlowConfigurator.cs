namespace KafkaFlow.Configuration
{
    using System;
    using System.Linq;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Dependency;

    /// <summary>
    /// A class to configure KafkaFlow
    /// </summary>
    public class KafkaFlowConfigurator
    {
        private readonly KafkaConfiguration configuration;

        /// <summary>
        /// Creates a <see cref="KafkaFlowConfigurator"/> instance
        /// </summary>
        /// <param name="dependencyConfigurator">Dependency injection configurator</param>
        /// <param name="kafka">A handler to setup the configuration</param>
        public KafkaFlowConfigurator(
            IDependencyConfigurator dependencyConfigurator,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            var builder = new KafkaConfigurationBuilder(dependencyConfigurator);

            kafka(builder);

            this.configuration = builder.Build();

            dependencyConfigurator.AddSingleton<KafkaConfiguration>();
        }
    }
}
