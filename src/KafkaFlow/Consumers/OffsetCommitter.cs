namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Confluent.Kafka;

    internal class OffsetCommitter : IOffsetCommitter
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly ILogHandler logHandler;

        private ConcurrentDictionary<(string, int), XXXTopicPartitionOffset> offsetsToCommit =
            new ConcurrentDictionary<(string, int), XXXTopicPartitionOffset>();

        private readonly Timer commitTimer;

        public OffsetCommitter(
            IConsumer<byte[], byte[]> consumer,
            TimeSpan autoCommitInterval,
            ILogHandler logHandler)
        {
            Console.WriteLine("autoCommitInterval   =" + autoCommitInterval.TotalSeconds);

            this.consumer = consumer;
            this.logHandler = logHandler;
            this.commitTimer = new Timer(
                _ => this.CommitHandler(),
                null,
                autoCommitInterval,
                autoCommitInterval);
        }

        private void CommitHandler()
        {
            if (!this.offsetsToCommit.Any())
            {
                return;
            }

            var offsets = this.offsetsToCommit;
            this.offsetsToCommit = new ConcurrentDictionary<(string, int), XXXTopicPartitionOffset>();

            try
            {
                Console.WriteLine("Commit Count         : " + offsets.Count);
                Console.WriteLine("Offsets Values Count : " + offsets.Values.Count);
                foreach(var x in offsets.Values)
                {
                    Console.WriteLine(x);
                }
                this.consumer.Commit(Util.TopicPartitionOffset(offsets.Values));
                Console.WriteLine("Commit##");

            }
            catch (Exception e)
            {
                this.logHandler.Error(
                    "Error Commiting Offsets",
                    e,
                    null);
            }
        }

        public void Dispose()
        {
            this.commitTimer.Dispose();
            this.CommitHandler();
        }

        public void StoreOffset(XXXTopicPartitionOffset tpo)
        {
            this.offsetsToCommit.AddOrUpdate(
                (tpo.Topic, tpo.Partition),
                tpo,
                (k, v) => tpo);
        }
    }
}
