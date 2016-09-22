using Hazelcast.Simulator.Test;
using NUnit.Framework;

namespace Hazelcast.Simulator.Protocol.Operations
{
	[TestFixture]
	public class TestPhaseTest
	{
		[NUnit.Framework.SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown() { }

		[Test]
		public void TestDescription()
		{
			Assert.AreEqual("setup", TestPhase.Setup.GetDescription());
			Assert.AreEqual("local prepare", TestPhase.LocalPrepare.GetDescription());
			Assert.AreEqual("global prepare", TestPhase.GlobalPrepare.GetDescription());
			Assert.AreEqual("warmup", TestPhase.Warmup.GetDescription());
			Assert.AreEqual("local after warmup", TestPhase.LocalAfterWarmup.GetDescription());
			Assert.AreEqual("global after warmup", TestPhase.GlobalAfterWarmup.GetDescription());
			Assert.AreEqual("run", TestPhase.Run.GetDescription());
			Assert.AreEqual("global verify", TestPhase.GlobalVerify.GetDescription());
			Assert.AreEqual("local verify", TestPhase.LocalVerify.GetDescription());
			Assert.AreEqual("global tear down", TestPhase.GlobalTeardown.GetDescription());
			Assert.AreEqual("local tear down", TestPhase.LocalTeardown.GetDescription());
		}

		[Test]
		public void TestIsGlobal()
		{
			Assert.AreEqual(false, TestPhase.Setup.IsGlobal());
			Assert.AreEqual(false, TestPhase.LocalPrepare.GetDescription());
			Assert.AreEqual(true, TestPhase.GlobalPrepare.GetDescription());
			Assert.AreEqual(false, TestPhase.Warmup.GetDescription());
			Assert.AreEqual(false, TestPhase.LocalAfterWarmup.GetDescription());
			Assert.AreEqual(true, TestPhase.GlobalAfterWarmup.GetDescription());
			Assert.AreEqual(false, TestPhase.Run.GetDescription());
			Assert.AreEqual(true, TestPhase.GlobalVerify.GetDescription());
			Assert.AreEqual(false, TestPhase.LocalVerify.GetDescription());
			Assert.AreEqual(true, TestPhase.GlobalTeardown.GetDescription());
			Assert.AreEqual(false, TestPhase.LocalTeardown.GetDescription());
		}
	}
}

