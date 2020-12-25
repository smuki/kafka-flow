namespace KafkaFlow.Configuration
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventBusSetting
    {
        private readonly Func<SecurityInformation> securityInformationHandler;
        private readonly List<ProducerSetting> producers = new List<ProducerSetting>();
        private readonly List<ConsumerSetting> consumers = new List<ConsumerSetting>();
        private IConfiguration config;

        public EventBusSetting(
            IConfiguration config,
            Func<SecurityInformation> securityInformationHandler)
        {
            this.securityInformationHandler = securityInformationHandler;
            this.config = config;
        }
        public EventBusSetting Build(IConfigurationSection config)
        {

            IConfigurationSection producer = config.GetSection("producer");
            IConfigurationSection consumer = config.GetSection("consumer");

            this.Brokers = config["servers"];
            this.Topic = config["topic"];

            return this;
        }
        public string Brokers { get; set; }
        public string Topic { get; set; }

        public IReadOnlyCollection<ProducerSetting> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<ConsumerSetting> Consumers => this.consumers.AsReadOnly();

        public void AddConsumers(IEnumerable<ConsumerSetting> configurations) => this.consumers.AddRange(configurations);

        public void AddProducers(IEnumerable<ProducerSetting> configurations) => this.producers.AddRange(configurations);

        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
