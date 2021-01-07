//namespace KafkaFlow
//{
//    using System;
//    using System.Reflection;
//    using KafkaFlow.Admin;
//    using KafkaFlow.Admin.Handlers;
//    using KafkaFlow.Configuration;
//    using KafkaFlow.Serializer;
//    using KafkaFlow.Serializer.ProtoBuf;
//    using KafkaFlow.TypedHandler;
//    using KafkaFlow.Dependency;

//    public static class ClusterConfigurationBuilderExtensions
//    {
//        public static IClusterConfigurationBuilder EnableAdminMessages(
//            this IClusterConfigurationBuilder cluster,
//            string adminTopic,
//            string adminConsumerGroup)
//        {
//            cluster.DependencyConfigurator.AddSingleton<IAdminProducer, AdminProducer>();

//            return cluster;
//        }

//        public static IClusterConfigurationBuilder EnableAdminMessages(
//            this IClusterConfigurationBuilder cluster,
//            string adminTopic)
//        {
//            return cluster.EnableAdminMessages(adminTopic, $"Admin-{Assembly.GetEntryAssembly().GetName().Name}");
//        }
//    }
//}
