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

using System.Collections.Generic;

namespace Hazelcast.Simulator.Metronome
{
    public class MetronomeFactory
    {
        private readonly string metronomeClass;
        private readonly long interval;

        public MetronomeFactory(IDictionary<string, string> properties)
        {
            properties.TryGetValue("metronomeClass", out this.metronomeClass);
            string intervalVal;
            if (properties.TryGetValue("metronomeInterval", out intervalVal))
            {
                this.interval = long.Parse(intervalVal);
            }
        }

        public MetronomeFactory(string metronomeClass="Hazelcast.Simulator.Metronome.EmptyMetronome", long interval=0)
        {
            this.metronomeClass = metronomeClass;
            this.interval = interval;
        }

        public IMetronome CreateMetronome()
        {
            //TODO parameters are ignored
            if (this.interval == 0)
            {
                return new EmptyMetronome();
            }
            else
            {
                return new SleepingMetronome(interval);
            }
        }
    }
}