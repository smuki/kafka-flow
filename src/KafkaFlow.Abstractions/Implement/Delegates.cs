namespace KafkaFlow
{
    using System.Threading.Tasks;
    using Volte.Data.VolteDi;

    /// <summary>
    /// The delegate used to call the next middleware
    /// </summary>
    /// <param name="context">The message context to be passed to the next middleware</param>
    public delegate Task MiddlewareDelegate(IMessageContext context);

    /// <summary>
    /// Defines a factory to create an instance of <typeparamref name="T" /> type 
    /// </summary>
    /// <param name="resolver">A <see cref="IVolteServiceResolver"/> instance</param>
    public delegate T Factory<out T>(IVolteServiceResolver resolver);
}
