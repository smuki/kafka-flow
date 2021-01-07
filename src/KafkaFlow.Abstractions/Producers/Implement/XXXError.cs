using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow
{
    public class XXXError
    {
        public bool IsError { get; set; }
       
        public string Reason { get; set; }
      
        public bool IsFatal { get; set; }
       
        public string Code { get; set; }
       
        public bool IsBrokerError { get; set; }
       
        public bool IsLocalError { get; set; }
    }
}
