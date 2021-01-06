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
        public string GroupId { get; set; }
        public string ConsumerName { get; set; }
        public int WorkerCount { get; set; }
        public int BufferSize { get; set; }
        public bool AutoStoreOffsets { get; set; }
        public TimeSpan AutoCommitInterval { get; set; }
        private readonly Dictionary<string, string> _Parameter = new Dictionary<string, string>();
        public Factory<IDistributionStrategy> DistributionStrategyFactory { get; set; }
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
            this.Topic = conf.Get("topic");
            this.Servers = conf.Get("servers");
            this.WorkerCount = Util.ToInt(conf.Get("WorkerCount"));

            this.MessageProducer = new MessageProducerSettting(conf.GetSection("producer"));
            this.MessageProducer.Topic = this.Topic;
            this.MessageProducer.Brokers = this.Servers;

            this.MessageConsumer = new MessageConsumerSettting(conf.GetSection("consumer"));
            this.MessageConsumer.Topic = this.Topic;
            this.MessageConsumer.Brokers = this.Servers;
        }
    }
}
