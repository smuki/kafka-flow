using Confluent.Kafka;
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

        public static IEnumerable<XXXTopicPartitionTimestamp> TopicPartitionTimestamp(IEnumerable<TopicPartitionTimestamp> tp)
        {
            List<XXXTopicPartitionTimestamp> xxx = new List<XXXTopicPartitionTimestamp>();
            foreach (TopicPartitionTimestamp item in tp)
            {
                xxx.Add(new XXXTopicPartitionTimestamp(Util.TopicPartition(item.TopicPartition), item.Timestamp.UnixTimestampMs));
            }
            return xxx;
        }
        public static IEnumerable<TopicPartitionTimestamp> TopicPartitionTimestamp(IEnumerable<XXXTopicPartitionTimestamp> tp)
        {
            List<TopicPartitionTimestamp> xxx = new List<TopicPartitionTimestamp>();
            foreach (XXXTopicPartitionTimestamp item in tp)
            {
                xxx.Add(new TopicPartitionTimestamp(Util.TopicPartition(item.TopicPartition), new Timestamp(item.Timestamp, TimestampType.CreateTime)));
            }
            return xxx;
        }

        public static IEnumerable<XXXTopicPartition> TopicPartition(IEnumerable<TopicPartition> tp)
        {
            List<XXXTopicPartition> xxx = new List<XXXTopicPartition>();
            foreach(TopicPartition item in tp)
            {
                xxx.Add(new XXXTopicPartition(item.Topic, item.Partition.Value));
            }
            return xxx;
        }

        public static IReadOnlyCollection<TopicPartition> TopicPartition(IEnumerable<XXXTopicPartition> tp)
        {
            List<TopicPartition> xxx = new List<TopicPartition>();
            foreach (var item in tp)
            {
                xxx.Add(new TopicPartition(item.Topic, item.Partition));
            }
            return xxx.AsReadOnly();
        }

        public static IEnumerable<TopicPartitionOffset> TopicPartitionOffset(IEnumerable<XXXTopicPartitionOffset> tp)
        {
            List<TopicPartitionOffset> xxx = new List<TopicPartitionOffset>();
            foreach (XXXTopicPartitionOffset item in tp)
            {
                Console.WriteLine("\nTopic=" + item.Topic + " Partition=" + item.Partition + " Offset=" + item.Offset);
                xxx.Add(new TopicPartitionOffset(item.Topic, new Partition(item.Partition), new Offset(item.Offset)));
            }
            return xxx;
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
        public static IEnumerable<XXXTopicPartitionOffset> TopicPartitionOffset(IEnumerable<TopicPartitionOffset> tp)
        {
            List<XXXTopicPartitionOffset> xxx = new List<XXXTopicPartitionOffset>();
            foreach (TopicPartitionOffset item in tp)
            {
                Console.WriteLine("\nTopic=" + item.Topic + " Partition=" + item.Partition + " Offset=" + item.Offset);
                xxx.Add(new XXXTopicPartitionOffset(item.Topic, item.Partition, item.Offset));
            }
            return xxx.AsReadOnly();
        }
    }
}
