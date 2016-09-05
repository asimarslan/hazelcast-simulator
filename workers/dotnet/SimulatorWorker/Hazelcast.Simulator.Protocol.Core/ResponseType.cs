namespace Hazelcast.Simulator.Protocol.Core
{
  public enum ResponseType
  {
    /**
     * Is returned when the {@link SimulatorMessage} was correctly processed.
     */
    SUCCESS = 0,

    /**
     * Is returned when the addressed Coordinator was not found by an Agent component.
     */
    FAILURE_COORDINATOR_NOT_FOUND = 1,

    /**
     * Is returned when the addressed Agent was not found by the Coordinator.
     */
    FAILURE_AGENT_NOT_FOUND = 2,

    /**
     * Is returned when the addressed Worker was not found by an Agent component.
     */
    FAILURE_WORKER_NOT_FOUND = 3,

    /**
     * Is returned when the addressed Test was not found by a Worker component.
     */
    FAILURE_TEST_NOT_FOUND = 4,

    /**
     * Is returned when an implementation of {@link hazelcast.simulator.protocol.processors.OperationProcessor}
     * does not implement the transmitted {@link hazelcast.simulator.protocol.operation.SimulatorOperation}.
     */
    UNSUPPORTED_OPERATION_ON_THIS_PROCESSOR = 5,

    /**
     * Is returned when an exception occurs during the execution of a
     * {@link hazelcast.simulator.protocol.operation.SimulatorOperation}.
     */
    EXCEPTION_DURING_OPERATION_EXECUTION = 6,

    /**
     * Is returned when a {@link ResponseFuture} was unblocked by a
     * {@link hazelcast.simulator.protocol.operation.FailureOperation}.
     */
    UNBLOCKED_BY_FAILURE = 7,

    /**
     * Is returned when a {@link ResponseFuture#get=} was interrupted.
     */
    INTERRUPTED = 8
  }
}