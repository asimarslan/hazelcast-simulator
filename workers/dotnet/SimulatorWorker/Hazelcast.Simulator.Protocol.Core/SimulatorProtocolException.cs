using System;

namespace Hazelcast.Simulator.Protocol.Core
{
  public class SimulatorProtocolException : Exception
  {
    public SimulatorProtocolException(string message):base(message)
    {
    }
  }
}