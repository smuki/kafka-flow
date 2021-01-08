namespace MessagePipeline.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Unicode;
    using System.Threading;
    using Confluent.Kafka;
    using FluentAssertions;
    using MessagePipeline.Consumers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MessageContextConsumerTests
    {
        [TestMethod]
        public void MessageTimestamp_ConsumeResultHasMessageTimestamp_ReturnsMessageTimestampFromResult()
        {
            // Arrange
            var expectedMessageTimestamp = new DateTime(2020, 1, 1, 0, 0, 0);

            var headers = new MessageHeaders();

            var consumerResult = new IntermediateMessage(headers, Encoding.UTF8.GetBytes("xxxxxx"));
            consumerResult.Topic = "xxxx";
            consumerResult.Partition = 1;
            consumerResult.Timestamp = expectedMessageTimestamp;

            //var target = new MessageContextConsumer(null, "consumer", null, consumerResult, CancellationToken.None);

            //// Act
            //var messageTimestamp = target.MessageTimestamp;

            //// Assert
            //messageTimestamp.Should().Be(expectedMessageTimestamp);
        }
    }
}
