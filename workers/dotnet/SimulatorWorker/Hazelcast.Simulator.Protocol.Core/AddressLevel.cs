using log4net.Appender;
namespace Hazelcast.Simulator.Protocol.Core
{
	/**
	 * Defines the address level of a Simulator component.
	 *
	 * <pre>
	 *                                               +---+
	 * REMOTE                                        + R +
	 *                                               +---+
	 *                                                 |
	 *                                                 v
	 *                                               +---+
	 * COORDINATOR           +-----------------------+ C +----------------------+
	 *                       |                       +---+                      |
	 *                       |                                                  |
	 *                       v                                                  v
	 *                    +--+---+                                           +---+--+
	 * AGENT              | C_A1 |                              +------------+ C_A2 +------------+
	 *                    +--+---+                              |            +------+            |
	 *                       |                                 |                                 |
	 *                       v                                 v                                 v
	 *                  +----+----+                       +----+----+                       +----+----+
	 * WORKER       +---+ C_A1_W1 +---+               +---+ C_A2_W1 +---+               +---+ C_A2_W2 +---+
	 *              |   +---------+   |               |   +---------+   |               |   +---------+   |
	 *              |                 |               |                 |               |                 |
	 *              v                 v               v                 v               v                 v
	 *        +-----+------+   +------+-----+   +-----+------+   +------+-----+   +-----+------+   +------+-----+
	 * TEST   | C_A1_W1_T1 |   | C_A1_W1_T2 |   | C_A2_W1_T1 |   | C_A2_W1_T2 |   | C_A2_W2_T1 |   | C_A2_W2_T2 |
	 *        +------------+   +------------+   +------------+   +------------+   +------------+   +------------+
	 * </pre>
	 */
	public enum AddressLevel
	{
		REMOTE=-1,
		COORDINATOR=0,
		AGENT=1,
		WORKER=2,
		TEST=3
	}

	public static class AddressLevelUtil
	{
		public static int GetMinLevel(this AddressLevel al)
		{
			return (int)AddressLevel.REMOTE;
		}

		public static int GetMaxLevel(this AddressLevel al)
		{
			return (int)AddressLevel.TEST;
		}

		public static bool IsParentAddressLevel(this AddressLevel al, AddressLevel addressLevel)
		{
			return (int)al < (int)addressLevel;
		}


	}
}

