namespace KafkaFlow.Consumers
{
    using System;
    //using Confluent.Kafka;

    public readonly struct OffsetsWatermark : IOffsetsWatermark, IEquatable<OffsetsWatermark>
    {
        private readonly long _High;
        private readonly long _Low;
        //private readonly WatermarkOffsets watermark;

        public OffsetsWatermark(long high ,long low)
        {
            _High = high;
            _Low = low;
        }

        public long High => _High;

        public long Low => _Low;

        public bool Equals(OffsetsWatermark other)
        {
            return High== other.High && Low==other.Low;
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetsWatermark other && this.Equals(other);
        }

        //public override int GetHashCode()
        //{
        //    return this.watermark != null ? this.watermark.GetHashCode() : 0;
        //}
    }
}
