// Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Utils
{
    public class ReflectionUtil
    {
        public static object CreateInstanceOfType(string typeName)
        {
            Type type = Type.GetType(typeName, true, false);
            return Activator.CreateInstance(type);
        }

        public static MemberInfo[] FindMemberInfo(Type type, string memberName)
        {
            MemberInfo[] memberInfos = GetFieldWithAttribute(type, typeof(NamedAttribute)).ToArray();
            if (memberInfos.Length == 0)
            {
                memberInfos = type.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance);
            }
            return memberInfos;
        }

        public static IEnumerable<MemberInfo> GetFieldWithAttribute(Type type, Type attributeType)
        {
            MemberInfo[] memberInfos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            return memberInfos.Where(field => field.IsDefined(attributeType, true));
        }

        public static void SetValue(object instance, MemberInfo memberInfo, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(instance, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(instance, value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public static void SetValue(object instance, MemberInfo memberInfo, string valueStr)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)memberInfo;
                    var fieldValue = Convert.ChangeType(valueStr, fieldInfo.FieldType);
                    fieldInfo.SetValue(instance, fieldValue);
                    break;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)memberInfo;
                    var propertyValue = Convert.ChangeType(valueStr, propertyInfo.PropertyType);
                    propertyInfo.SetValue(instance, propertyValue);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}