using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Configuration
{
    public class ConsumerSetting
    {
        public string Topic { get; set; }
        public string GroupId { get; set; }
        public string ConsumerName { get; set; }
        public int WorkerCount { get; set; }
        public int BufferSize { get; set; }
        public bool AutoStoreOffsets { get; set; }
        public TimeSpan AutoCommitInterval { get; set; }
        private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();
        public Factory<IDistributionStrategy> DistributionStrategyFactory { get; set; }

        public MiddlewareConfiguration MiddlewareConfiguration { get; set; }
        public ConsumerSetting Build(IConfigurationSection config)
        {

            IConfigurationSection consumer = config.GetSection("consumer");

            this.Topic = config["topic"];

            return this;
        }
        public string this[string name]
        {
            get
            {
                if (_dict.ContainsKey(name))
                {
                    return null;
                }
                else
                {
                    return _dict[name];
                }
            }
        }
        public void SetParameter(string name, string value)
        {
            _dict[name] = value;
        }
        public Dictionary<string, string> GetParameters()
        {
            return _dict;
        }
    }
}
