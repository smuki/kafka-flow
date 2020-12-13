namespace KafkaFlow.Admin.Handlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Consumers;
    using KafkaFlow.TypedHandler;

    internal class ResetConsumerOffsetHandler : IMessageHandler<ResetConsumerOffset>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ResetConsumerOffsetHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ResetConsumerOffset message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            if (consumer is null)
            {
                return Task.CompletedTask;
            }

            var offsets = consumer.OffsetsForTimes(
                consumer.Assignment.Select(
                    partition =>
                        new XXXTopicPartitionTimestamp(partition,0)),
                TimeSpan.FromSeconds(30));

            return consumer.OverrideOffsetsAndRestartAsync(offsets);
        }
    }
}
