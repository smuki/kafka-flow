namespace KafkaFlow.Producers
{
    using KafkaFlow.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Auto,Lifetime =InjectionLifetime.Singleton)]
    public class ProducerAccessor : IProducerAccessor
    {
        private Dictionary<string, IMessageProducer> producers;
        private IVolteServiceResolver dependencyResolver;

        public ProducerAccessor(IVolteServiceResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

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
