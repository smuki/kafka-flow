


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using KafkaFlow.Configuration;
using Microsoft.Extensions.Options;

namespace KafkaFlow.Consumers
{
    internal sealed class KafkaConsumerClient : IConsumerClient
    {
        private static readonly SemaphoreSlim ConnectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private readonly ConsumerConfiguration configuration;

        //private readonly string _groupId;
        //private readonly KafkaOptions _kafkaOptions;
        private IConsumer<string, byte[]> _consumerClient;
        private readonly ConsumerBuilder<byte[], byte[]> consumerBuilder;
        private readonly ILogHandler logHandler;

        public KafkaConsumerClient(
                        ConsumerConfiguration configuration,
                        ILogHandler logHandler
        )
        {
            //_groupId = groupId;
            //_kafkaOptions = options.Value ?? throw new ArgumentNullException(nameof(options));

            this.logHandler = logHandler;
            this.configuration = configuration;
            var kafkaConfig = configuration.GetKafkaConfig();
            this.consumerBuilder = new ConsumerBuilder<byte[], byte[]>(kafkaConfig);

            this.consumerBuilder
                .SetPartitionsAssignedHandler((consumer, partitions) =>
                {
                    this.OnPartitionAssigned(this, Util.TopicPartition(partitions));
                })

                .SetPartitionsRevokedHandler((consumer, partitions) =>
                {
                    this.OnPartitionRevoked(this, Util.TopicPartitionOffset(partitions));
                }
                )
                .SetErrorHandler((p, error) =>
                {
                    if (error.IsFatal)
                    {
                        this.logHandler.Error("Kafka Consumer Fatal Error", null, new { Error = error });
                    }
                    else
                    {
                        this.logHandler.Warning("Kafka Consumer Error", new { Error = error });
                    }
                })
                .SetStatisticsHandler((consumer, statistics) =>
                {
                    foreach (var handler in configuration.StatisticsHandlers)
                    {
                        handler.Invoke(statistics);
                    }
                });
        }
        public string BrokerAddress { get; }

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            Connect();

            _consumerClient.Subscribe(topics);
        }
        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            Connect();
        }
        public void OnPartitionRevoked(IConsumerClient consumer, IReadOnlyCollection<XXXTopicPartitionOffset> topicPartitions)
        {

        }
        public void OnPartitionAssigned(IConsumerClient consumer, IReadOnlyCollection<XXXTopicPartition> partitions)
        {

        }

        public IntermediateMessage Pull(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var consumerResult = _consumerClient.Consume(cancellationToken);

            if (consumerResult.IsPartitionEOF || consumerResult.Message.Value == null)
            {
                return null;
            }
            var headers = new MessageHeaders();
            foreach (var header in consumerResult.Message.Headers)
            {
                headers.Add(header.Key, header.GetValueBytes());
            }
            return new IntermediateMessage(headers, consumerResult.Message.Value);
        }

        public void Commit(object sender)
        {
            _consumerClient.Commit((ConsumeResult<string, byte[]>)sender);
        }

        public void Reject(object sender)
        {
            _consumerClient.Assign(_consumerClient.Assignment);
        }

        public void Dispose()
        {
            _consumerClient?.Dispose();
        }

        public void Connect()
        {
            if (_consumerClient != null)
            {
                return;
            }

            ConnectionLock.Wait();

            try
            {
                if (_consumerClient == null)
                {
                    //_kafkaOptions.MainConfig["group.id"] = _groupId;
                    //_kafkaOptions.MainConfig["auto.offset.reset"] = "earliest";
                    //var config = _kafkaOptions.AsKafkaConfig();

                    //_consumerClient = new ConsumerBuilder<string, byte[]>(config)
                        //.SetErrorHandler(ConsumerClient_OnConsumeError)
                  //      .Build();
                }
            }
            finally
            {
                ConnectionLock.Release();
            }
        }
    }
}