namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// </summary>
    public interface IClusterConfigurationBuilder
    {
        /// <summary>
        /// Gets the dependency injection configurator
        /// </summary>
        IDependencyConfigurator DependencyConfigurator { get; }

        /// <summary>
        /// Adds a producer to the cluster
        /// </summary> 
        /// <param name="name">The producer name used to get its instance</param>
        /// <param name="producer">A handler to configure the producer</param>
        /// <returns></returns>
        IClusterConfigurationBuilder AddProducer(string name, Action<IProducerConfigurationBuilder> producer);

        /// <summary>
        /// Adds a consumer to the cluster
        /// </summary>
        /// <param name="consumer">A handler to configure the consumer</param>
        /// <returns></returns>
        IClusterConfigurationBuilder AddConsumer(Action<IConsumerConfigurationBuilder> consumer);
    }
}
