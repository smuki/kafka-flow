namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ClusterConfiguration
    {
        private readonly Func<SecurityInformation> securityInformationHandler;
        //private readonly List<ProducerConfiguration> producers = new List<ProducerConfiguration>();
        private readonly List<ConsumerConfiguration> consumers = new List<ConsumerConfiguration>();

        public ClusterConfiguration(
            KafkaConfiguration kafka,
            IEnumerable<string> brokers,
            Func<SecurityInformation> securityInformationHandler)
        {
            this.securityInformationHandler = securityInformationHandler;
            this.Kafka = kafka;
        }

        public KafkaConfiguration Kafka { get; }
        //public IReadOnlyCollection<ProducerConfiguration> Producers => this.producers.AsReadOnly();

        public IReadOnlyCollection<ConsumerConfiguration> Consumers => this.consumers.AsReadOnly();

        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
