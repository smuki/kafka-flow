namespace KafkaFlow.Configuration
{
    using System;

    /// <summary>
    /// </summary>
    public interface IKafkaConfigurationBuilder
    {
        /// <summary>
        /// Adds a new Cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        IKafkaConfigurationBuilder AddCluster(Action<IClusterConfigurationBuilder> cluster);
    }
}
