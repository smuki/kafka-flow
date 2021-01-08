namespace MessagePipeline.Producers
{
    using MessagePipeline.Configuration;
    using System;
    using System.Threading.Tasks;
    public class MessageProducerWrapper<TProducer> : IMessageProducer<TProducer>
    {
        private readonly IMessageProducer producer;

        public MessageProducerWrapper(IMessageProducer producer)
        {
            this.producer = producer;
        }
        public void Initialize(MessageProducerSettting configuration)
        {

        }
        public string ProducerName => this.producer.ProducerName;

        public Task<XXXDeliveryResult> ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            return this.producer.ProduceAsync(topic, partitionKey, message, headers);
        }

        public Task<XXXDeliveryResult> ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            return this.producer.ProduceAsync(partitionKey, message, headers);
        }
        public void Produce(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<XXXDeliveryReport> deliveryHandler = null)
        {
            this.producer.Produce(topic, partitionKey, message, headers, deliveryHandler);
        }

        public void Produce(
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<XXXDeliveryReport> deliveryHandler = null)
        {
            this.producer.Produce(partitionKey, message, headers, deliveryHandler);
        }
    }
}
