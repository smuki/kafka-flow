namespace KafkaFlow.Sample
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;

    public class PrintConsoleHandler : IMessageHandler<TestMessage>
    {
        static long total = 0;
        public Task Handle(IMessageContext context, TestMessage message)
        {
            if (total % 30 == 0)
            {
                Console.WriteLine("\n#Total ...= " + total);
            }
            Console.Write(".");

            total++;
        
            return Task.CompletedTask;
        }
    }
}
