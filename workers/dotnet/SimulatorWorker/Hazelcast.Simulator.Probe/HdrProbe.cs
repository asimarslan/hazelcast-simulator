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
using HdrHistogram;

namespace Hazelcast.Simulator.Probe
{
    /// <summary>
    /// Measures the latency distribution of a test.
    /// </summary>
    public class HdrProbe : IProbe
    {
        // we care only about microsecond accuracy. 10 ticks = 1 microseconds
        public static readonly long LowestDiscernibleValue = TimeSpan.FromTicks(10).Ticks;

        // we want to track up to an hour.
        public static readonly long HighestTrackableValue = TimeSpan.FromHours(1).Ticks;

        // since we care about us, the value should be 1000 according to the API doc of Recorder.
        public static readonly int NumberOfSignificantValueDigits = 3;

        private readonly Recorder recorder;

        private readonly bool partOfTotalThroughput;

        public HdrProbe(bool partOfTotalThroughput)
        {
            this.partOfTotalThroughput = partOfTotalThroughput;
            HistogramFactoryDelegate factoryDelegate =
                (instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits)
                => new LongConcurrentHistogram(instanceId, lowestDiscernibleValue, highestTrackableValue, numberOfSignificantValueDigits);
            this.recorder = new Recorder(LowestDiscernibleValue, HighestTrackableValue, NumberOfSignificantValueDigits,
                factoryDelegate);
        }

        public bool IsPartOfTotalThroughput() => this.partOfTotalThroughput;

        public void Done(long startNanos)
        {
            if (startNanos <= 0)
            {
                throw new ArgumentException("startedNanos has to be a positive number");
            }
            this.RecordValue(DateTime.Now.Ticks * 100 - startNanos);

        }

        public void RecordValue(long latencyNanos)
        {
            if (latencyNanos > HighestTrackableValue)
            {
                latencyNanos = HighestTrackableValue;
            }
            this.recorder.RecordValue(latencyNanos);
        }
    }
}