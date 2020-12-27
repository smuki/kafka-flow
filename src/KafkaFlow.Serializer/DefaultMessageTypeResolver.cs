namespace KafkaFlow.Serializer
{
    using System;
    using KafkaFlow.Dependency;

    /// <summary>
    /// The default implementation of <see cref="IMessageTypeResolver"/>
    /// </summary>
    public class DefaultMessageTypeResolver : IMessageTypeResolver
    {
        private const string MessageType = "Message-Type";

        /// <summary>
        /// Get the message type when consuming
        /// </summary>
        /// <param name="context">The message context</param>
        /// <returns></returns>
        public Type OnConsume(IMessageContext context)
        {
            var typeName = context.Headers.GetString(MessageType);

            return Type.GetType(typeName);
        }

        /// <summary>
        /// Fills the type metadata when producing
        /// </summary>
        /// <param name="context"></param>
        public void OnProduce(IMessageContext context)
        {
            if (context.Message is null)
            {
                return;
            }

            context.Headers.SetString(MessageType, $"{context.Message.GetType().FullName}, {context.Message.GetType().Assembly.GetName().Name}");
        }
    }
}
