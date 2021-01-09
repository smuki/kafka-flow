using System;
using System.Collections.Generic;
using System.Text;

namespace Volte.Data.VolteDi
{
    public enum MiddlewareType
    {
        Producer,
        Consumer
    }
    /// <summary>
    /// Mark the class for dependency injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MiddlewareAttribute : Attribute
    {
        /// <summary>
        /// Sets if the service will be injected by interface or implementation. Default = Auto
        /// </summary>
        public MiddlewareType MiddlewareType { get; set; } = MiddlewareType.Consumer;
        public int Priority { get; set; } = 100;
    }
}
