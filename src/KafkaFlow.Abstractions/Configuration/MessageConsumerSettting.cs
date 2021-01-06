using KafkaFlow.Consumers.DistributionStrategies;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Volte.Utils;

namespace KafkaFlow.Configuration
{
    public class MessageConsumerSettting
    {
        public string Name { get; set; }
        public string Brokers { get; set; }
        public string Topic { get; set; }
        public string GroupId { get; set; }
        public string ConsumerName { get; set; }
        public int WorkerCount { get; set; }
        public int BufferSize { get; set; }
        public bool AutoStoreOffsets { get; set; }
        public TimeSpan AutoCommitInterval { get; set; }
        public IReadOnlyList<Action<string>> StatisticsHandlers { get; set; }

        private readonly Dictionary<string, string> _Parameter = new Dictionary<string, string>();
        public IDistributionStrategy DistributionStrategy { get; set; } 

        public string this[string name]
        {
            get
            {
                return _Parameter.TryGetValue(name, out var o) ? o : null;
            }
        }
        public MessageConsumerSettting()
        {
        }
        public MessageConsumerSettting(IConfigurationSection conf)
        {
            this.DistributionStrategy = new BytesSumDistributionStrategy();
            this.ConsumerName = conf.Get("ConsumerName");
            this.WorkerCount = Util.ToInt(conf.Get("WorkerCount"));
            if (this.WorkerCount<=0 || this.WorkerCount >= 1000)
            {
                this.WorkerCount = 10;
            }
            if (this.BufferSize <= 0 || this.BufferSize >= 100)
            {
                this.BufferSize = 10;
            }
            

        }
    }
}
