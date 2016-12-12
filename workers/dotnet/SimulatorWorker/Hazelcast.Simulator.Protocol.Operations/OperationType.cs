using System;
using Hazelcast.Simulator.Utils;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Operation types defined by simlulator.
    /// </summary>
    public enum OperationType
    {
        [Value(typeof(IntegrationTestOperation))]
        IntegerationTest = 0,

        [Value(typeof(LogOperation))]
        Log = 1,

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
            var attr = ValueAttribute.GetAttr(ot);
            return attr.Value as Type;
        }

        public static OperationType GetOperationType(this ISimulatorOperation operation)
        {
            foreach (OperationType opType in Enum.GetValues(typeof(OperationType)))
            {
                if (opType.GetClassType().Equals(operation))
                {
                    return opType;
                }
            }
            throw new ArgumentException($"operation does not have an enum value:{operation}");
        }
    }
}