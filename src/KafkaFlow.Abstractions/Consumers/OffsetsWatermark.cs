namespace KafkaFlow.Consumers
{
    using System;
    //using Confluent.Kafka;

    public readonly struct OffsetsWatermark : IOffsetsWatermark, IEquatable<OffsetsWatermark>
    {
        private readonly long _High;
        private readonly long _Low;

        public OffsetsWatermark(long high ,long low)
        {
            _High = high;
            _Low = low;
        }

        public long High
        {
            get
            {
                return _High;
            }
        }

        public long Low
        {
            get
            {
                return _Low;
            }
        }

        public bool Equals(OffsetsWatermark other)
        {
            return High== other.High && Low==other.Low;
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetsWatermark other && this.Equals(other);
        }
    }
}
