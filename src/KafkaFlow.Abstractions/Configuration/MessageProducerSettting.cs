using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Volte.Utils;

namespace KafkaFlow.Configuration
{
    public class MessageProducerSettting
    {
        public string Name { get; set; }
        public string Brokers { get; set; }
        public string Topic { get; set; }

        private readonly Dictionary<string, string> _Parameter = new Dictionary<string, string>();
        public IReadOnlyList<Action<string>> StatisticsHandlers { get; set; }
        public string this[string name]
        {
            get
            {
                return _Parameter.TryGetValue(name, out var o) ? o : null;
            }
        }
        public bool ContainsKey(string name)
        {
            return _Parameter.ContainsKey(name);
        }
        public Dictionary<string, string> Parameter
        {
            get
            {
                return _Parameter;
            }
        }
        public MessageProducerSettting()
        {

        }
        public MessageProducerSettting(IConfigurationSection conf)
        {
            foreach (var it in conf.GetChildren())
            {
                if (!string.IsNullOrWhiteSpace(it.Value))
                {
                    _Parameter[it.Key] = it.Value;
                }
            }
        }
    }
}
