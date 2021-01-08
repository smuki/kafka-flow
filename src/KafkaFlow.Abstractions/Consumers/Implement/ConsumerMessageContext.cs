namespace MessagePipeline
{
    using MessagePipeline.Consumers;
    using System.Text;

    public class ConsumerMessageContext : IMessageContext
    {
        private readonly IntermediateMessage result;

        public ConsumerMessageContext(
            IMessageContextConsumer consumer,
            IntermediateMessage result,
            int workerId,
            string groupId)
        {
            this.result = result;
            this.Consumer = consumer;
            this.Message = result.Payload;
            this.Headers = result.Headers;
            this.WorkerId = workerId;
            this.GroupId = groupId;
        }

        public int WorkerId { get; }

        public byte[] PartitionKey
        {
            get
            {
                return Encoding.UTF8.GetBytes(this.result.Partition.ToString());
            }
        }

        public object Message { get; private set; }

        public IMessageHeaders Headers { get; }

        public string Topic => this.result.Topic;

        public string GroupId { get; }

        public int? Partition => this.result.Partition;

        public long? Offset => this.result.Offset;

        public IMessageContextConsumer Consumer { get; }

        public void TransformMessage(object message)
        {
            this.Message = message;
        }

        public IMessageContext Clone()
        {
            return (IMessageContext)this.MemberwiseClone();
        }
    }
}
