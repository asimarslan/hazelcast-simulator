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
using System.Text;

namespace Hazelcast.Simulator.Protocol.Core
{
    /// <summary>
    ///     Address object which(uniquely) identifies one or more Simulator components.
    /// </summary>
    /// <remark>
    ///     Supports wildcards on each { @link AddressLevel} to target all components on that address level.
    ///     For example a { @link SimulatorMessage}
    ///     to<c>C_A2_W* _T1</c> will be sent to <c>C_A2_W1_T1</c> and <c>C_A2_W2_T1</c>.
    ///     <pre>
    ///         +---+
    ///         REMOTE                                        + R +
    ///         +---+
    ///         |
    ///         v
    ///         +---+
    ///         COORDINATOR           +-----------------------+ C +----------------------+
    ///         |                       +---+                      |
    ///         |                                                  |
    ///         v v
    ///         +--+---+                                           +---+--+
    ///         AGENT              | C_A1 |                              +------------+ C_A2 +------------+
    ///         +--+---+                              |            +------+            |
    ///         |                                 |                                 |
    ///         v v                                 v
    ///         +----+----+                       +----+----+                       +----+----+
    ///         WORKER       +---+ C_A1_W1 +---+               +---+ C_A2_W1 +---+               +---+ C_A2_W2 +---+
    ///         |   +---------+   |               |   +---------+   |               |   +---------+   |
    ///         |                 |               |                 |               |                 |
    ///         v v               v v               v v
    ///         +-----+------+   +------+-----+   +-----+------+   +------+-----+   +-----+------+   +------+-----+
    ///         TEST   | C_A1_W1_T1 |   | C_A1_W1_T2 |   | C_A2_W1_T1 |   | C_A2_W1_T2 |   | C_A2_W2_T1 |   | C_A2_W2_T2 |
    ///         +------------+   +------------+   +------------+   +------------+   +------------+   +------------+
    ///     </pre>
    /// </remark>
    public class SimulatorAddress
    {
        public static readonly SimulatorAddress REMOTE = new SimulatorAddress(AddressLevel.REMOTE, 0, 0, 0);
        public static readonly SimulatorAddress COORDINATOR = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
        public static readonly SimulatorAddress ALL_AGENTS = new SimulatorAddress(AddressLevel.AGENT, 0, 0, 0);
        public static readonly SimulatorAddress ALL_WORKERS = new SimulatorAddress(AddressLevel.WORKER, 0, 0, 0);

        private const string REMOTE_STRING = "R";
        private const string COORDINATOR_STRING = "C";

        public SimulatorAddress(AddressLevel addressLevel, int agentIndex, int workerIndex, int testIndex)
        {
            AddressLevel = addressLevel;
            AgentIndex = agentIndex;
            WorkerIndex = workerIndex;
            TestIndex = testIndex;
        }

        public AddressLevel AddressLevel { get; }

        public int AgentIndex { get; }

        public int WorkerIndex { get; }

        public int TestIndex { get; }

        public int GetAddressIndex()
        {
            switch (AddressLevel)
            {
                case AddressLevel.AGENT:
                    return AgentIndex;
                case AddressLevel.WORKER:
                    return WorkerIndex;
                case AddressLevel.TEST:
                    return TestIndex;
                default:
                    throw new ArgumentException($"No address index for {AddressLevel}");
            }
        }

        public SimulatorAddress GetChild(int childIndex)
        {
            if (AddressLevel == AddressLevel.WORKER)
            {
                return new SimulatorAddress(AddressLevel.TEST, AgentIndex, WorkerIndex, childIndex);
            }
            if (AddressLevel == AddressLevel.TEST)
            {
                throw new ArgumentException("Test has no child!");
            }
            throw new ArgumentException("Unsupported address level for ");
        }

        public SimulatorAddress GetParent()
        {
            switch (AddressLevel)
            {
                case AddressLevel.TEST:
                    return new SimulatorAddress(AddressLevel.WORKER, AgentIndex, WorkerIndex, 0);
                case AddressLevel.WORKER:
                    return new SimulatorAddress(AddressLevel.AGENT, AgentIndex, 0, 0);
                case AddressLevel.AGENT:
                    return COORDINATOR;
                default:
                    throw new ArgumentException($"No parent for {AddressLevel}");
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var that = obj as SimulatorAddress;
            if (AgentIndex != that.AgentIndex)
            {
                return false;
            }
            if (WorkerIndex != that.WorkerIndex)
            {
                return false;
            }
            if (TestIndex != that.TestIndex)
            {
                return false;
            }
            return AddressLevel == that.AddressLevel;
        }

        public override int GetHashCode()
        {
            int result = AddressLevel.GetHashCode();
            result = 31 * result + AgentIndex;
            result = 31 * result + WorkerIndex;
            result = 31 * result + TestIndex;
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(AddressLevel.REMOTE == AddressLevel ? REMOTE_STRING : COORDINATOR_STRING);
            AppendAddressLevel(sb, AddressLevel.COORDINATOR, "_A", AgentIndex);
            AppendAddressLevel(sb, AddressLevel.AGENT, "_W", WorkerIndex);
            AppendAddressLevel(sb, AddressLevel.WORKER, "_T", TestIndex);
            return sb.ToString();
        }

        private void AppendAddressLevel(StringBuilder sb, AddressLevel parent, string name, int index)
        {
            if (parent.IsParentAddressLevel(AddressLevel))
            {
                sb.Append(name).Append(index != 0 ? index.ToString() : "*");
            }
        }

        public string CreateResponseKey(long messageId, int remoteAddressIndex)
        {
            return ToString() + '-' + messageId + '-' + remoteAddressIndex;
        }
    }
}