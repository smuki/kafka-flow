namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IConsumerConfiguration
    {
        string GetParameter(string name);
        Dictionary<string, string> GetConsumerConfig();
    }
}
