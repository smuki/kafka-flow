namespace KafkaFlow.Configuration
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSource
    {
        private readonly Func<SecurityInformation> securityInformationHandler;
        private readonly List<EventProducer> producers = new List<EventProducer>();
        private readonly List<EventConsumer> consumers = new List<EventConsumer>();
        private IConfiguration config;

        public EventSource(
            IConfiguration config,
            Func<SecurityInformation> securityInformationHandler)
        {
            this.securityInformationHandler = securityInformationHandler;
            this.config = config;
        }
        public EventSource Build(IConfigurationSection config)
        {

            IConfigurationSection producer = config.GetSection("producer");
            IConfigurationSection consumer = config.GetSection("consumer");

            this.Brokers = config["servers"];
            this.Topic = config["topic"];

            return this;
        }
        public string Brokers { get; set; }
        public string Topic { get; set; }

        public IReadOnlyCollection<EventProducer> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<EventConsumer> Consumers => this.consumers.AsReadOnly();

        public void AddConsumers(IEnumerable<EventConsumer> configurations) => this.consumers.AddRange(configurations);

        public void AddProducers(IEnumerable<EventProducer> configurations) => this.producers.AddRange(configurations);

        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
