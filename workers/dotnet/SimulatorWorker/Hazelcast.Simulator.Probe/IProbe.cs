// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
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

namespace Hazelcast.Simulator.Probe
{
    public interface IProbe
    {
        /// <summary>
        ///     Checks if a probe should be considered to calculate the throughput of a test.
        /// </summary>
        /// <returns><c>true</c> if probe is relevant for throughput, <c>false</c> otherwise</returns>
        bool IsPartOfTotalThroughput();

        /// <summary>
        ///     Calculates the latency from an external start time and records the value.
        /// </summary>
        /// <param name="startNanos">external start time from <c>DateTime.Now.Ticks * 100</c></param>
        void Done(long startNanos);

        /// <summary>
        ///     Adds a latency value in nanoseconds to the probe result.
        /// </summary>
        /// <param name="latencyNanos">latencyNanos latency value in nanoseconds</param>
        void RecordValue(long latencyNanos);
    }
}