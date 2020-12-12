﻿using Confluent.Kafka;
using KafkaFlow.Consumers;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow
{
    public class Util
    {
        public static TopicPartitionOffset TopicPartitionOffset(XXXTopicPartitionOffset tpo)
        {
            return new TopicPartitionOffset(tpo.Topic,new Partition(tpo.Partition),new Offset(tpo.Offset));
        }
        public static XXXTopicPartitionOffset TopicPartitionOffset(TopicPartitionOffset tpo)
        {
            return new XXXTopicPartitionOffset(tpo.Topic, tpo.Partition.Value, tpo.Offset.Value);
        }

        public static XXXTopicPartition TopicPartition(TopicPartition tp)
        {
            return new XXXTopicPartition(tp.Topic, tp.Partition.Value);
        }
        public static TopicPartition TopicPartition(XXXTopicPartition tp)
        {
            return new TopicPartition(tp.Topic, tp.Partition);
        }
        public static IReadOnlyCollection<XXXTopicPartition> TopicPartition(IReadOnlyCollection<TopicPartition> tp)
        {
            List<XXXTopicPartition> xxx = new List<XXXTopicPartition>();
            foreach(TopicPartition item in tp)
            {
                xxx.Add(new XXXTopicPartition(item.Topic, item.Partition.Value));
            }
            return xxx.AsReadOnly();
        }
        public static ICollection<TopicPartitionOffset> TopicPartitionOffset(ICollection<XXXTopicPartitionOffset> tp)
        {
            List<TopicPartitionOffset> xxx = new List<TopicPartitionOffset>();
            foreach (XXXTopicPartitionOffset item in tp)
            {
                Console.WriteLine("\nTopic=" + item.Topic+ " Partition="+item.Partition + " Offset=" + item.Offset);
                xxx.Add(new TopicPartitionOffset(item.Topic,new Partition(item.Partition),new Offset(item.Offset)));
            }
            return xxx.AsReadOnly();
        }
    }
}
