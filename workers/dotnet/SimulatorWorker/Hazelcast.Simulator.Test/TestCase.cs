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

using System.Collections.Generic;
using System.Text;
using log4net;
using static Hazelcast.Simulator.Utils.Constants;

namespace Hazelcast.Simulator.Test
{
    public class TestCase
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TestCase));

        public string TestId { get; set; }

        public IDictionary<string, string> Properties { get; }

        public TestCase(string testTestId, IDictionary<string, string> properties = null)
        {
            TestId = testTestId;
            Properties = properties ?? new Dictionary<string, string>();
        }

        public string GetClassname() => GetProperty("class");

        public string GetProperty(string name)
        {
            string value;
            Properties.TryGetValue(name, out value);
            return value;
        }

        public string SetProperty(string name, string value) => Properties[name] = value;

        public void OverrideProperties(IDictionary<string, string> properties)
        {
            foreach (KeyValuePair<string, string> pair in Properties)
            {
                if (properties.ContainsKey(pair.Key))
                {
                    SetProperty(pair.Key, pair.Value);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("TestCase{");
            sb.Append(NEW_LINE).Append("    ").Append("id=").Append(TestId);
            sb.Append(',').Append(NEW_LINE).Append("    ").Append("class=").Append(GetClassname());

            var keys = new List<string>(Properties.Keys);
            keys.Sort();

            foreach (string key in keys)
            {
                if (!"class".Equals(key))
                {
                    sb.Append(',').Append(NEW_LINE).Append("    ").Append(key).Append('=').Append(Properties[key]);
                }
            }
            sb.Append(NEW_LINE).Append('}');
            return sb.ToString();
        }
    }
}