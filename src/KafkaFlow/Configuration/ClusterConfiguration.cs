namespace KafkaFlow.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ClusterConfiguration
    {
        private readonly Func<SecurityInformation> securityInformationHandler;

        public ClusterConfiguration(
            Func<SecurityInformation> securityInformationHandler)
        {
            this.securityInformationHandler = securityInformationHandler;
        }

        public SecurityInformation GetSecurityInformation() => this.securityInformationHandler?.Invoke();
    }
}
