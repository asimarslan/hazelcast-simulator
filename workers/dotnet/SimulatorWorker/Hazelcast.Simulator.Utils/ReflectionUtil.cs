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
using Hazelcast.Simulator.Probe;
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

        public static MemberInfo FindMemberInfo(Type type, string memberName)
        {
            //search fields/properties with Named attribute
            MemberInfo[] memberInfos = GetFieldWithAttribute(type, typeof(NamedAttribute)).ToArray();
            var result = new List<MemberInfo>();
            foreach (MemberInfo memberInfo in memberInfos)
            {
                var namedAttr = memberInfo.GetCustomAttribute<NamedAttribute>();
                if (namedAttr.Name == memberName)
                {
                    result.Add(memberInfo);
                }
            }
            MemberInfo memberInfoResult = GetFieldFromResult(result.ToArray(), memberName);
            if (memberInfoResult != null)
            {
                return memberInfoResult;
            }
            //search fields/properties with MemberInfo name if Named Attribute is missing
            memberInfos = type.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance);
            return GetFieldFromResult(memberInfos, memberName);
        }

        static MemberInfo GetFieldFromResult(MemberInfo[] memberInfos, string memberName)
        {
            if (memberInfos.Length == 1)
            {
                return memberInfos[0];
            }

            if (memberInfos.Length > 1)
            {
                throw new BindingException($"More than one field found with same name {memberName}");
            }
            return null;
        }

        public static IEnumerable<MemberInfo> GetFieldWithAttribute(Type type, Type attributeType)
        {
            MemberInfo[] memberInfos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            return memberInfos.Where(field => field.IsDefined(attributeType, true) &&
                (field.MemberType == MemberTypes.Field || field.MemberType == MemberTypes.Property));
        }

        public static IEnumerable<MemberInfo> GetMethodsWithAttribute(Type type, Type attributeType)
        {
            MemberInfo[] memberInfos = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            return memberInfos.Where(field => field.IsDefined(attributeType, true) && field.MemberType == MemberTypes.Method);
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

        public static object GetValue(object instance, MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(instance);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(instance);
                default:
                    throw new NotSupportedException();
            }

        }

        public static Type GetFieldType(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
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

        public static bool IsProbeType(MemberInfo memberInfo)
        {
            return memberInfo != null && GetFieldType(memberInfo) == typeof(IProbe);
        }

        public static string GetProbeName(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return null;
            }
            var namedAttribute = memberInfo.GetCustomAttribute<NamedAttribute>();
            return namedAttribute?.Name ?? memberInfo.Name;
        }

        public static bool IsPartOfTotalThoughput(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return false;
            }
            var attr = memberInfo.GetCustomAttribute<InjectProbeAttribute>();
            return attr?.useForThroughput ?? false;

        }
    }
}