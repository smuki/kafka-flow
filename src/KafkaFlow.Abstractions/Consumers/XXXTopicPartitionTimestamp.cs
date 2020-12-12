using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Consumers
{
    public class XXXTopicPartitionTimestamp
    {
        public XXXTopicPartitionTimestamp(XXXTopicPartition tp, DateTimeOffset timestamp)
        {

        }
        public XXXTopicPartitionTimestamp(string topic, int partition, DateTimeOffset timestamp)
        {

        }
        public string Topic { get; }
      
        public int Partition { get; }
      
        public DateTimeOffset Timestamp { get; }
       
        public XXXTopicPartition TopicPartition { get; }
    }
}
