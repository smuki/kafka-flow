namespace MessagePipeline.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    //using Confluent.Kafka;

    public interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(IntermediateMessage message, CancellationToken stopCancellationToken = default);

        Task StartAsync(CancellationToken stopCancellationToken);
        
        Task StartAsync();

        Task StopAsync();
    }
}
