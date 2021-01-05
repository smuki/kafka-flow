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

        /// <summary>
        /// Sets the default topic to be used when producing messages
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns></returns>
        IProducerConfigurationBuilder DefaultTopic(string topic);

        /// <summary>
        /// Sets the <see cref="Acks"/> to be used when producing messages
        /// </summary>
        /// <param name="acks"></param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithAcks(Acks acks);

        /// <summary>
        /// Adds a handler for the Kafka producer statistics
        /// </summary>
        /// <param name="statisticsHandler">A handler for the statistics</param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithStatisticsHandler(Action<string> statisticsHandler);

        /// <summary>
        /// Sets the interval the statistics are emitted
        /// </summary>
        /// <param name="statisticsIntervalMs">The interval in miliseconds</param>
        /// <returns></returns>
        IProducerConfigurationBuilder WithStatisticsIntervalMs(int statisticsIntervalMs);
    }
}
