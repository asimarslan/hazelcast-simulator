using System;
using System.Reflection;

namespace Hazelcast.Simulator.Utils
{
    public class ValueAttribute: Attribute
	{
		public object Value { get; private set;}
		public object Value2 { get; private set;}
		public object Value3 { get; private set;}

		public ValueAttribute(object value, object value2=null, object value3=null)
		{
			this.Value = value;
			this.Value2 = value2;
			this.Value3 = value3;
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

