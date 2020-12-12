using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Consumers
{
    /// <summary>
    /// Intermediate Message
    /// </summary>
    public class IntermediateMessage
    {
        public IntermediateMessage(IDictionary<string, string> headers, byte[] Payload)
        {
            this.Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            this.Payload = Payload;
        }
        public IDictionary<string, string> Headers { get; }
        public string Key { get; set; }
        /// <summary>
        /// Gets the body object of this message
        /// </summary>
        public byte[] Payload { get; }
    }
}
