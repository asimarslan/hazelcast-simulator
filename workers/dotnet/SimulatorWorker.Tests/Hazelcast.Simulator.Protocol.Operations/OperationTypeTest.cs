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
			Assert.AreEqual(typeof(IntegrationTestOperation), OperationType.IntegerationTest.GetClassType());
			Assert.AreEqual(typeof(LogOperation), OperationType.Log.GetClassType());
			Assert.AreEqual(typeof(PingOperation), OperationType.Ping.GetClassType());
			Assert.AreEqual(typeof(TerminateWorkerOperation), OperationType.TerminateWorker.GetClassType());
			Assert.AreEqual(typeof(CreateTestOperation), OperationType.CreateTest.GetClassType());
			Assert.AreEqual(typeof(ExecuteScriptOperation), OperationType.ExecuteScript.GetClassType());
			Assert.AreEqual(typeof(StartTestPhaseOperation), OperationType.StartTestPhase.GetClassType());
			Assert.AreEqual(typeof(StartTestOperation), OperationType.StartTest.GetClassType());
			Assert.AreEqual(typeof(StopTestOperation), OperationType.StopTest.GetClassType());
		}

		[Test]
		public void TestImplicitConvert()
		{
			Assert.AreEqual(0, (int)OperationType.IntegerationTest);
			Assert.AreEqual(1, (int)OperationType.Log);
			Assert.AreEqual(4000, (int)OperationType.Ping);
			Assert.AreEqual(4001, (int)OperationType.TerminateWorker);
			Assert.AreEqual(4002, (int)OperationType.CreateTest);
			Assert.AreEqual(5001, (int)OperationType.StartTest);
			Assert.AreEqual(5000, (int)OperationType.StartTestPhase);
			Assert.AreEqual(5002, (int)OperationType.StopTest);
		}
	}
}

