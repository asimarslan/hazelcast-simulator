﻿using System;
using Hazelcast.Simulator.Utils;

namespace Hazelcast.Simulator.Protocol.Operations
{
	public enum OperationType
	{
	    [Value(typeof(IntegrationTestOperation))]
	    IntegerationTest=0,

	    [Value(typeof(LogOperation))]
	    Log=1,

		[Value(typeof(PingOperation))]
		Ping = 4000,

		[Value(typeof(TerminateWorkerOperation))]
		TerminateWorker = 4001,

		[Value(typeof(CreateTestOperation))]
		CreateTest = 4002,

	    [Value(typeof(ExecuteScriptOperation))]
	    ExecuteScript = 4003,

		[Value(typeof(StartTestPhaseOperation))]
		StartTestPhase=5000,

		[Value(typeof(StartTestOperation))]
		StartTest = 5001,

		[Value(typeof(StopTestOperation))]
		StopTest=5002
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

