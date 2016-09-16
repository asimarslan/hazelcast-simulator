using System;
using Hazelcast.Core;
namespace Hazelcast.Simulator.Test
{
	public class TestContext
	{
		public const string LOCALHOST = "127.0.0.1";

		public IHazelcastInstance TargetInstance { get; private set; }
		public string TestId { get; private set;}
		public string PublicIpAddress { get; private set; }

		volatile bool _stopped;

		public TestContext(string testId, IHazelcastInstance targetInstance=null, string publicIpAddress=LOCALHOST)
		{
			TestId = testId;
			TargetInstance = targetInstance;
			PublicIpAddress = publicIpAddress;
		}

		public bool IsStoppped { 
			get 
			{
				return _stopped;
			}
			private set 
			{ 
				_stopped = value; 
			} 
		}

		public void Stop()
		{
			_stopped = true;
		}

		public void AfterLocalWarmup()
		{
			_stopped = false;
		}


	}
}

