using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace DotNetty.Codec
{
    public abstract class MessageToByteEncoder<T> : ChannelHandlerAdapter
    {
        /// <summary>
        ///     Returns {@code true} if the given message should be handled. If {@code false} it will be passed to the next
        ///     {@link ChannelHandler} in the {@link ChannelPipeline}.
        /// </summary>
        public bool AcceptOutboundMessage(object msg) => msg is T;

        public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            IByteBuffer buffer= null;
            try
            {
                if (this.AcceptOutboundMessage(msg))
                {
                    var cast = (T) msg;
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

        protected abstract void Encode(IChannelHandlerContext ctx, T message, IByteBuffer buf);

    }
}