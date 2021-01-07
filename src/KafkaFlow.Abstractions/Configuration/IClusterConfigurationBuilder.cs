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

    }
}
