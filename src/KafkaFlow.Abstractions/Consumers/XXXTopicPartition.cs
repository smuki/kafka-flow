using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePipeline.Consumers
{
    public class XXXTopicPartition
    {
        public XXXTopicPartition(string topic, int partition)
        {
            this.Topic = topic;
            this.Partition = partition;
        }

        public string Topic { get; }
        public int Partition { get; }
    }
}
