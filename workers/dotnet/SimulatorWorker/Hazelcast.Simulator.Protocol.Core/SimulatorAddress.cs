using System;
namespace Hazelcast.Simulator.Protocol.Core
{
	/// <summary>
	/// Address object which(uniquely) identifies one or more Simulator components.
	/// </summary>
	///<remark>
	/// Supports wildcards on each { @link AddressLevel} to target all components on that address level.
	/// For example a { @link SimulatorMessage}
	///	to<c>C_A2_W* _T1</c> will be sent to <c>C_A2_W1_T1</c> and <c>C_A2_W2_T1</c>.
	///
	///
	/// <pre>
	///                                               +---+
	/// REMOTE                                        + R +
	///                                               +---+
	///                                                 |
	///                                                 v
	///                                               +---+
	/// COORDINATOR           +-----------------------+ C +----------------------+
	///                       |                       +---+                      |
	///                       |                                                  |
	///                       v v
	///                    +--+---+                                           +---+--+
	/// AGENT              | C_A1 |                              +------------+ C_A2 +------------+
	///                    +--+---+                              |            +------+            |
	///                       |                                 |                                 |
	///                       v v                                 v
	///                  +----+----+                       +----+----+                       +----+----+
	/// WORKER       +---+ C_A1_W1 +---+               +---+ C_A2_W1 +---+               +---+ C_A2_W2 +---+
	///              |   +---------+   |               |   +---------+   |               |   +---------+   |
	///              |                 |               |                 |               |                 |
	///              v v               v v               v v
	///        +-----+------+   +------+-----+   +-----+------+   +------+-----+   +-----+------+   +------+-----+
	/// TEST   | C_A1_W1_T1 |   | C_A1_W1_T2 |   | C_A2_W1_T1 |   | C_A2_W1_T2 |   | C_A2_W2_T1 |   | C_A2_W2_T2 |
	///        +------------+   +------------+   +------------+   +------------+   +------------+   +------------+
	/// </pre>
	/// </remark>
	public class SimulatorAddress
	{

		public static readonly SimulatorAddress REMOTE = new SimulatorAddress(AddressLevel.REMOTE, 0, 0, 0);
		public static readonly SimulatorAddress COORDINATOR = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
		public static readonly SimulatorAddress ALL_AGENTS = new SimulatorAddress(AddressLevel.AGENT, 0, 0, 0);
		public static readonly SimulatorAddress ALL_WORKERS = new SimulatorAddress(AddressLevel.WORKER, 0, 0, 0);

		private const String REMOTE_STRING = "R";
		private const String COORDINATOR_STRING = "C";

		public SimulatorAddress(AddressLevel addressLevel, int agentIndex, int workerIndex, int testIndex)
		{
			AddressLevel = addressLevel;
			AgentIndex = agentIndex;
			WorkerIndex = workerIndex;
			TestIndex = testIndex;
		}

		public AddressLevel AddressLevel{ get; }
		public int AgentIndex{ get; }
		public int WorkerIndex{ get; }
		public int TestIndex{ get; }


		public int AddressIndex
		{
			get
			{
				switch (AddressLevel)
				{
					case AddressLevel.WORKER:
						return WorkerIndex;
					case AddressLevel.TEST:
						return TestIndex;
					default:
						throw new ArgumentException();
				}
			}
		}
	}
}

