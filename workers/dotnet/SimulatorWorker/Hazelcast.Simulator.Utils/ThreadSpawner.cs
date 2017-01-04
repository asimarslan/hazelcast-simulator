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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Hazelcast.Simulator.Utils
{
    /// <summary>
    /// Responsible for spawning and waiting for threads.
    /// </summary>
    /// <remarks>
    /// If used in a test context <see cref="testId"/> should be set to the testId of that test. This is needed to correlate an exception to a specific test case. 
    /// In a test context you should not set  <see cref="throwException"/> to <c>true</c>, so the <see cref="ExceptionReporter"/> will be used.
    /// You can also use your own threads in Simulator tests, but make sure that you detect thrown exceptions and report them to the <see cref="ExceptionReporter"/> by yourself.
    /// </remarks>
    public class ThreadSpawner
    {
        private readonly ConcurrentDictionary<string, int> ids = new ConcurrentDictionary<string, int>();
        private readonly SynchronizedCollection<Thread> threads = new SynchronizedCollection<Thread>();
        private readonly IList<Exception> uncaughtExceptions = new List<Exception>();
        private readonly string testId;
        private readonly bool throwException;

        public ThreadSpawner(string testId, bool throwException = false)
        {
            this.testId = testId;
            this.throwException = throwException;
        }

        /// <summary>
        /// Spawns a new thread for the given <see cref="Action"/> with default name prefix using "testId-Thread"
        /// </summary>
        /// <param name="action">the <see cref="Action"/> to execute</param>
        /// <returns>the created thread</returns>
        public Thread Spawn(Action action) => Spawn(testId + "-Thread", action);

        /// <summary>
        /// Spawns a new thread for the given <see cref="Action"/>
        /// </summary>
        /// <param name="namePrefix">the name prefix for the thread</param>
        /// <param name="action">the <see cref="Action"/> to execute</param>
        /// <returns>the created thread</returns>
        public Thread Spawn(string namePrefix, Action action)
        {
            if (namePrefix == null)
            {
                throw new NullReferenceException("Thread's namePrefix can't be null");
            }
            if (action == null)
            {
                throw new NullReferenceException("Thread's action can't be null");
            }
            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    if (throwException)
                    {
                        uncaughtExceptions.Add(e);
                    }
                    else
                    {
                        ExceptionReporter.Report(testId, e);
                    }
                }
            })
            {
                IsBackground = true,
                Name = namePrefix + '-' + ids.AddOrUpdate(namePrefix, 0, (key, value) => value + 1)
            };
            threads.Add(thread);
            thread.Start();
            return thread;
        }

        /// <summary>
        /// Waits for all threads to finish. if <see cref="throwException"/> is <c>true</c> then an 
        /// <see cref="AggregateException"/> will be thron including all the thrown exceptions in threads.
        /// </summary>
        public void AwaitCompletion()
        {
            foreach (var thread in threads)
            {
                thread.Join();
            }
            if (uncaughtExceptions.Count > 0)
            {
                throw new AggregateException(uncaughtExceptions);
            }
        }

        /// <summary>
        /// Interrupts all running threads.
        /// </summary>
        public void Interrupt()
        {
            foreach (Thread thread in threads)
            {
                thread.Interrupt();
            }
        }
    }
}