namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;
    using KafkaFlow.Dependency;

    public class ProducerMiddlewareConfigurationBuilder
        : IProducerMiddlewareConfigurationBuilder
    {
        public IDependencyConfigurator DependencyConfigurator { get; }

        private readonly List<Factory<IMessageMiddleware>> middlewaresFactories =
            new List<Factory<IMessageMiddleware>>();

        public ProducerMiddlewareConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IProducerMiddlewareConfigurationBuilder Add<T>(Factory<T> factory) where T : class, IMessageMiddleware
        {
            this.middlewaresFactories.Add(factory);
            return this;
        }
    }
}
