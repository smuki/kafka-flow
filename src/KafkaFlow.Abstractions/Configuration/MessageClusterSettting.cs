using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Volte.Data.VolteDi;

namespace KafkaFlow.Configuration
{
    [Injection(InjectionType = InjectionType.Auto)]
    public class ClusterSettting
    {
        private readonly List<MessageProducerSettting> producers = new List<MessageProducerSettting>();

        private readonly List<MessageConsumerSettting> consumers = new List<MessageConsumerSettting>();
        public IReadOnlyCollection<MessageProducerSettting> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<MessageConsumerSettting> Consumers => this.consumers.AsReadOnly();

        public ClusterSettting(IConfiguration config)
        {
            foreach (var cluster in config.GetSection("eventbus").GetChildren())
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                foreach (var it in cluster.GetChildren())
                {
                    if (!string.IsNullOrWhiteSpace(it.Value))
                    {
                        parameter[it.Key] = it.Value;
                    }
                }

                MessageProducerSettting MessageProducer = new MessageProducerSettting(parameter,cluster.GetSection("producer"));
                MessageProducer.Name = cluster.Key;

                MessageConsumerSettting MessageConsumer = new MessageConsumerSettting(parameter,cluster.GetSection("consumer"));
                MessageConsumer.Name = cluster.Key;

                producers.Add(MessageProducer);
                consumers.Add(MessageConsumer);
            }

          
        }
    }
}
