namespace MessagePipeline.Sample
{
    using System;
    using System.Threading.Tasks;
    using MessagePipeline.TypedHandler;
    using Volte.Data.VolteDi;
    using Volte.Utils;

    [Injection(InjectionType = InjectionType.Auto)]
    public class PrintConsoleHandler1 : IMessageHandler<TestMessage>
    {
        static long total = 0;
        public Task Handle(IMessageContext context, TestMessage message)
        {
            NLogger.Info($"GroupId {context.GroupId}");
            total++;
            NLogger.Info($"A1 {total} WorkerId:{context.WorkerId} #Partition : {context.Partition} | Offset: {context.Offset} | {message.Text}");
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
            NLogger.Info($"GroupId {context.GroupId}");
            NLogger.Info($"A2 {total} WorkerId:{context.WorkerId} #Partition : {context.Partition} | Offset: {context.Offset} | {message.Text}");
            return Task.CompletedTask;
        }
    }
}
