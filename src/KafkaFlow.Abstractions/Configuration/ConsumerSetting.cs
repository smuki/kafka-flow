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

        public string this[string name]
        {
            get
            {
                return this._dict.TryGetValue(name, out var o) ? o : null;
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
