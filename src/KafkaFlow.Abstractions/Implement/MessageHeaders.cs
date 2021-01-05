namespace KafkaFlow
{
    using System.Collections;
    using System.Collections.Generic;
    //using Confluent.Kafka;

    /// <summary>
    /// Collection of message headers
    /// </summary>
    public class MessageHeaders : IMessageHeaders
    {
        private readonly Dictionary<string, byte[]> headers;
       
        public MessageHeaders()
        {
            headers=new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Adds a new header to the enumeration
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value (possibly null)</param>
        public void Add(string key, byte[] value)
        {
            this.headers.Add(key, value);
        }

        /// <summary>
        /// Gets the header with specified key
        /// </summary>
        /// <param name="key">The zero-based index of the element to get</param>
        public byte[] this[string key]
        {
            get => this.headers.TryGetValue(key, out var value) ? value : null;
            set
            {
                this.headers.Remove(key);
                this.headers.Add(key, value);
            }
        }
        
        /// <summary>
        /// Gets an enumerator that iterates through <see cref="Headers"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
        {
            foreach (var header in this.headers)
            {
                yield return header;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
