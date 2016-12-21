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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using static System.Environment;
using static Hazelcast.Simulator.TestEnvironmentUtils;
using static Hazelcast.Simulator.Utils.FileUtils;

namespace Hazelcast.Simulator.Utils
{
    [TestFixture]
    public class ExceptionReporterTest
    {
        [SetUp]
        public void Setup()
        {
            SetupFakeUserDir();
            SetEnvironmentVariable(WorkerHome, GetEnvironmentVariable(UserDirTest) );
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
        public void TestReportException()
        {
            ExceptionReporter.Report("testID", new Exception("Expected exception"));

            string userDirectoryPath = GetUserDirectoryPath();
            string exceptionFile = Path.Combine(userDirectoryPath, "1.exception");

            Assert.True(File.Exists(exceptionFile));
            Assert.False(File.Exists(Path.Combine(userDirectoryPath, "1.exception.tmp")));
            Assert.IsNotEmpty(File.ReadAllText(exceptionFile));
        }

        [Test]
        public void TestReportNullCause()
        {
            ExceptionReporter.Report("testID",null);

            string userDirectoryPath = GetUserDirectoryPath();
            string exceptionFile = Path.Combine(userDirectoryPath, "1.exception");

            Assert.False(File.Exists(exceptionFile));
        }

        [Test]
        public void TestReportTooManyException()
        {
            //init internal static counter to exceed the max value
            var memberInfo = typeof(ExceptionReporter).GetMember("failureCount", BindingFlags.NonPublic | BindingFlags.Static)[0];
            ((FieldInfo)memberInfo).SetValue(null, 1001);

            ExceptionReporter.Report("testID", new Exception("Expected exception"));

            string userDirectoryPath = GetUserDirectoryPath();
            string exceptionFile = Path.Combine(userDirectoryPath, "1.exception");

            Assert.False(File.Exists(exceptionFile));
        }

    }
}