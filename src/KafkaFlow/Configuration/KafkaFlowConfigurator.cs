namespace KafkaFlow.Configuration
{
    using KafkaFlow.Dependency;
    using System;

    /// <summary>
    /// A class to configure KafkaFlow
    /// </summary>
    public class KafkaFlowConfigurator
    {
        /// <summary>
        /// Creates a <see cref="KafkaFlowConfigurator"/> instance
        /// </summary>
        /// <param name="dependencyConfigurator">Dependency injection configurator</param>
        /// <param name="kafka">A handler to setup the configuration</param>
        public KafkaFlowConfigurator(
            IDependencyConfigurator dependencyConfigurator,
            Action<IKafkaConfigurationBuilder> kafka)
        {
            //dependencyConfigurator.AddSingleton<KafkaConfiguration>();
        }
    }
}
