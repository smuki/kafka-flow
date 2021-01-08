namespace MessagePipeline.UnitTests.Serializers
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using MessagePipeline.Serializer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SerializerProducerMiddlewareTests
    {
        private Mock<IMessageContext> contextMock;
        private Mock<IMessageSerializer> serializerMock;
        private Mock<IMessageTypeResolver> typeResolverMock;

        private bool nextCalled;

        private SerializerProducerMiddleware target;

        [TestInitialize]
        public void Setup()
        {
            this.contextMock = new Mock<IMessageContext>();
            this.serializerMock = new Mock<IMessageSerializer>();
            this.typeResolverMock = new Mock<IMessageTypeResolver>();

            this.target = new SerializerProducerMiddleware(
                this.serializerMock.Object,
                this.typeResolverMock.Object);
        }

        [TestMethod]
        public async Task Invoke_ValidMessage_CallNext()
        {
            // Arrange
            var rawMessage = new byte[1];
            var deserializedMessage = new TestMessage();

            this.contextMock
                .SetupGet(x => x.Message)
                .Returns(deserializedMessage);

            this.typeResolverMock.Setup(x => x.OnProduce(this.contextMock.Object));
            
            this.serializerMock
                .Setup(x => x.Serialize(deserializedMessage))
                .Returns(rawMessage);

            this.contextMock.Setup(x => x.TransformMessage(rawMessage));
            
            // Act
            await this.target.Invoke(this.contextMock.Object, c => this.SetNextCalled());

            // Assert
            this.nextCalled.Should().BeTrue();
            this.contextMock.VerifyAll();
            this.serializerMock.VerifyAll();
            this.typeResolverMock.VerifyAll();
        }

        private Task SetNextCalled()
        {
            this.nextCalled = true;
            return Task.CompletedTask;
        }

        private class TestMessage
        {
        }
    }
}
