namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;
    using KafkaFlow.Dependency;

    public class ConsumerMiddlewareConfigurationBuilder
        : IConsumerMiddlewareConfigurationBuilder
    {
        public IDependencyConfigurator DependencyConfigurator { get; }

        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories =
            new List<Factory<IMessageMiddleware>>();

        public ConsumerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IConsumerMiddlewareConfigurationBuilder Add<T>(Factory<T> factory) where T : class, IMessageMiddleware
        {
            this.middlewaresFactories.Add(factory);
            return this;
        }

    }
}
