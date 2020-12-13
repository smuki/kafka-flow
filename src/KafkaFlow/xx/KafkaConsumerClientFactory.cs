


using KafkaFlow.Consumers;
using Microsoft.Extensions.Options;

namespace DotNetCore.CAP.Kafka
{
    internal sealed class KafkaConsumerClientFactory : IConsumerClientFactory
    {
        private readonly IOptions<KafkaOptions> _kafkaOptions;

        public KafkaConsumerClientFactory(IOptions<KafkaOptions> kafkaOptions)
        {
            _kafkaOptions = kafkaOptions;
        }

        public IConsumerClient Create(string groupId)
        {
            try
            {
                //var client = new KafkaConsumerClient(groupId, _kafkaOptions);
                //client.Connect();
                return null;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
    }
}
