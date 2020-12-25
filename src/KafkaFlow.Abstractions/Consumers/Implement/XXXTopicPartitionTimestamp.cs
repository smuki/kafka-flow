using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Consumers
{
    public class XXXTopicPartitionTimestamp
    {
        public XXXTopicPartitionTimestamp(XXXTopicPartition tp, long timestamp)
        {
            this.Timestamp = timestamp;
            this.TopicPartition = tp;
        }
        public XXXTopicPartitionTimestamp(string topic, int partition, long timestamp)
        {
            this.Topic = topic;
            this.Partition = partition;
            this.Timestamp = timestamp;
            this.TopicPartition = new XXXTopicPartition(this.Topic, this.Partition);

        }
        public string Topic { get; }
      
        public int Partition { get; }
      
        public long Timestamp { get; }
       
        public XXXTopicPartition TopicPartition { get; }
    }
}
