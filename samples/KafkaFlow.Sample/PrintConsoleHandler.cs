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
            foreach(var xx in context.Headers)
            {
             //   Console.WriteLine(xx);
            }
            Console.Write("#"+ total);
            total++;
            //Console.WriteLine("Partition***: {0} | Offset: {1} | Message: {2}",
            //context.Partition,
            //context.Offset,
            //message.Text);
            return Task.CompletedTask;
        }
    }
}
