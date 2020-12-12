namespace KafkaFlow.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(IntermediateMessage message, CancellationToken stopCancellationToken = default);

        Task StartAsync(CancellationToken stopCancellationToken);
        
        Task StartAsync();

        Task StopAsync();
    }
}
