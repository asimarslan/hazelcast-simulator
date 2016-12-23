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
using log4net;
using static Hazelcast.Simulator.Utils.FileUtils;

namespace Hazelcast.Simulator.Utils
{
    /// <summary>
    ///     Responsible for writing an exception to a file. Every exception file will have a unique name.
    /// </summary>
    public class ExceptionReporter
    {
        private const int MaxExceptionCount = 1000;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExceptionReporter));

        private static long failureCount;

        public static void Report(string testId, Exception cause)
        {
            if (cause == null)
            {
                Logger.Fatal("Can't call report with a null exception");
                return;
            }

            long exceptionCount = Interlocked.Increment(ref failureCount);
            Logger.Warn($"Exception #{exceptionCount} detected.", cause);
            if (exceptionCount > MaxExceptionCount)
            {
                Logger.Warn("The maximum number of exceptions has been exceeded, so it won't be reported to the Agent.", cause);
                return;
            }

            string targetFileName = $"{exceptionCount}.exception";
            string userDirectoryPath = GetUserDirectoryPath();
            string tmpName = Path.Combine(userDirectoryPath, targetFileName + ".tmp");
            string text = testId + "\n" + cause.StackTrace;

            File.WriteAllText(tmpName, text);
            File.Move(tmpName, Path.Combine(userDirectoryPath, targetFileName));
        }

        public static void Reset() => failureCount = 0;
    }
}