using System.Threading;

namespace Hazelcast.Simulator.Utils
{
  internal class AtomicBoolean
  {
    private int _value;

    public AtomicBoolean()
    {
    }

    public AtomicBoolean(bool initialValue)
    {
      _value = initialValue ? 1 : 0;
    }

    public bool CompareAndSet(bool expect, bool update)
    {
      var e = expect ? 1 : 0;
      var u = update ? 1 : 0;
      return (Interlocked.CompareExchange(ref _value, u, e) == e);
    }

    public bool Get()
    {
      return _value != 0;
    }

    public bool GetAndSet(bool newValue)
    {
      for (;;)
      {
        var current = Get();
        if (CompareAndSet(current, newValue))
          return current;
      }
    }

    public void Set(bool newValue)
    {
      _value = newValue ? 1 : 0;
    }

    public bool WeakCompareAndSet(bool expect, bool update)
    {
      return CompareAndSet(expect, update);
    }
  }
}