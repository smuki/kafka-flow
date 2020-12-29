namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Dependency;
    using KafkaFlow.Middleware;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.TypedHandler;

    public class KafkaConfigurationBuilder : IKafkaConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;
        private readonly List<ClusterConfigurationBuilder> clusters = new List<ClusterConfigurationBuilder>();
        private Type logHandler = typeof(NullLogHandler);

        public KafkaConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
        }

        public KafkaConfiguration Build()
        {
            var configuration = new KafkaConfiguration();

            configuration.AddClusters(this.clusters.Select(x => x.Build(configuration)));

            this.dependencyConfigurator.AddSingleton<IProducerAccessor>(
                resolver => new ProducerAccessor(
                    configuration.Clusters
                        .SelectMany(x => x.Producers)
                        .Select(
                            producer => new MessageProducer(
                                resolver,
                                producer))));

            var consumerManager = new ConsumerManager();
            var DefaultMessageTypeResolver = new DefaultMessageTypeResolver();
            this.dependencyConfigurator
                .AddTransient(typeof(ILogHandler), this.logHandler)
                //.AddSingleton<IMiddlewareExecutor, MiddlewareExecutor>()
                //.AddSingleton<IConsumerWorkerPool, ConsumerWorkerPool>()
                //.AddSingleton<IMessageTypeResolver, DefaultMessageTypeResolver>()
                //.AddSingleton<IMessageMiddleware, SerializerProducerMiddleware>()
                //.AddSingleton<IMessageSerializer, JsonMessageSerializer>()
                //.AddSingleton<IMessageMiddleware, TypedHandlerMiddleware>()
                .AddSingleton<IConsumerAccessor>(consumerManager)
                .AddSingleton<IConsumerAccessor>(consumerManager)
               .AddSingleton<IConsumerManager>(consumerManager);

            return configuration;
        }

        public IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster)
        {
            var builder = new ClusterConfigurationBuilder(this.dependencyConfigurator);

            cluster(builder);

            this.clusters.Add(builder);

            return this;
        }

        public IKafkaConfigurationBuilder UseLogHandler<TLogHandler>() where TLogHandler : ILogHandler
        {
            this.logHandler = typeof(TLogHandler);
            return this;
        }
    }
}
