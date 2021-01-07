namespace KafkaFlow.Configuration
{
    using System;

    /// <summary>
    /// Used to build the producer configuration
    /// </summary>
    public interface IProducerConfigurationBuilder
    {
        /// <summary>
        /// Gets the dependency injection configurator
        /// </summary>
        IDependencyConfigurator DependencyConfigurator { get; }

    }
}
