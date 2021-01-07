namespace KafkaFlow.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using global::Microsoft.Extensions.DependencyInjection;
    using KafkaFlow.Admin;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.Consumers;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;
    using Volte.Data.VolteDi;
    using Volte.Utils;

    internal static class Program
    {
        private static async Task xxxMain()
        {
            var services = new ServiceCollection();

            const string producerName = "PrintConsole";

            const string consumerName = "test";

            //services.AddKafka(
            //    kafka => kafka
            //        .AddCluster(
            //            cluster => cluster
            //                .EnableAdminMessages("kafka-flow.admin", Guid.NewGuid().ToString())
            //        )
            //);

            services.UseKafkaFlow();

            VolteDiOptions _opt = new VolteDiOptions();

            IList<Assembly> _Assembly = AssemblyLoader.LoadAssembly(_opt).ToList();
            StringBuilder UseAssembly = new StringBuilder();
            UseAssembly.AppendLine("\nUse Assembly:");
            foreach (var v in _Assembly)
            {
                if (string.IsNullOrEmpty(v.Location))
                {
                    UseAssembly.AppendLine("  Assembly ---- " + v.GetAssemblyName());
                }
                else
                {
                    UseAssembly.AppendLine("  Assembly File " + v.Location);
                }
            }
            NLogger.Debug(UseAssembly);

            Console.WriteLine(UseAssembly.ToString());

            StringBuilder ScanClass = new StringBuilder();
            ScanClass.AppendLine();
            services.LoadInjection(_Assembly.ToArray<Assembly>());


            foreach (var x in services.ToList<ServiceDescriptor>())
            {
                Console.WriteLine(x.ServiceType);

                if (x.ServiceType != null && x.ServiceType.IsGenericType)
                {
                      Console.WriteLine(x.ServiceType);
                }
            }

            var provider = services.BuildServiceProvider();

            //var bus = provider.CreateKafkaBus();

            //await bus.StartAsync();

            var consumers = provider.GetRequiredService<IConsumerAccessor>();
            var producers = provider.GetRequiredService<IProducerAccessor>();

            var adminProducer = provider.GetService<IAdminProducer>();

            while (true)
            {
                Console.Write("resume\npause\nreset\nrewind\nworkers\nexit\nNumber of messages to produce:");
                Console.WriteLine("");
                var input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case var _ when int.TryParse(input, out var count):
                        for (var i = 0; i < count; i++)
                        {
                            var msg = new TestMessage { Text = $"Message: {Guid.NewGuid()}" };
                            producers[producerName].Produce(Guid.NewGuid().ToString(), msg);
                        }
                        break;

                    case "pause":
                        foreach (var consumer in consumers.Consumers)
                        {
                            consumer.Pause(consumer.Assignment);
                        }
                        Console.WriteLine("Consumer paused");

                        break;

                    case "resume":
                        foreach (var consumer in consumers.Consumers)
                        {
                            consumer.Resume(consumer.Assignment);
                        }
                        Console.WriteLine("Consumer resumed");

                        break;

                    case "reset":
                        await adminProducer.ProduceAsync(new ResetConsumerOffset { ConsumerName = consumerName });

                        break;

                    case "rewind":
                        Console.Write("Input a time: ");
                        var timeInput = Console.ReadLine();

                        if (DateTime.TryParse(timeInput, out var time))
                        {
                            adminProducer.ProduceAsync(
                                new RewindConsumerOffsetToDateTime
                                {
                                    ConsumerName = consumerName,
                                    DateTime = time
                                });
                        }

                        break;

                    case "workers":
                        Console.Write("Input a new worker count: ");
                        var workersInput = Console.ReadLine();

                        if (int.TryParse(workersInput, out var workers))
                        {
                            await adminProducer.ProduceAsync(
                                new ChangeConsumerWorkerCount
                                {
                                    ConsumerName = consumerName,
                                    WorkerCount = workers
                                });
                        }

                        break;

                    case "exit":
                        //await bus.StopAsync();
                        return;
                }
            }
        }
    }
}
