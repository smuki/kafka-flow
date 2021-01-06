namespace KafkaFlow.Producers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using KafkaFlow.Configuration;
    using KafkaFlow.Middleware;
    using KafkaFlow.Dependency;
    using Volte.Utils;
    using Volte.Data.VolteDi;
    using System.Reflection;
    using System.Collections.Generic;

    [Injection(InjectionType = InjectionType.Auto)]
    public class MessageProducer : IMessageProducer, IDisposable
    {
        private MessageProducerSettting configuration;
        private IMiddlewareExecutor middlewareExecutor;
        private IDependencyResolverScope dependencyResolverScope;

        private volatile IProducer<byte[], byte[]> producer;
        private readonly object producerCreationSync = new object();


        public MessageProducer(IDependencyResolver dependencyResolver)
        {
            // Create middlewares instances inside a scope to allow scoped injections in producer middlewares

            this.dependencyResolverScope = dependencyResolver.CreateScope();

            this.middlewareExecutor = this.dependencyResolverScope.Resolver.Resolve<IMiddlewareExecutor>();

            var middlewares = dependencyResolver.Resolves<IMessageMiddleware>().Where(x =>
            {
                Console.WriteLine(x.ToString());

                var injectionAttribute = x.GetType().GetCustomAttribute<MiddlewareAttribute>();
                if (injectionAttribute != null)
                {
                    Console.WriteLine(injectionAttribute.MiddlewareType);
                    return injectionAttribute.MiddlewareType == MiddlewareType.Producer;
                }
                return false;
            })
          .ToList();

            middlewares.Sort((x, y) =>
            {
                var injectionAttributex = x.GetType().GetCustomAttribute<MiddlewareAttribute>();
                var injectionAttributey = y.GetType().GetCustomAttribute<MiddlewareAttribute>();
                return injectionAttributex.Priority.CompareTo(injectionAttributey.Priority);
            });

            this.middlewareExecutor.Initialize(middlewares);
        }
        public void Initialize(MessageProducerSettting configuration)
        {
            this.configuration = configuration;
        }

        public string ProducerName => this.configuration.Name;

        public async Task<XXXDeliveryResult> ProduceAsync(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            var messageKey = partitionKey is null ? null : Encoding.UTF8.GetBytes(partitionKey);

            XXXDeliveryResult report = null;

            await this.middlewareExecutor.Execute(new ProducerMessageContext(message, messageKey, headers, topic),
                    async context =>
                    {
                        report = await this.InternalProduceAsync((ProducerMessageContext)context).ConfigureAwait(false);
                    }).ConfigureAwait(false);

            return report;
        }

        public Task<XXXDeliveryResult> ProduceAsync(
            string partitionKey,
            object message,
            IMessageHeaders headers = null)
        {
            if (string.IsNullOrWhiteSpace(this.configuration.Topic))
            {
                throw new InvalidOperationException($"There is no default topic defined for producer {this.ProducerName}");
            }

            return this.ProduceAsync(this.configuration.Topic, partitionKey, message, headers);
        }

        public void Produce(
            string topic,
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<XXXDeliveryReport> deliveryHandler = null)
        {
            var messageKey = partitionKey is null ? null : Encoding.UTF8.GetBytes(partitionKey);

            this.middlewareExecutor.Execute(new ProducerMessageContext(message, messageKey, headers, topic),
                context =>
                {
                    var completionSource = new TaskCompletionSource<byte>();
                    try
                    {

                        this.InternalProduce((ProducerMessageContext)context,
                            report =>
                            {
                                Console.WriteLine(report.Status);

                                if (report.Error.IsError)
                                {
                                    Console.WriteLine("error");
                                //completionSource.SetException(new ProduceException<byte[], byte[]>(report.Error, report));
                                completionSource.SetException(new Exception(report.Error.ToString()));
                                }
                                else
                                {
                                    completionSource.SetResult(0);
                                }

                                deliveryHandler?.Invoke(report);
                            });
                    }catch(Exception ex)
                    {
                        NLogger.Error(ex);
                    }

                    return completionSource.Task;
                });
        }

        public void Produce(
            string partitionKey,
            object message,
            IMessageHeaders headers = null,
            Action<XXXDeliveryReport> deliveryHandler = null)
        {
            if (string.IsNullOrWhiteSpace(this.configuration.Topic))
            {
                throw new InvalidOperationException($"There is no default topic defined for producer {this.ProducerName}");
            }

            this.Produce(this.configuration.Topic, partitionKey, message, headers, deliveryHandler);
        }

        private IProducer<byte[], byte[]> EnsureProducer()
        {
            if (this.producer != null)
            {
                return this.producer;
            }

            lock (this.producerCreationSync)
            {
                if (this.producer != null)
                {
                    return this.producer;
                }
                Dictionary<string, string> Parameter = this.configuration.Parameter;

                ProducerConfig _producerConfig = new ProducerConfig();

                _producerConfig.BootstrapServers = this.configuration.Brokers;
                if (configuration.ContainsKey("Acks"))
                {
                    if (configuration["Acks"] == "Leader")
                    {
                        _producerConfig.Acks = Confluent.Kafka.Acks.Leader;
                    }
                    else if (configuration["Acks"] == "All")
                    {
                        _producerConfig.Acks = Confluent.Kafka.Acks.All;
                    }
                    else if (configuration["Acks"] == "None")
                    {
                        _producerConfig.Acks = Confluent.Kafka.Acks.None;
                    }
                }
                else
                {
                    _producerConfig.Acks = Confluent.Kafka.Acks.Leader;
                }

                return this.producer = new ProducerBuilder<byte[], byte[]>(_producerConfig).SetErrorHandler(
                        (p, error) =>
                        {
                            if (error.IsFatal)
                            {
                                this.InvalidateProducer(error, null);
                            }
                            else
                            {
                                this.dependencyResolverScope.Resolver.Resolve<ILogHandler>().Warning("Kafka Producer Error", new { Error = error });
                            }
                        })
                    .SetStatisticsHandler((producer, statistics) =>
                    {
                        foreach (var handler in this.configuration.StatisticsHandlers)
                        {
                            handler.Invoke(statistics);
                        }
                    }).Build();
            }
        }

        private void InvalidateProducer(Error error, XXXDeliveryResult result)
        {
            lock (this.producerCreationSync)
            {
                this.producer = null;
            }
            XXXError xerror = new XXXError();
            xerror.Reason = error.Reason;
            xerror.IsLocalError = error.IsLocalError;
            xerror.IsBrokerError = error.IsBrokerError;
            xerror.IsError = error.IsError;
            xerror.IsFatal = error.IsFatal;

            this.dependencyResolverScope.Resolver.Resolve<ILogHandler>()
                .Error("Kafka produce fatal error occurred. The producer will be recreated",
                    result is null ? new XXXProduceException(xerror) : new XXXProduceException(xerror, result),
                    new { Error = error });
        }

        private async Task<XXXDeliveryResult> InternalProduceAsync(ProducerMessageContext context)
        {
            XXXDeliveryResult result = null;

            try
            {
                var result2 = await this.EnsureProducer().ProduceAsync(context.Topic, CreateMessage(context)).ConfigureAwait(false);

                result = XXXUtil.XXXDeliveryResult(result2);
            }
            catch (ProduceException<byte[], byte[]> e)
            {
                if (e.Error.IsFatal)
                {
                    this.InvalidateProducer(e.Error, result);
                }

                throw;
            }

            context.Offset = result.Offset;
            context.Partition = result.Partition;

            return result;
        }

        private void InternalProduce(
            ProducerMessageContext context,
            Action<XXXDeliveryReport> deliveryHandler)
        {
            
            NLogger.Info("InternalProduce send....");
            try
            {


                this.EnsureProducer().Produce(context.Topic, CreateMessage(context),
                        report =>
                        {
                            var result = XXXUtil.XXXDeliveryResult(report);

                            if (result.Error.IsFatal
                            || result.Error.IsBrokerError
                            || result.Error.IsLocalError)
                            {
                                NLogger.Info("Error....");
                                NLogger.Info("Error...."+ result.Error.Reason);

                                this.InvalidateProducer(report.Error, result);
                            }

                            NLogger.Info("Offset=" + report.Offset);
                            NLogger.Info("Partition=" + report.Partition);

                            context.Offset = report.Offset;
                            context.Partition = report.Partition;

                            deliveryHandler(result);
                        });
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            NLogger.Info("InternalProduce after send....");

        }

        private static Message<byte[], byte[]> CreateMessage(IMessageContext context)
        {
            var headers = new Confluent.Kafka.Headers();

            foreach (var header in context.Headers)
            {
                headers.Add(header.Value != null ? new Header(header.Key, header.Value) : new Header(header.Key, null));
            }
            return new Message<byte[], byte[]>
            {
                Key = context.PartitionKey,
                Value = GetMessageContent(context),
                Headers = headers,
                Timestamp = Timestamp.Default
            };
        }

        private static byte[] GetMessageContent(IMessageContext context)
        {
            if (!(context.Message is byte[] value))
            {
                throw new InvalidOperationException($"{nameof(context.Message)} must be a byte array to be produced, it is a {context.Message.GetType().FullName}." +
                    "You should serialize or encode your message object using a middleware");
            }

            return value;
        }

        public void Dispose()
        {
            this.dependencyResolverScope.Dispose();
            this.producer?.Dispose();
        }
    }
}
