namespace MessagePipeline.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OffsetManager : IOffsetManager, IDisposable
    {
        private readonly IOffsetCommitter committer;
        private readonly Dictionary<(string, int), PartitionOffsets> partitionsOffsets;

        public OffsetManager(
            IOffsetCommitter committer,
            IEnumerable<XXXTopicPartition> partitions)
        {
            this.committer = committer;
            this.partitionsOffsets = partitions.ToDictionary(
                partition => (partition.Topic, partition.Partition),
                partition => new PartitionOffsets());
        }

        public void StoreOffset(XXXTopicPartitionOffset offset)
        {
            if (!this.partitionsOffsets.TryGetValue((offset.Topic, offset.Partition), out var offsets))
            {
                return;
            }

            lock (offsets)
            {
                if (offsets.ShouldUpdateOffset(offset.Offset))
                {
                    this.committer.StoreOffset(new XXXTopicPartitionOffset(offset.Topic, offset.Partition, offsets.LastOffset + 1));
                }
            }
        }

        public void AddOffset(XXXTopicPartitionOffset offset)
        {
            if (this.partitionsOffsets.TryGetValue((offset.Topic, offset.Partition), out var offsets))
            {
                offsets.AddOffset(offset.Offset);
            }
        }

        public void Dispose()
        {
            this.committer.Dispose();
        }
    }
}
