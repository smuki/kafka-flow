using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.TypedHandler
{
    /// <summary>
    /// Mark the class for dependency injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HandlerAttribute : Attribute
    {
        /// <summary>
        /// Sets if the service will be injected by interface or implementation. Default = Auto
        /// </summary>
        public int Priority { get; set; } = 100;
    }
}
