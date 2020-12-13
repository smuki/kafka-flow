namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    public class OffsetCommitter : IOffsetCommitter
    {
        private readonly ILogHandler logHandler;

        private ConcurrentDictionary<(string, int), XXXTopicPartitionOffset> offsetsToCommit =
            new ConcurrentDictionary<(string, int), XXXTopicPartitionOffset>();

        private readonly Timer commitTimer;
        IConsumerClient consumerClient;

        public OffsetCommitter(
            IConsumerClient consumerClient,
            TimeSpan autoCommitInterval,
            ILogHandler logHandler)
        {
            Console.WriteLine("autoCommitInterval   =" + autoCommitInterval.TotalSeconds);

            this.logHandler = logHandler;
            this.consumerClient = consumerClient;
            this.commitTimer = new Timer( _ => this.CommitHandler(), null, autoCommitInterval, autoCommitInterval);
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
                this.consumerClient.Commit(offsets.Values);
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
