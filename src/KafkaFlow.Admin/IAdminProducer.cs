namespace MessagePipeline.Admin
{
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;

    public interface IAdminProducer
    {
        Task ProduceAsync(IAdminMessage message);
    }
}
