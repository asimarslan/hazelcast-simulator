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

namespace Hazelcast.Simulator.Test
{

    [AttributeUsage(AttributeTargets.Method)]
    public class SetupAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TeardownAttribute : Attribute
    {
        public bool Global { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class VerifyAttribute : Attribute
    {
        public bool Global { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RunAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeRunAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AfterRunAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PrepareAttribute : Attribute
    {
        public bool Global { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public string Property { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TimeStepAttribute : Attribute
    {
        public double Probability { get; set; }
    }
}