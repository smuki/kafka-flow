//namespace KafkaFlow.Configuration
//{
//    using System;
//    using System.Collections.Generic;
//    using System.ComponentModel;
//    using System.Linq;
//    using Confluent.Kafka;
//    using KafkaFlow.Dependency;
//    using KafkaFlow.Consumers.DistributionStrategies;

//    public sealed class ConsumerConfigurationBuilder : IConsumerConfigurationBuilder
//    {
//        private readonly List<string> topics = new List<string>();
//        private readonly List<Action<string>> statisticsHandlers = new List<Action<string>>();
//        //private readonly IConsumerMiddlewareConfigurationBuilder middlewareConfigurationBuilder;
//        private readonly Dictionary<string,string> _dict = new Dictionary<string,string>();

//        private ConsumerConfig consumerConfig;

//        private string name;
//        private string groupId;
//        private AutoOffsetReset? autoOffsetReset;
//        private int? maxPollIntervalMs;
//        private int workersCount;
//        private int bufferSize;
//        private bool autoStoreOffsets = true;
//        private int statisticsInterval;

//        private Factory<IDistributionStrategy> distributionStrategyFactory = resolver => new BytesSumDistributionStrategy();
//        private TimeSpan autoCommitInterval = TimeSpan.FromSeconds(5);

//        public IDependencyConfigurator DependencyConfigurator { get; }

//        public ConsumerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
//        {
//            this.DependencyConfigurator = dependencyConfigurator;
//        }

//        public ConsumerConfiguration Build(ClusterConfiguration clusterConfiguration)
//        {
//            this.consumerConfig ??= new ConsumerConfig();
//            this.consumerConfig.BootstrapServers ??= string.Join(",", clusterConfiguration.Brokers);
//            this.consumerConfig.GroupId ??= this.groupId;
//            this.consumerConfig.AutoOffsetReset ??= this.autoOffsetReset;
//            this.consumerConfig.MaxPollIntervalMs ??= this.maxPollIntervalMs;
//            this.consumerConfig.StatisticsIntervalMs ??= this.statisticsInterval;

//            this.consumerConfig.EnableAutoOffsetStore = false;
//            this.consumerConfig.EnableAutoCommit = false;

//            this.consumerConfig.ReadSecurityInformation(clusterConfiguration);

//            return new ConsumerConfiguration(
//                this.consumerConfig,
//                this.topics,
//                this.name,
//                this.workersCount,
//                this.bufferSize,
//                this.distributionStrategyFactory,
//                this.autoStoreOffsets,
//                this.autoCommitInterval,
//                this.statisticsHandlers);
//        }
//    }
//}
