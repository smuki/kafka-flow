using System;

namespace MessagePipeline.Consumers
{
    public class XXXMessageMetadata
    {
        public DateTimeOffset Timestamp { get; set; }
        public IMessageHeaders Headers { get; set; }
    }
}
