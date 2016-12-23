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
using Hazelcast.Simulator.Protocol.Operations;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Core
{
    public class SimulatorMessage
    {
        public SimulatorAddress Destination { get; }

        public SimulatorAddress Source { get; }

        public long MessageId { get; }

        public OperationType OperationType { get; }

        public string OperationData { get; }

        public SimulatorMessage(SimulatorAddress destination, SimulatorAddress source, long messageId,
            OperationType operationType, string operationData)
        {
            Destination = destination;
            Source = source;
            MessageId = messageId;
            OperationType = operationType;
            OperationData = operationData;
        }

        public ISimulatorOperation ToOperation()
        {
            Type type = OperationType.GetClassType();
            ISimulatorOperation simulatorOperation;
            try
            {
                simulatorOperation = (ISimulatorOperation)JsonConvert.DeserializeObject(OperationData, type);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            simulatorOperation.SetSourceAddress(Source);
            return simulatorOperation;
        }

        public override string ToString() =>
            $"[SimulatorMessage: Destination={Destination}, Source={Source}, MessageId={MessageId}, " +
            $"operationType={OperationType}, operationData={OperationData}]";
    }
}