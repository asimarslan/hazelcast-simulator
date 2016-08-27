using System;
using NUnit.Framework;

namespace Hazelcast.Simulator.Protocol.Operations
{
	[TestFixture]
	public class OperationTypeTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown() { }

		[Test]
		public void TestClassType()
		{
			Assert.AreEqual(typeof(CreateTestOperation), OperationType.CREATE_TEST.GetClassType());
			Assert.AreEqual(typeof(KillWorkerOperation), OperationType.KILL_WORKER.GetClassType());
			Assert.AreEqual(typeof(PingOperation), OperationType.PING.GetClassType());
			Assert.AreEqual(typeof(StartTestOperation), OperationType.START_TEST.GetClassType());
			Assert.AreEqual(typeof(StartTestPhaseOperation), OperationType.START_TEST_PHASE.GetClassType());
			Assert.AreEqual(typeof(StopTestOperation), OperationType.STOP_TEST.GetClassType());
			Assert.AreEqual(typeof(TerminateWorkerOperation), OperationType.TERMINATE_WORKER.GetClassType());
		}

		[Test]
		public void TestImplicitConvert()
		{
			Assert.AreEqual(5001, (int)OperationType.START_TEST);
			Assert.AreEqual(4002, (int)OperationType.CREATE_TEST);
			Assert.AreEqual(4003, (int)OperationType.KILL_WORKER);
			Assert.AreEqual(4000, (int)OperationType.PING);
			Assert.AreEqual(5000, (int)OperationType.START_TEST_PHASE);
			Assert.AreEqual(5002, (int)OperationType.STOP_TEST);
			Assert.AreEqual(4001, (int)OperationType.TERMINATE_WORKER);
		}
	}
}

