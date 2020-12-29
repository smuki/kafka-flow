namespace KafkaFlow.Consumers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Auto,Lifetime =InjectionLifetime.Singleton)]

    public class ConsumerManager : IConsumerManager
    {
        private readonly ConcurrentDictionary<string, IMessageConsumer> consumers = new ConcurrentDictionary<string, IMessageConsumer>();

        public IMessageConsumer GetConsumer(string name)
        {
            return this.consumers.TryGetValue(name, out var consumer) ? consumer : null;
        }
        public IEnumerable<IMessageConsumer> Consumers
        {
            get
            {
                return this.consumers.Values;
            }
        }
        public IMessageConsumer this[string name]
        {
            get
            {
                return this.GetConsumer(name);
            }
        }

        public void AddOrUpdate(IMessageConsumer consumer)
        {
            this.consumers.AddOrUpdate(consumer.ConsumerName, consumer, (x, y) => consumer);
        }
    }
}
