using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Protocol.Operations;

namespace Hazelcast.Simulator.Protocol.Core
{
    public enum ResponseType
    {
        /// <summary>
        /// Is returned when the {@link SimulatorMessage} was correctly processed.
        /// </summary>
        Success = 0,

        /// <summary>
        ///  RESERVED : NOT USED ON WORKER
        /// </summary>
        FailureCoordinatorNotFound = 1,

        /// <summary>
        ///  RESERVED : NOT USED ON WORKER
        /// </summary>
        FailureAgentNotFound = 2,

        /// <summary>
        ///  RESERVED : NOT USED ON WORKER
        /// </summary>
        FailureWorkerNotFound = 3,

        /// <summary>
        ///  Is returned when the addressed Test was not found by a Worker component.
        /// </summary>
        FailureTestNotFound = 4,

        /// <summary>
        ///  Is returned when an implementation of <see cref="OperationProcessor"/>
        ///  does not support the transmitted <see cref="ISimulatorOperation"/>.
        /// </summary>
        UnsupportedOperationOnThisProcessor = 5,

        /// <summary>
        ///  Is returned when an exception occurs during the execution of a <see cref="ISimulatorOperation"/>
        /// </summary>
        ExceptionDuringOperationExecution = 6,

        /// <summary>
        ///  RESERVED : NOT USED ON WORKER
        /// </summary>
        UnblockedByFailure = 7,

        /// <summary>
        ///  RESERVED : NOT USED ON WORKER
        /// </summary>
        Interrupted = 8
    }
}