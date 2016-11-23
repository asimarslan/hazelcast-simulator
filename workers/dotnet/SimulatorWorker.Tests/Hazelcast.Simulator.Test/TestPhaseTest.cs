using NUnit.Framework;

namespace Hazelcast.Simulator.Test
{
	[TestFixture]
	public class TestPhaseTest
	{
		[SetUp]
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
			Assert.AreEqual(false, TestPhase.LocalPrepare.IsGlobal());
			Assert.AreEqual(true, TestPhase.GlobalPrepare.IsGlobal());
			Assert.AreEqual(false, TestPhase.Warmup.IsGlobal());
			Assert.AreEqual(false, TestPhase.LocalAfterWarmup.IsGlobal());
			Assert.AreEqual(true, TestPhase.GlobalAfterWarmup.IsGlobal());
			Assert.AreEqual(false, TestPhase.Run.IsGlobal());
			Assert.AreEqual(true, TestPhase.GlobalVerify.IsGlobal());
			Assert.AreEqual(false, TestPhase.LocalVerify.IsGlobal());
			Assert.AreEqual(true, TestPhase.GlobalTeardown.IsGlobal());
			Assert.AreEqual(false, TestPhase.LocalTeardown.IsGlobal());
		}


	    [Test]
	    public void TestLastPhase()
	    {
	        Assert.AreEqual(TestPhase.LocalTeardown, TestPhases.GetLastTestPhase());
	    }
	}
}

