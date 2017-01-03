﻿// Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
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

namespace Hazelcast.Simulator.Test
{
    /// <summary>
    /// The TestContext is they way for a test to get access to test related information.
    /// Most importantly if a test is running.
    /// </summary>
    public interface ITestContext
    {
        /// <summary>
        /// Checks if the test is currently warming up. This method is true only during the warmup and can be used inside
        /// <ol>
        ///     <li><see cref="Hazelcast.Simulator.Test.BeforeRunAttribute"/></li>
        ///     <li><see cref="Hazelcast.Simulator.Test.TimeStepAttribute"/></li>
        ///     <li><see cref="Hazelcast.Simulator.Test.AfterRunAttribute"/></li>
        /// </ol>
        /// methods to do warmup specific things. For example if a different number of iterations is required during warmup, than
        /// during actual running, this method can accessed to figure out what to load.
        ///
        /// If the test isn't in the run or warmup state, the return value is undefined.
        ///
        /// Using the value inside the timestep method should be done with great care since it can lead to an incorrect warmup and
        /// then the jit needs to do a new warmup during the actual running.
        /// </summary>
        ///<returns>true if warming up, false otherwise.</returns>
        bool IsWarmingUp();

        /// <summary>returns the id of the current test</summary>
        /// <returns>the id of the current test</returns>
        string GetTestId();

        /// <summary>
        /// Returns the public ip address of the machine the test runs on. In some environments like ec2, there are
        /// public and private  ip addresses.
        /// </summary>
        /// <returns>the public ip address</returns>
        string GetPublicIpAddress();

        /// <summary>Checks if the run phase or warmup phase has stopped</summary>
        /// <returns>true if stopped, false otherwise.</returns>
        bool IsStopped();

        /// <summary>Stops the run or warmup phase. In most cases an outside duration is passed and the test will run
        /// as long as needed or until an exception is thrown. But in certain condition the implementer of a test wants
        /// to stop the run/warmup phase directly.
        /// Once stopped, the test moves on to the next phase. If the warmup is stopped, the test will eventually move
        /// on to the run phase.</summary>
        void Stop();

        /// <summary>Echoes a message to coordinator.
        ///
        /// Be very careful sending huge quantities of messages to the coordinator because it cause stability issues.
        /// Messages are written async, so you could easily kill the by flooding it or causing other problems. So don't
        /// use this as a debug logging alternative.
        /// </summary>
        /// <param name="msg">the message to send</param>
        void EchoCoordinator(string msg);
    }
}