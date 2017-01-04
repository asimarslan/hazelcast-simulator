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
using System.IO;
using System.Threading;
using NUnit.Framework;
using static System.Environment;
using static Hazelcast.Simulator.TestEnvironmentUtils;
using static Hazelcast.Simulator.Utils.FileUtils;

namespace Hazelcast.Simulator.Utils
{
    [TestFixture]
    public class ThreadSpawnerTest
    {
        private static readonly Action sleepInfiniteAction = () =>
        {
            try
            {
                Thread.Sleep(int.MaxValue);
            }
            catch (ThreadInterruptedException)
            {
                //ignore
            }
        };

        [SetUp]
        public void Setup()
        {
            SetupFakeUserDir();
            SetEnvironmentVariable(WorkerHome, GetEnvironmentVariable(UserDirTest));
            ExceptionReporter.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            TeardownFakeUserDir();
            ExceptionReporter.Reset();
            SetEnvironmentVariable(WorkerHome, null);
        }

        [Test]
        public void TestNullPrefix()
        {
            var threadSpawner = new ThreadSpawner("someTestId");
            Assert.Throws<NullReferenceException>(() => threadSpawner.Spawn(null, () => { }));
        }

        [Test]
        public void TestNullAction()
        {
            var threadSpawner = new ThreadSpawner("someTestId");
            Assert.Throws<NullReferenceException>(() => threadSpawner.Spawn("somePrefix", null));
        }

        [Test]
        public void TestThreadSpawner()
        {
            var counter = 0;
            var spawner = new ThreadSpawner("someTestId");
            for (var i = 0; i < 10; i++)
            {
                spawner.Spawn(() => { Interlocked.Increment(ref counter); });
            }
            spawner.AwaitCompletion();
            Assert.AreEqual(counter, 10);
        }

        [Test]
        public void TestThreadSpawnerWithPrefix()
        {
            var counter = 0;
            var spawner = new ThreadSpawner("someTestId");
            for (var i = 0; i < 10; i++)
            {
                spawner.Spawn("NamePrefix", () => { Interlocked.Increment(ref counter); });
            }
            spawner.AwaitCompletion();
            Assert.AreEqual(counter, 10);
        }

        [Test]
        public void TestInterrupt()
        {
            var spawner = new ThreadSpawner("AnyTestCaseId", true);
            spawner.Spawn(sleepInfiniteAction);
            spawner.Interrupt();
            spawner.AwaitCompletion();
        }

        [Test]
        public void TestThrowException()
        {
            var spawner = new ThreadSpawner("AnyTestCaseId", true);
            spawner.Spawn("NamePrefix", () => { throw new Exception("Expected exception"); });
            Assert.Throws<AggregateException>(() => spawner.AwaitCompletion());
        }

        [Test]
        public void TestThreadSpawnerException_reportException()
        {
            var spawner = new ThreadSpawner("someTestId");
            for (var i = 0; i < 10; i++)
            {
                spawner.Spawn("NamePrefix", () => { throw new Exception("Expected exception"); });
            }
            spawner.AwaitCompletion();
            string userDirectoryPath = GetUserDirectoryPath();
            string exceptionFile = Path.Combine(userDirectoryPath, "1.exception");

            Assert.True(File.Exists(exceptionFile));
            Assert.IsNotEmpty(File.ReadAllText(exceptionFile));
        }
    }
}