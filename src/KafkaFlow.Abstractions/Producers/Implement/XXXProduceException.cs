using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePipeline
{
    public class XXXProduceException : Exception
    {
        public XXXProduceException(XXXError error)
        {
            this.Error = error;
        }
        public XXXProduceException(XXXError error, XXXDeliveryResult deliveryResult)
        {
            this.DeliveryResult = deliveryResult;
            this.Error = error;
        }
        public XXXProduceException(XXXError error, XXXDeliveryResult deliveryResult, Exception innerException)
        {
            this.DeliveryResult = deliveryResult;
            this.Error = error;
        }
        public XXXError Error { get; set; }
        public XXXDeliveryResult DeliveryResult { get; set; }
    }
}
