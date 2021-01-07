using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Volte.Utils;

namespace KafkaFlow.Configuration
{
    public class MessageQueueSettting
    {
        public string Topic { get; set; }
        public string Servers { get; set; }

        private readonly Dictionary<string, string> _Parameter = new Dictionary<string, string>();
        MessageConsumerSettting MessageConsumer { get; set; }
        MessageProducerSettting MessageProducer { get; set; }
        public string this[string name]
        {
            get
            {
                return _Parameter.TryGetValue(name, out var o) ? o : null;
            }
        }
        public MessageQueueSettting(IConfigurationSection conf)
        {
            foreach (var it in conf.GetChildren())
            {
                if (!string.IsNullOrWhiteSpace(it.Value))
                {
                    _Parameter[it.Key] = it.Value;
                }
            }
            this.Topic = this["topic"];
            this.Servers = this["servers"];

            this.MessageProducer = new MessageProducerSettting(conf.GetSection("producer"));
            this.MessageProducer.Topic = this.Topic;
            this.MessageProducer.Brokers = this.Servers;

            this.MessageConsumer = new MessageConsumerSettting(conf.GetSection("consumer"));
            this.MessageConsumer.Topic = this.Topic;
            this.MessageConsumer.Brokers = this.Servers;
        }
    }
}
