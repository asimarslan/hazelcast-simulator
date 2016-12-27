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

using System.Collections.Concurrent;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Protocol.Processors
{
    /// <summary>
    ///     A stateful context for executing simulator operations.
    /// </summary>
    public class OperationContext
    {
        public IHazelcastInstance HazelcastInstance { get; }

        public SimulatorAddress WorkerAddress { get; }

        public WorkerConnector Connector { get; }

        public ConcurrentDictionary<int, TestContainer> Tests { get; }

        public OperationContext(IHazelcastInstance hazelcastInstance, SimulatorAddress workerAddress, WorkerConnector connector)
        {
            HazelcastInstance = hazelcastInstance;
            WorkerAddress = workerAddress;
            Connector = connector;
            Tests = new ConcurrentDictionary<int, TestContainer>();
        }
    }
}