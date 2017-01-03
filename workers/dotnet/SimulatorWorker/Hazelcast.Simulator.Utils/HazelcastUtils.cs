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

using System.Net;
using Hazelcast.Client;
using Hazelcast.Core;

namespace Hazelcast.Simulator.Utils
{
    public class HazelcastUtils
    {
        public static string GetHazelcastAddress(string workerType, string publicAddress, IHazelcastInstance hazelcastInstance)
        {
            if (hazelcastInstance != null)
            {
                IPEndPoint socketAddress = hazelcastInstance.GetLocalEndpoint().GetSocketAddress();
                if (socketAddress != null)
                {
                    return $"{socketAddress.Address}:{socketAddress.Port}";
                }
            }
            return $"client:{publicAddress}";
        }

        public static IHazelcastInstance CreateClientHazelcastInstance(string hzConfigFile)
        {
            return HazelcastClient.NewHazelcastClient(hzConfigFile);
        }
    }
}