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
using Hazelcast.Simulator.Utils;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    ///     Operation types defined by simlulator.
    /// </summary>
    public enum OperationType
    {
        [Value(typeof(IntegrationTestOperation))]
        IntegerationTest = 0,

        [Value(typeof(AuthenticationOperation))]
        Auth = 1,

        [Value(typeof(LogOperation))]
        Log = 2,

        [Value(typeof(PhaseCompletedOperation))]
        PhaseCompleted = 1001,

        [Value(typeof(PerformanceStatsOperation))]
        PerformanceState = 1002,

        [Value(typeof(PingOperation))]
        Ping = 4000,

        //does not need an operation
        TerminateWorker = 4001,

        [Value(typeof(CreateTestOperation))]
        CreateTest = 4002,

        [Value(typeof(ExecuteScriptOperation))]
        ExecuteScript = 4003,

        [Value(typeof(StartTestPhaseOperation))]
        StartTestPhase = 5000,

        [Value(typeof(StartTestOperation))]
        StartTest = 5001,

        [Value(typeof(StopTestOperation))]
        StopTest = 5002
    }

    public static class OperationTypes
    {
        public static Type GetClassType(this OperationType ot)
        {
            ValueAttribute attr = ValueAttribute.GetAttr(ot);
            return attr?.Value as Type;
        }

        public static OperationType GetOperationType(this ISimulatorOperation operation)
        {
            foreach (OperationType opType in Enum.GetValues(typeof(OperationType)))
            {
                if (opType.GetClassType() == operation.GetType())
                {
                    return opType;
                }
            }
            throw new ArgumentException($"operation does not have an enum value:{operation}");
        }
    }
}