namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Producers;
    using KafkaFlow.Dependency;

    public class ClusterConfigurationBuilder : IClusterConfigurationBuilder
    {
        //private readonly List<ProducerConfigurationBuilder> producers = new List<ProducerConfigurationBuilder>();

        private IEnumerable<string> brokers;
        private Func<SecurityInformation> securityInformationHandler;

        public ClusterConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.DependencyConfigurator = dependencyConfigurator;
        }

        public IDependencyConfigurator DependencyConfigurator { get; }

        public ClusterConfiguration Build(KafkaConfiguration kafkaConfiguration)
        {
            var configuration = new ClusterConfiguration(
                kafkaConfiguration,
                this.brokers.ToList(),
                this.securityInformationHandler);

            return configuration;
        }

        public IClusterConfigurationBuilder AddProducer<TProducer>(Action<IProducerConfigurationBuilder> producer)
        {
            this.DependencyConfigurator.AddSingleton<IMessageProducer<TProducer>>(
                resolver => new MessageProducerWrapper<TProducer>(
                    resolver.Resolve<IProducerAccessor>().GetProducer<TProducer>()));

            return this.AddProducer(typeof(TProducer).FullName, producer);
        }

        public IClusterConfigurationBuilder AddProducer(string name, Action<IProducerConfigurationBuilder> producer)
        {
            //var builder = new ProducerConfigurationBuilder(this.DependencyConfigurator, name);

            //producer(builder);

            //this.producers.Add(builder);

            return this;
        }
    }
}
