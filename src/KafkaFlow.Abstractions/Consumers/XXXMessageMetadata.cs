using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Consumers
{
    public class XXXMessageMetadata
    {
        public DateTimeOffset Timestamp { get; set; }
        public IMessageHeaders Headers { get; set; }
    }
}
