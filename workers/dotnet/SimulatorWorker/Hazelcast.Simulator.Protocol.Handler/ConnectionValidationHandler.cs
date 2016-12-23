// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using DotNetty.Transport.Channels;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{
    public class ConnectionValidationHandler : ChannelHandlerAdapter
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectionValidationHandler));

        //        private const int MinimumByteBufferSize = 8;
        private readonly Action<IChannel> setChannel;

        //        private Delegate ChannelInit;

        public ConnectionValidationHandler(Action<IChannel> setChannel)
        {
            this.setChannel = setChannel;
        }

        //TODO Do we really need MAGIC_BYTE Validation as each message is validated later on
        public override void ChannelActive(IChannelHandlerContext context)
        {
            //            if (Logger.IsDebugEnabled)
            //            {
            //            }
            Logger.Info($"Channel create from {context.Channel.RemoteAddress}");
            context.Channel.CloseCompletion.ContinueWith(task => { setChannel(null); });
            setChannel(context.Channel);
        }
    }
}