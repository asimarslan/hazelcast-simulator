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
using Hazelcast.Simulator.Metronome;

namespace Hazelcast.Simulator.Test
{

    [AttributeUsage(AttributeTargets.Method)]
    public class SetupAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TeardownAttribute : Attribute
    {
        public bool Global { get; set; }

        public TeardownAttribute(bool global = false)
        {
            this.Global = global;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class VerifyAttribute : Attribute
    {
        public bool Global { get; set; }

        public VerifyAttribute(bool global = false)
        {
            this.Global = global;
        }
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
        public bool Global { get; set; }

        public PrepareAttribute(bool global = false)
        {
            this.Global = global;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TimeStepAttribute : Attribute
    {
        public double Probability { get; set; }

        public TimeStepAttribute(double probability)
        {
            this.Probability = probability;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property )]
    public class InjectAttribute : Attribute
    {
    }

    public class InjectProbeAttribute : InjectAttribute
    {
        public bool useForThroughput { get; set; }

        public InjectProbeAttribute(bool useForThroughput = false)
        {
            this.useForThroughput = useForThroughput;
        }
    }

    public class InjectMetronomeAttribute : InjectAttribute
    {
        public int IntervalMillis { get; set; }
        public MetronomeType MetronomeType { get; set; }

        public InjectMetronomeAttribute(int intervalMillis = 0, MetronomeType metronomeType= MetronomeType.Nop)
        {
            this.IntervalMillis = intervalMillis;
            this.MetronomeType = metronomeType;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property )]
    public class NamedAttribute : Attribute
    {
        public string Name { get; set; }

        public NamedAttribute(string name)
        {
            this.Name = name;
        }
    }
}