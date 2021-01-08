namespace MessagePipeline.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using Consumers;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class OffsetManagerTests
    {
        private Mock<IOffsetCommitter> committerMock;
        private XXXTopicPartition topicPartition;
        private OffsetManager target;

        [TestInitialize]
        public void Setup()
        {
            this.committerMock = new Mock<IOffsetCommitter>();
            this.topicPartition = new XXXTopicPartition("topic-A", 1);

            this.target = new OffsetManager(
                this.committerMock.Object,
                new List<XXXTopicPartition> { this.topicPartition });
        }
        
        [TestMethod]
        public void StoreOffset_WithInvalidTopicPartition_ShouldDoNothing()
        {
            // Arrange
            this.target.AddOffset(new XXXTopicPartitionOffset(this.topicPartition, new Offset(1)));
            
            // Act
            this.target.StoreOffset(new XXXTopicPartitionOffset(new XXXTopicPartition("topic-B", 1), 1));
            
            // Assert
            this.committerMock.Verify(c => c.StoreOffset(It.IsAny<XXXTopicPartitionOffset>()), Times.Never());
        }
        
        [TestMethod]
        public void StoreOffset_WithGaps_ShouldStoreOffsetJustOnce()
        {
            // Arrange
            this.target.AddOffset(new XXXTopicPartitionOffset(this.topicPartition, 1));
            this.target.AddOffset(new XXXTopicPartitionOffset(this.topicPartition, 2));
            this.target.AddOffset(new XXXTopicPartitionOffset(this.topicPartition, 3));
            
            // Act
            this.target.StoreOffset(new XXXTopicPartitionOffset(this.topicPartition, 3));
            this.target.StoreOffset(new XXXTopicPartitionOffset(this.topicPartition, 2));
            this.target.StoreOffset(new XXXTopicPartitionOffset(this.topicPartition, 1));
            
            // Assert
            this.committerMock.Verify(c => 
                c.StoreOffset(It.Is<XXXTopicPartitionOffset>(p => 
                    p.Partition.Equals(this.topicPartition.Partition) &&
                    p.Offset.Equals(4))), 
                Times.Once);
        }
    }
}
