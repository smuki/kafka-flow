namespace KafkaFlow.Configuration
{
    using System.Collections.Generic;

    public class EventBusConfiguration
    {
        private readonly List<EventBusSetting> clusters = new List<EventBusSetting>();

        public IReadOnlyCollection<EventBusSetting> Clusters => this.clusters;

        public void AddClusters(IEnumerable<EventBusSetting> configurations) => this.clusters.AddRange(configurations);
    }
}
