namespace KafkaFlow.TypedHandler
{
    using System;
    using KafkaFlow.Configuration;
    using KafkaFlow.Dependency;

    /// <summary>
    /// Extension methods over <see cref="IConsumerMiddlewareConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds typed handler middleware
        /// </summary>
        /// <param name="builder">Instance of <see cref="IConsumerMiddlewareConfigurationBuilder"/></param>
        /// <param name="configure">A handler to configure the middleware</param>
        /// <returns></returns>
        public static IConsumerMiddlewareConfigurationBuilder AddTypedHandlers(
            this IConsumerMiddlewareConfigurationBuilder builder,
            Action<TypedHandlerConfigurationBuilder> configure)
        {
            return builder;
        }
    }
}
