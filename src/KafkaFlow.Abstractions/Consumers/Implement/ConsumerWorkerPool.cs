namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Middleware;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Both)]
    public class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly ILogHandler logHandler;
        private ConsumerSetting configuration;
        private IMiddlewareExecutor middlewareExecutor;
        private Factory<IDistributionStrategy> distributionStrategyFactory;

        private List<IConsumerWorker> workers = new List<IConsumerWorker>();

        private IDistributionStrategy distributionStrategy;
        private OffsetManager offsetManager;

        public ConsumerWorkerPool(
            IDependencyResolver dependencyResolver,
            IMiddlewareExecutor middlewareExecutor,
            ILogHandler logHandler)
        {
            this.dependencyResolver = dependencyResolver;
            this.logHandler = logHandler;
            this.middlewareExecutor = middlewareExecutor;
        }

        public void Initialize(ConsumerSetting eventConsumer)
        {
            this.configuration = eventConsumer;
            //var middlewares = configuration.MiddlewareConfiguration.Factories
            //    .Select(factory => factory(dependencyResolver))
            //    .ToList();

            //this.middlewareExecutor = this.dependencyResolverScope.Resolver.Resolve<IMiddlewareExecutor>();
            //this.middlewareExecutor.Initialize(middlewares);
            //this.middlewareExecutor = new MiddlewareExecutor(middlewares);
            this.distributionStrategyFactory = configuration.DistributionStrategyFactory;
        }

        public async Task StartAsync(
            IConsumerClient consumerClient,
            IEnumerable<XXXTopicPartition> partitions,
            CancellationToken stopCancellationToken = default)
        {
            this.offsetManager = new OffsetManager(new OffsetCommitter(consumerClient, this.configuration.AutoCommitInterval, this.logHandler), partitions);

            await Task.WhenAll(
                    Enumerable.Range(0, this.configuration.WorkerCount).Select(
                            workerId =>
                            {
                                var worker = new ConsumerWorker(
                                    consumerClient,
                                    workerId,
                                    this.configuration,
                                    this.offsetManager,
                                    this.logHandler,
                                    this.middlewareExecutor);

                                this.workers.Add(worker);

                                return worker.StartAsync(stopCancellationToken);
                            }))
                .ConfigureAwait(false);

            this.distributionStrategy = this.distributionStrategyFactory(this.dependencyResolver);
            this.distributionStrategy.Initialize(this.workers.AsReadOnly());
        }

        public async Task StopAsync()
        {
            var currentWorkers = this.workers;
            this.workers = new List<IConsumerWorker>();

            await Task.WhenAll(currentWorkers.Select(x => x.StopAsync())).ConfigureAwait(false);

            this.offsetManager?.Dispose();
            this.offsetManager = null;
        }

        public async Task EnqueueAsync(IntermediateMessage message, CancellationToken stopCancellationToken = default)
        {
            var worker = (IConsumerWorker)await this.distributionStrategy
                .GetWorkerAsync(Encoding.UTF8.GetBytes(message.Partition.ToString()), stopCancellationToken)
                .ConfigureAwait(false);

            if (worker == null)
            {
                return;
            }

            this.offsetManager.AddOffset(message.TopicPartitionOffset);

            await worker.EnqueueAsync(message, stopCancellationToken).ConfigureAwait(false);
        }

    }
}
