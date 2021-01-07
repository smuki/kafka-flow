namespace KafkaFlow.Producers
{
    using KafkaFlow.Configuration;
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to the configured producers
    /// </summary>
    public interface IProducerAccessor
    {
        void Initialize(IReadOnlyCollection<MessageProducerSettting> Producers);

        /// <summary>
        /// Gets a producer by its name
        /// </summary>
        /// <param name="name">The name defined in the producer configuration</param>
        /// <returns></returns>
        IMessageProducer GetProducer(string name);

        /// <summary>
        /// Gets a producer by its type
        /// </summary>
        /// <typeparam name="TProducer">The type defined in the configuration</typeparam>
        /// <returns></returns>
        IMessageProducer GetProducer<TProducer>();

        /// <summary>
        /// Returns all configured producers
        /// </summary>
        IEnumerable<IMessageProducer> Producers { get; }

        /// <summary>
        /// Gets a producer by its name
        /// </summary>
        /// <param name="name"></param>
        IMessageProducer this[string name] { get; }
    }
}
