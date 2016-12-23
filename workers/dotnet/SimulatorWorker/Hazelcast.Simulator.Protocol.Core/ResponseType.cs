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

using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Core
{
    public enum ResponseType
    {
        /// <summary>
        ///     Is returned when the {@link SimulatorMessage} was correctly processed.
        /// </summary>
        Success = 0,

        /// <summary>
        ///     RESERVED : NOT USED ON WORKER
        /// </summary>
        FailureCoordinatorNotFound = 1,

        /// <summary>
        ///     RESERVED : NOT USED ON WORKER
        /// </summary>
        FailureAgentNotFound = 2,

        /// <summary>
        ///     RESERVED : NOT USED ON WORKER
        /// </summary>
        FailureWorkerNotFound = 3,

        /// <summary>
        ///     Is returned when the addressed Test was not found by a Worker component.
        /// </summary>
        FailureTestNotFound = 4,

        /// <summary>
        ///     Is returned when an implementation of <see cref="OperationProcessor" />
        ///     does not support the transmitted <see cref="ISimulatorOperation" />.
        /// </summary>
        UnsupportedOperationOnThisProcessor = 5,

        /// <summary>
        ///     Is returned when an exception occurs during the execution of a <see cref="ISimulatorOperation" />
        /// </summary>
        ExceptionDuringOperationExecution = 6,

        /// <summary>
        ///     RESERVED : NOT USED ON WORKER
        /// </summary>
        UnblockedByFailure = 7,

        /// <summary>
        ///     RESERVED : NOT USED ON WORKER
        /// </summary>
        Interrupted = 8
    }
}