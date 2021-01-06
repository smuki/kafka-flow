//namespace KafkaFlow.Configuration
//{
//    //using Microsoft.Extensions.Configuration;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    public class EventBusSetting
//    {
//        private readonly Func<SecurityInformation> securityInformationHandler;
//        private readonly List<ProducerSetting> producers = new List<ProducerSetting>();
//        private readonly List<MessageConsumerSettting> consumers = new List<MessageConsumerSettting>();

//        public EventBusSetting(
//            Func<SecurityInformation> securityInformationHandler)
//        {
//            this.securityInformationHandler = securityInformationHandler;
//        }
        
//        public string Brokers { get; set; }
//        public string Topic { get; set; }

//        public IReadOnlyCollection<ProducerSetting> Producers => this.producers.AsReadOnly();

//        public IReadOnlyCollection<MessageConsumerSettting> Consumers => this.consumers.AsReadOnly();

//        public void AddConsumers(IEnumerable<MessageConsumerSettting> configurations) => this.consumers.AddRange(configurations);

//        public void AddProducers(IEnumerable<ProducerSetting> configurations) => this.producers.AddRange(configurations);

//        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
//    }
//}
