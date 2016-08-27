using System;
using System.Reflection;

namespace Hazelcast.Simulator.Utils
{
	public class ValueAttribute: Attribute
	{
		public object Value { get; private set;}
		public ValueAttribute(object value)
		{
			this.Value = value;
		}

		public static ValueAttribute GetAttr<T>(T ot)
		{
			return (ValueAttribute)GetCustomAttribute(ForValue(ot), typeof(ValueAttribute));
		}

		public static MemberInfo ForValue<T>(T ot)
		{
			return typeof(T).GetField(Enum.GetName(typeof(T), ot));
		}
	}
}

