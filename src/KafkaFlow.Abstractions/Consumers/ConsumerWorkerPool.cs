namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Middleware;

    public class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly ConsumerSetting configuration;
        private readonly ILogHandler logHandler;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private readonly Factory<IDistributionStrategy> distributionStrategyFactory;

        private List<IConsumerWorker> workers = new List<IConsumerWorker>();

        private IDistributionStrategy distributionStrategy;
        private OffsetManager offsetManager;

        public ConsumerWorkerPool(
            IDependencyResolver dependencyResolver,
            ConsumerSetting configuration,
            ILogHandler logHandler)
        {
            this.dependencyResolver = dependencyResolver;
            this.configuration = configuration;
            this.logHandler = logHandler;

            var middlewares = configuration.MiddlewareConfiguration.Factories
                .Select(factory => factory(dependencyResolver))
                .ToList();

            this.middlewareExecutor = new MiddlewareExecutor(middlewares);
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
                Console.WriteLine("worker == null...");
                return;
            }

            this.offsetManager.AddOffset(message.TopicPartitionOffset);

            await worker.EnqueueAsync(message, stopCancellationToken).ConfigureAwait(false);
        }
    }
}
