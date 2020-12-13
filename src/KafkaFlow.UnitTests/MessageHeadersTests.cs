namespace KafkaFlow.UnitTests
{
    using System.Text;
    using Confluent.Kafka;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using KafkaFlow.Dependency;

    [TestClass]
    public class MessageHeadersTests
    {
        private const string key = "abc";
        private const string strValue = "123";
        private readonly byte[] value = Encoding.UTF8.GetBytes("123");

        [TestMethod]
        public void Add_WithKeyNotNull_ShouldAddValueCorrectly()
        {
            // Arrange
            var header = new MessageHeaders();

            // Act
            header.Add(key, this.value);

            // Assert
            header[key].Should().BeEquivalentTo(this.value);
        }

        [TestMethod]
        public void GetKafkaHeader_ShouldReturnKafkaHeaders()
        {
            // Arrange
            var kafkaHeaders = new Headers { { key, this.value } };
            var messageHeaders = new MessageHeaders();
            messageHeaders.Add(key, this.value);

            var result = new Confluent.Kafka.Headers();

            foreach (var header in messageHeaders)
            {
                result.Add(header.Value != null
                    ? new Header(header.Key, header.Value)
                    : new Header(header.Key, null));
            }
            // Assert
            result.Should().BeEquivalentTo(kafkaHeaders);
        }

        [TestMethod]
        public void SetString_WithValueNotNull_ShouldAddValueCorrectly()
        {
            // Arrange
            var header = new MessageHeaders();

            // Act
            header.SetString(key, strValue);

            // Assert
            header.GetString(key).Should().BeEquivalentTo(strValue);
        }
    }
}
