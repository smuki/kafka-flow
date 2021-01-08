﻿namespace MessagePipeline.Compressor
{
    using System;
    using System.Threading.Tasks;
    using Volte.Data.VolteDi;

    /// <summary>
    /// Middleware to decompress the messages when consuming
    /// </summary>
    [Middleware(MiddlewareType=MiddlewareType.Consumer)]
    public class CompressorConsumerMiddleware : IMessageMiddleware
    {
        private readonly IMessageCompressor compressor;

        /// <summary>
        /// Creates a <see cref="CompressorConsumerMiddleware"/> instance
        /// </summary>
        /// <param name="compressor">Instance of <see cref="IMessageCompressor"/></param>
        public CompressorConsumerMiddleware(IMessageCompressor compressor)
        {
            this.compressor = compressor;
        }

        /// <summary>
        /// Decompress a message based on the passed message compressor
        /// </summary>
        /// <param name="context">Instance of <see cref="IMessageContext"/></param>
        /// <param name="next">Next middleware to be executed</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throw if message is not byte[]</exception>
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException($"{nameof(context.Message)} must be a byte array to be decompressed and it is '{context.Message.GetType().FullName}'");
            }

            var data = this.compressor.Decompress(rawData);
            context.TransformMessage(data);

            return next(context);
        }
    }
}
