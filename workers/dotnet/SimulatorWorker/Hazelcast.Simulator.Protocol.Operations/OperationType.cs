using System;
using Hazelcast.Simulator.Utils;

namespace Hazelcast.Simulator.Protocol.Operations
{
	public enum OperationType
	{
		[Value(typeof(PingOperation))]
		PING = 4000,

		[Value(typeof(TerminateWorkerOperation))]
		TERMINATE_WORKER = 4001,

		[Value(typeof(KillWorkerOperation))]
		KILL_WORKER = 4003,

		[Value(typeof(CreateTestOperation))]
		CREATE_TEST = 4002,

	    [Value(typeof(ExecuteScriptOperation))]
	    EXECUTE_SCRIPT = 4003,

		[Value(typeof(StartTestPhaseOperation))]
		START_TEST_PHASE=5000,

		[Value(typeof(StartTestOperation))]
		START_TEST = 5001,

		[Value(typeof(StopTestOperation))]
		STOP_TEST=5002
	}

	public static class OperationTypes
	{
		public static Type GetClassType(this OperationType ot)
		{
			var attr = ValueAttribute.GetAttr(ot);
			return attr.Value as Type;
		}
	}
}

