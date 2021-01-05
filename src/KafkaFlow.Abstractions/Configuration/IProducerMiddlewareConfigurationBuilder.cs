namespace KafkaFlow.Configuration
{
    /// <summary>
    /// Used to build the producer middlewares configuration
    /// </summary>
    public interface IProducerMiddlewareConfigurationBuilder
    {
        /// <summary>
        /// Gets the dependency injection configurator
        /// </summary>
        IDependencyConfigurator DependencyConfigurator { get; }

        /// <summary>
        /// Registers a middleware
        /// </summary>
        /// <param name="factory">A factory to create the instance</param>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        IProducerMiddlewareConfigurationBuilder Add<T>(Factory<T> factory)
            where T : class, IMessageMiddleware;

        /// <summary>
        /// Registers a middleware
        /// </summary>
        /// <typeparam name="T">A class that implements the <see cref="IMessageMiddleware"/></typeparam>
        /// <returns></returns>
        //IProducerMiddlewareConfigurationBuilder Add<T>()           where T : class, IMessageMiddleware;
    }
}
