namespace KafkaFlow.Sample
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.TypedHandler;
    using Volte.Data.VolteDi;
    using Volte.Utils;

    [Injection(InjectionType = InjectionType.Auto)]
    public class PrintConsoleHandler1 : IMessageHandler<TestMessage>
    {
        static long total = 0;
        public Task Handle(IMessageContext context, TestMessage message)
        {
            total++;
            NLogger.Info($"A1 {total} #Partition : {context.Partition} | Offset: {context.Offset} | {message.Text}");
            return Task.CompletedTask;
        }
    }
    [Injection(InjectionType = InjectionType.Auto)]
    public class PrintConsoleHandler2 : IMessageHandler<TestMessage>
    {
        static long total = 0;
        public Task Handle(IMessageContext context, TestMessage message)
        {
            total++;
            NLogger.Info($"A2 {total} #Partition : {context.Partition} | Offset: {context.Offset} | {message.Text}");
            return Task.CompletedTask;
        }
    }
}
