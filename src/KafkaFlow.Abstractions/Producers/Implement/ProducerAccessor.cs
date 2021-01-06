namespace KafkaFlow.Producers
{
    using KafkaFlow.Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public class ProducerAccessor : IProducerAccessor
    {
        private Dictionary<string, IMessageProducer> producers;
        private IDependencyResolver dependencyResolver;

        public ProducerAccessor(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }
        //public ProducerAccessor(IEnumerable<IMessageProducer> producers)
        //{

        //    this.producers = producers.ToDictionary(x => x.ProducerName);
        //}
        public void Initialize(IReadOnlyCollection<MessageProducerSettting> Producers)
        {
            this.producers = Producers.Select(
                        producer =>
                        {
                            var x = dependencyResolver.Resolve<IMessageProducer>();
                            x.Initialize(producer);
                            return x;
                        }
                 ).ToDictionary(x => x.ProducerName);
        }
        public IMessageProducer GetProducer(string name)
        {
            return this.producers.TryGetValue(name, out var consumer) ? consumer : null;
        }
        public IMessageProducer GetProducer<TProducer>()
        {
            return   this.producers.TryGetValue(typeof(TProducer).FullName, out var consumer) ? consumer : null;
        }
        public IEnumerable<IMessageProducer> Producers 
        {
            get
            {
                return this.producers.Values;
            }
        }

        public IMessageProducer this[string name]
        {
            get
            {
                return this.GetProducer(name);
            }
        }
    }
}
