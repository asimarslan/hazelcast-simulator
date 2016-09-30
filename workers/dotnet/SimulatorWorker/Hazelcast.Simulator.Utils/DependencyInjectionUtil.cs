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
using System.Reflection;
using Hazelcast.Simulator.Test;
using static Hazelcast.Simulator.Utils.ReflectionUtil;

namespace Hazelcast.Simulator.Utils
{
    public class DependencyInjectionUtil
    {

        public static bool Inject(object instance, string fieldPath, object value)
        {
            var fieldWithAttribute = GetFieldWithAttribute(instance.GetType(), typeof(InjectAttribute));
            foreach (MemberInfo memberInfo in fieldWithAttribute)
            {
                var injectAttr = memberInfo.GetCustomAttribute<InjectAttribute>();
                if (injectAttr.Property == fieldPath)
                {
                    SetValue(instance, memberInfo, value);
                    return true;
                }
            }
            return false;
        }

        public static bool Inject(object instance, string fieldPath, string valueStr)
        {
            valueStr = valueStr.Trim();
            string[] path = fieldPath.Split('.');

            object targetObject = FindTargetObject(instance, path);

            return Inject0(instance, path, valueStr);
        }

        private static object FindTargetObject(object parent, string[] fieldPath)
        {
            for (int i = 0; i < fieldPath.Length-1; i++)
            {
                Type type = parent.GetType();
                var fieldName = fieldPath[i];

                FindField(type, fieldName);
            }


        }

        internal static bool Inject0(object instance, string[] fieldPath, string valueStr)
        {
            if (fieldPath.Length < 1)
            {
                return false;
            }
            if (fieldPath.Length > 1)
            {

            }
        }

        internal static bool InjectToField(object instance, string fieldName, string valueStr)
        {
            IEnumerable<MemberInfo> fieldWithAttribute = GetFieldWithAttribute(instance.GetType(), typeof(InjectAttribute));
            foreach (MemberInfo memberInfo in fieldWithAttribute)
            {
                var injectAttr = memberInfo.GetCustomAttribute<InjectAttribute>();
                if (injectAttr.Property == fieldName)
                {
                    SetValue(instance, memberInfo, valueStr);
                    return true;
                }
            }
            return false;
        }
    }
}