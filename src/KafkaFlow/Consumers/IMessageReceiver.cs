using KafkaFlow.Consumers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KafkaFlow.MQClient
{
    public interface IMessageReceiver
    {
        Task<XXXConsumeResult<byte[],byte[]>> ReceiveAsync();
    }
}
