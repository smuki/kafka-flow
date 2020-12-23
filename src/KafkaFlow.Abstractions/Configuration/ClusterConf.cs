namespace KafkaFlow.Configuration
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ClusterConf
    {
        private readonly Func<SecurityInformation> securityInformationHandler;
        private readonly List<ProducerParameter> producers = new List<ProducerParameter>();
        private readonly List<ConsumerParameter> consumers = new List<ConsumerParameter>();

        public ClusterConf(
            IEnumerable<string> brokers,
            Func<SecurityInformation> securityInformationHandler)
        {
            this.securityInformationHandler = securityInformationHandler;
            this.Brokers = brokers.ToList();
        }
        public void xxx()
        {
            var builder = new ConfigurationBuilder()
                           .AddJsonFile(@"d:\rpt.json");
            var configuration = builder.Build();

            IConfigurationSection data = configuration.GetSection("eventbus");

            foreach (IConfigurationSection it in data.GetChildren())
            {

                Console.WriteLine("Key      =" + it.Key);

                IConfigurationSection xxx = it.GetSection(it.Key);
                IConfigurationSection producer = it.GetSection("producer");

                Console.WriteLine("Key      =" + xxx.Key);
                Console.WriteLine("servers  =" + it["servers"]);
                Console.WriteLine("topic  =" + it["topic"]);

                Console.WriteLine("Value=" + xxx.Value);

            }
        }
public IReadOnlyCollection<string> Brokers { get; }

        public IReadOnlyCollection<ProducerParameter> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<ConsumerParameter> Consumers => this.consumers.AsReadOnly();

        public void AddConsumers(IEnumerable<ConsumerParameter> configurations) => this.consumers.AddRange(configurations);

        public void AddProducers(IEnumerable<ProducerParameter> configurations) => this.producers.AddRange(configurations);

        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
