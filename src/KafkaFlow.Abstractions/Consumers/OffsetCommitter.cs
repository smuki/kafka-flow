namespace MessagePipeline.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Volte.Utils;

    public class OffsetCommitter : IOffsetCommitter
    {
        private ConcurrentDictionary<(string, int), XXXTopicPartitionOffset> offsetsToCommit =
            new ConcurrentDictionary<(string, int), XXXTopicPartitionOffset>();

        private readonly Timer commitTimer;
        private readonly IConsumerClient consumerClient;

        public OffsetCommitter(
            IConsumerClient consumerClient,
            TimeSpan autoCommitInterval)
        {
            NLogger.Info($"AutoCommitInterval {autoCommitInterval.TotalSeconds} second(s)");

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
                NLogger.Info($"#Commiting Offsets#");
                this.consumerClient.Commit(offsets.Values);
            }
            catch (Exception e)
            {
                NLogger.Error("Error Commiting Offsets", e, null);
            }
        }

        public void Dispose()
        {
            this.commitTimer.Dispose();
            this.CommitHandler();
        }
        public void StoreOffset(XXXTopicPartitionOffset tpo)
        {
            this.offsetsToCommit.AddOrUpdate((tpo.Topic, tpo.Partition), tpo, (k, v) => tpo);
        }
    }
}
