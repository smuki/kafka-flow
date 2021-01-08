using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePipeline.Consumers
{
    public class XXXTopicPartitionOffset
    {

        public XXXTopicPartitionOffset(XXXTopicPartition tp, long offset)
        {
            this.TopicPartition = tp;
            this.Offset = offset;
        }

        public XXXTopicPartitionOffset(string topic, int partition, long offset)
        {
            this.Topic = topic;
            this.Partition = partition;
            this.Offset = offset;
            this.TopicPartition = new XXXTopicPartition(this.Topic, this.Partition);

        }
        public string Topic { get; }
        public int Partition { get; }
        public long Offset { get; }
        public XXXTopicPartition TopicPartition { get; }
    }
}