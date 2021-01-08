namespace MessagePipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Microsoft.Extensions.Configuration;
    using MessagePipeline.Configuration;
    using MessagePipeline.Consumers;
    using MessagePipeline.Producers;
    using Volte.Data.VolteDi;
    using Volte.Utils;

    [Injection(InjectionType = InjectionType.Auto, Lifetime = InjectionLifetime.Singleton)]
    public class KafkaBus : IKafkaBus
    {
        private readonly IVolteServiceResolver dependencyResolver;
        private readonly IConsumerManager consumerManager;
        private readonly IList<IConsumerClient> consumers = new List<IConsumerClient>();

        public KafkaBus(
            IVolteServiceResolver dependencyResolver,
            IConsumerManager consumerManager)
        {
            this.dependencyResolver = dependencyResolver;
            this.consumerManager = consumerManager;
        }

        public IConsumerAccessor Consumers => this.consumerManager;

        public IProducerAccessor Producers { get; }

        public async Task Initialize(CancellationToken stopCancellationToken = default)
        {
            try
            {
                var cluster = dependencyResolver.Resolve<ClusterSettting>();
                
                var ProducerAccessor = dependencyResolver.Resolve<IProducerAccessor>();
                ProducerAccessor.Initialize(cluster.Producers);

                foreach (var ccc in cluster.Consumers)
                {

                    var consumerWorkerPool = dependencyResolver.Resolve<IConsumerWorkerPool>();
                    consumerWorkerPool.Initialize(ccc);

                    var dependencyScope = this.dependencyResolver.CreateScope();
                    var consumer = dependencyScope.Resolver.Resolve<IConsumerClient>("Kafka");
                    consumer.Initialize(consumerWorkerPool, ccc, stopCancellationToken);

                    this.consumers.Add(consumer);

                    await consumer.StartAsync().ConfigureAwait(false);
                }
            }
            catch(Exception ex)
            {
                NLogger.Error(ex);

            }
            await Task.CompletedTask;
        }
        public async Task StartAsync(CancellationToken stopCancellationToken = default)
        {
            await this.Initialize(stopCancellationToken);
        }

        public Task StopAsync()
        {
            return Task.WhenAll(this.consumers.Select(x => x.StopAsync()));
        }
    }
}
