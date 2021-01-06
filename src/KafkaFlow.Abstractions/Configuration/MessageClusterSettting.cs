using KafkaFlow.Dependency;
using KafkaFlow.Producers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volte.Data.VolteDi;
using KafkaFlow.Producers;
using Volte.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaFlow.Configuration
{
    [Injection(InjectionType = InjectionType.Auto)]
    public class ClusterSettting
    {
        private readonly IDependencyConfigurator dependencyConfigurator;
        private readonly List<MessageProducerSettting> producers = new List<MessageProducerSettting>();
        private readonly List<MessageConsumerSettting> consumers = new List<MessageConsumerSettting>();
        public IReadOnlyCollection<MessageProducerSettting> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<MessageConsumerSettting> Consumers => this.consumers.AsReadOnly();

        public ClusterSettting( IConfiguration config)
        {

            foreach (var cluster in config.GetSection("eventbus").GetChildren())
            {

                string topic = cluster.Get("topic");
                string servers = cluster.Get("servers");

                MessageProducerSettting MessageProducer = new MessageProducerSettting(cluster.GetSection("producer"));
                MessageProducer.Topic = topic;
                MessageProducer.Brokers = servers;
                MessageProducer.Name = cluster.Key;

                MessageConsumerSettting MessageConsumer = new MessageConsumerSettting(cluster.GetSection("consumer"));
                MessageConsumer.Topic = topic;
                MessageConsumer.Brokers = servers;
                MessageConsumer.Name = cluster.Key;

                producers.Add(MessageProducer);
                consumers.Add(MessageConsumer);
            }

          
        }
    }
}
