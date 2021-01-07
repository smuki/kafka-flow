namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    //using Acks = KafkaFlow.Acks;

    public class ProducerConfiguration: MessageProducerSettting
    {
        public ProducerConfiguration(
            ClusterConfiguration cluster,
            Acks? acks,
            ProducerConfig baseProducerConfig,
            IReadOnlyList<Action<string>> statisticsHandlers)
        {
            this.Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
        }

        public ClusterConfiguration Cluster { get; }

        public ProducerConfig BaseProducerConfig { get; }
      
    }
}
