using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow
{
    //
    // 摘要:
    //     Represents an error that occured whilst producing a message.
    public class XXXProduceException : Exception
    {

        public XXXProduceException(XXXError error)
        {

        }

        public XXXProduceException(XXXError error, XXXDeliveryResult deliveryResult)
        {
            this.DeliveryResult = deliveryResult;

        }
      
        public XXXProduceException(XXXError error, XXXDeliveryResult deliveryResult, Exception innerException)
        {

        }

        //
        // 摘要:
        //     The delivery result associated with the produce request.
        public XXXDeliveryResult DeliveryResult { get; }
    }
}
