namespace DotNetty.Codecs
{
    using System;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Common.Utilities;
    using DotNetty.Transport.Channels;

    public abstract class MessageToByteEncoder<T> : ChannelHandlerAdapter
    {
        /// <summary>
        ///     Returns <c>true</c> if the given message should be handled. If <c>false</c> it will be passed to the next
        ///     <see cref="IChannelHandler"/> in the <see cref="IChannelPipeline"/>.
        /// </summary>
        public bool AcceptOutboundMessage(object msg) => msg is T;

        public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            IByteBuffer buffer = null;
            try
            {
                if (this.AcceptOutboundMessage(msg))
                {
                    var cast = (T)msg;
                    buffer = ctx.Allocator.Buffer();
                    try
                    {
                        this.Encode(ctx, cast, buffer);
                    }
                    finally
                    {
                        ReferenceCountUtil.Release(cast);
                    }

                    if (buffer.IsReadable())
                    {
                        return ctx.WriteAsync(msg);
                    }
                    else
                    {
                        buffer.Release();
                        return ctx.WriteAsync(Unpooled.Empty);
                    }
                }
                else
                {
                    return ctx.WriteAsync(msg);
                }
            }
            catch (EncoderException e)
            {
                return TaskEx.FromException(e);
            }
            catch (Exception ex)
            {
                return TaskEx.FromException(new EncoderException(ex));
            }
            finally
            {
                buffer?.Release();
            }
        }

        /// <summary>
        ///     Encode a message into a <see cref="IByteBuffer"/>. This method will be called for each written message
        ///     that can be handled by this encoder.
        ///</summary>
        ///<param name="context">
        ///     The <see cref="IChannelHandlerContext"/> which this <see cref="MessageToByteEncoder"/> belongs to
        /// </param>
        ///<param name="message">
        ///     the message to encode
        /// </param>
        ///<param name="output">
        ///     the <see cref="IByteBuffer"/> into which the encoded message will be written
        /// </param>
        protected abstract void Encode(IChannelHandlerContext context, T message, IByteBuffer output);
    }
}