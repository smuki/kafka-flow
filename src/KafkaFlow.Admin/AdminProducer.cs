namespace MessagePipeline.Admin
{
    using System;
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;
    using MessagePipeline.Producers;

    internal class AdminProducer : IAdminProducer
    {
        private readonly IMessageProducer<AdminProducer> producer;

        public AdminProducer(IMessageProducer<AdminProducer> producer) => this.producer = producer;

        public Task ProduceAsync(IAdminMessage message) =>
            this.producer.ProduceAsync(Guid.NewGuid().ToString(), message);
    }
}
