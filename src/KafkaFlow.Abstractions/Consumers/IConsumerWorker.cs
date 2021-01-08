namespace MessagePipeline.Consumers
{
    using System.Threading;
    using System.Threading.Tasks;
    public interface IConsumerWorker : IWorker
    {
        ValueTask EnqueueAsync(IntermediateMessage message, CancellationToken stopCancellationToken = default);

        Task StartAsync(CancellationToken stopCancellationToken);
        
        Task StartAsync();

        Task StopAsync();
    }
}
