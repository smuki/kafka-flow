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

        private readonly Dictionary<string, string> parameter = new Dictionary<string, string>();
        public IReadOnlyList<Action<string>> StatisticsHandlers { get; set; }
        public string this[string name]
        {
            get
            {
                return parameter.TryGetValue(name, out var o) ? o : null;
            }
        }
        public bool ContainsKey(string name)
        {
            return parameter.ContainsKey(name);
        }
        public Dictionary<string, string> Parameter
        {
            get
            {
                return parameter;
            }
        }
        public MessageProducerSettting()
        {

        }
        public MessageProducerSettting(Dictionary<string, string> _Parameter, IConfigurationSection conf)
        {
            this.parameter = _Parameter;
            foreach (var it in conf.GetChildren())
            {
                if (!string.IsNullOrWhiteSpace(it.Value))
                {
                    _Parameter[it.Key] = it.Value;
                }
            }
            this.Topic = this["topic"];
            this.Brokers = this["servers"];
        }
    }
}
