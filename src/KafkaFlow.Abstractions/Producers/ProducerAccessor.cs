namespace KafkaFlow.Producers
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProducerAccessor : IProducerAccessor
    {
        private readonly Dictionary<string, IMessageProducer> producers;

        public ProducerAccessor(IEnumerable<IMessageProducer> producers)
        {
            this.producers = producers.ToDictionary(x => x.ProducerName);
        }

        public IMessageProducer GetProducer(string name) =>
            this.producers.TryGetValue(name, out var consumer) ? consumer : null;

        public IMessageProducer GetProducer<TProducer>() =>
            this.producers.TryGetValue(typeof(TProducer).FullName, out var consumer) ? consumer : null;

        public IEnumerable<IMessageProducer> All => this.producers.Values;

        public IMessageProducer this[string name] => this.GetProducer(name);
    }
}
