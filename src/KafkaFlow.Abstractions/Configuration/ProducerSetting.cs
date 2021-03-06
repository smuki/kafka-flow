namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;

    public class ProducerSetting
    {
        public string Name { get; set; }

        public string DefaultTopic { get; set; }

        public Acks? Acks { get; set; }

        public MiddlewareConfiguration MiddlewareConfiguration { get; set; }

        public IReadOnlyList<Action<string>> StatisticsHandlers { get; set; }
    }
}
