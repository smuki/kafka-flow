using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Configuration
{
    public class ConsumerParameter
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

        public void SetParameter(string name, string value)
        {
            _dict[name] = value;
        }
        public string GetParameter(string name)
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
        public Dictionary<string, string> GetParameters()
        {
            return _dict;
        }
    }
}
