﻿// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
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

using Hazelcast.Simulator.Utils;

namespace Hazelcast.Simulator.Protocol.Core
{
    public enum FailureType
    {
        //String id, String humanReadable, boolean isTerminal

        [Value("nettyException", "Netty exception", false)]
        NettyException,

        [Value("workerException", "Worker exception", false)]
        WorkerException,

        [Value("workerTimeout", "Worker timeout", false)]
        WorkerTimeout,

        [Value("workerOOME", "Worker OOME", true)]
        WorkerOome,

        [Value("workerAbnormalExit", "Worker abnormal exit", true)]
        WorkerAbnormalExit,

        [Value("workerNormalExit", "Worker normal exit", true)]
        WorkerNormalExit
    }
}