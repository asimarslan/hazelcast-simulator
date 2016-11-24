﻿using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Operations
{
	public interface ISimulatorOperation
	{
	    Task<Response.Part> Run(OperationContext operationContext, SimulatorAddress targetAddress);

	    void SetSourceAddress(SimulatorAddress sourceAddress);
	}
}

