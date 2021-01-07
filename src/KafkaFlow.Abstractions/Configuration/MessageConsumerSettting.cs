﻿using KafkaFlow.Consumers.DistributionStrategies;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool ContainsKey(string name)
        {
            return _Parameter.ContainsKey(name);
        }

        public MessageConsumerSettting()
        {

        }
        public MessageConsumerSettting(IConfigurationSection conf)
        {
            foreach (var it in conf.GetChildren())
            {
                if (!string.IsNullOrWhiteSpace(it.Value))
                {
                    _Parameter[it.Key] = it.Value;
                }
            }
            if (this["DistributionStrategy"] == "FreeStrategy")
            {
                this.DistributionStrategy = new FreeWorkerDistributionStrategy();
            }
            else
            {
                this.DistributionStrategy = new BytesSumDistributionStrategy();
            }
            this.ConsumerName = conf.Get("ConsumerName");
            this.WorkerCount = Util.ToInt(conf.Get("WorkerCount"));
            if (this.ContainsKey("AutoStoreOffsets"))
            {
                this.AutoStoreOffsets = Util.ToBoolean(this["AutoStoreOffsets"]);
            }
            if (this.ContainsKey("WorkerCount"))
            {
                this.WorkerCount = Util.ToInt(this["WorkerCount"]);
            }
            else
            {
                this.WorkerCount = 10;
            }
            if (this.ContainsKey("AutoCommitInterval"))
            {
                this.AutoCommitInterval = TimeSpan.FromSeconds(Util.ToInt(this["AutoCommitInterval"]));
            }
            else
            {
                this.AutoCommitInterval = TimeSpan.FromSeconds(5);
            }

            if (this.WorkerCount <= 0 || this.WorkerCount >= 1000)
            {
                this.WorkerCount = 10;
            }
            if (this.BufferSize <= 0 || this.BufferSize >= 100)
            {
                this.BufferSize = 10;
            }
            if (this.AutoCommitInterval.TotalSeconds <= 0 || this.AutoCommitInterval.TotalSeconds > 60)
            {
                this.AutoCommitInterval = TimeSpan.FromSeconds(5);
            }
        }
    }
}
