namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using Microsoft.Extensions.Configuration;

    internal class KafkaBus : IKafkaBus
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly KafkaConfiguration configuration;
        private readonly IConsumerManager consumerManager;
        private readonly ILogHandler logHandler;
        private readonly IList<KafkaConsumer> consumers = new List<KafkaConsumer>();
        private readonly IConfiguration config;

        public KafkaBus(
            IDependencyResolver dependencyResolver,
            IConsumerManager consumerManager,
            IProducerAccessor accessor,
            ILogHandler logHandler,
        //IConfiguration config,
        KafkaConfiguration configuration)
        {
            this.dependencyResolver = dependencyResolver;
            this.configuration = configuration;
            this.consumerManager = consumerManager;
            this.logHandler = logHandler;
            this.Producers = accessor;
            //this.config = config;
        }

        public IConsumerAccessor Consumers => this.consumerManager;

        public IProducerAccessor Producers { get; }
       
        public async Task Initialize(CancellationToken stopCancellationToken = default)
        {
            //foreach (var vvv in config.GetSection("eventbus").GetChildren())
            //{

                var dependencyScope = this.dependencyResolver.CreateScope();
                ConsumerSetting vconsumerConfiguration = new ConsumerSetting();

                //var consumerWorkerPool = new ConsumerWorkerPool(
                //    dependencyScope.Resolver,
                //    vconsumerConfiguration,
                //    this.logHandler
                //    );
                var consumer = dependencyScope.Resolver.Resolve<IConsumerClient>("Kafka");
                consumer.Initialize(vconsumerConfiguration);

                //var consumer = new KafkaConsumer(
                //     vconsumerConfiguration,
                //     this.consumerManager,
                //     this.logHandler,
                //     consumerWorkerPool,
                //     stopCancellationToken);

                //this.consumers.Add(consumer);

                //await consumer.StartAsync().ConfigureAwait(false);

                // Console.WriteLine("Key = " + v.Key);
            //}
            await Task.CompletedTask;
        }
        public async Task StartAsync(CancellationToken stopCancellationToken = default)
        {
           await this.Initialize(stopCancellationToken);

            foreach (var consumerConfiguration in this.configuration.Clusters.SelectMany(cl => cl.Consumers))
            {
                var dependencyScope = this.dependencyResolver.CreateScope();

                var consumerWorkerPool = new ConsumerWorkerPool(
                    dependencyScope.Resolver,
                    consumerConfiguration,
                    this.logHandler
                    );

                var consumer = new KafkaConsumer(
                    consumerConfiguration,
                    this.consumerManager,
                    this.logHandler,
                    consumerWorkerPool,
                    stopCancellationToken);

                this.consumers.Add(consumer);

                await consumer.StartAsync().ConfigureAwait(false);
            }
        }

        public Task StopAsync()
        {
            return Task.WhenAll(this.consumers.Select(x => x.StopAsync()));
        }
    }
}
