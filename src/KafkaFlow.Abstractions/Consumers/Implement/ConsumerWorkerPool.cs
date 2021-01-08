namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using KafkaFlow.Middleware;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Both)]
    public class ConsumerWorkerPool : IConsumerWorkerPool
    {
        private readonly IVolteServiceResolver dependencyResolver;
        private readonly IMiddlewareExecutor middlewareExecutor;
        private MessageConsumerSettting configuration;
        private List<IConsumerWorker> workers = new List<IConsumerWorker>();
        private IDistributionStrategy distributionStrategy;
        private OffsetManager offsetManager;

        public ConsumerWorkerPool(
            IVolteServiceResolver dependencyResolver,
            IMiddlewareExecutor middlewareExecutor)
        {
            this.dependencyResolver = dependencyResolver;
            this.middlewareExecutor = middlewareExecutor;
        }

        public void Initialize(MessageConsumerSettting eventConsumer)
        {
            this.configuration = eventConsumer;

            var middlewares = dependencyResolver.Resolves<IMessageMiddleware>().Where(x =>
            {
                var injectionAttribute = x.GetType().GetCustomAttribute<MiddlewareAttribute>();
                if (injectionAttribute != null)
                {
                    return injectionAttribute.MiddlewareType == MiddlewareType.Consumer;
                }
                return false;
            })
           .ToList();

            middlewares.Sort((x, y) =>
            {
                var injectionAttributex = x.GetType().GetCustomAttribute<MiddlewareAttribute>();
                var injectionAttributey = y.GetType().GetCustomAttribute<MiddlewareAttribute>();
                return injectionAttributex.Priority.CompareTo(injectionAttributey.Priority);
            });

            middlewareExecutor.Initialize(middlewares);

            this.distributionStrategy = eventConsumer.DistributionStrategy;
            this.distributionStrategy.Initialize(this.workers.AsReadOnly());
        }

        public async Task StartAsync(
            IConsumerClient consumerClient,
            IEnumerable<XXXTopicPartition> partitions,
            CancellationToken stopCancellationToken = default)
        {
            this.offsetManager = new OffsetManager(new OffsetCommitter(consumerClient, this.configuration.AutoCommitInterval), partitions);

            int WorkerCount = configuration.WorkerCount;
            if (WorkerCount <= 0)
            {
                WorkerCount = partitions.Count() - 1;
                if (WorkerCount <= 0)
                {
                    WorkerCount = 4;
                }
            }

            await Task.WhenAll(
                    Enumerable.Range(0, WorkerCount).Select(
                            workerId =>
                            {
                                var worker = new ConsumerWorker(
                                    consumerClient,
                                    workerId,
                                    this.configuration,
                                    this.offsetManager,
                                    this.middlewareExecutor);

                                this.workers.Add(worker);

                                return worker.StartAsync(stopCancellationToken);
                            }))
                .ConfigureAwait(false);

            this.distributionStrategy = consumerClient.Parameter.DistributionStrategy;
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
