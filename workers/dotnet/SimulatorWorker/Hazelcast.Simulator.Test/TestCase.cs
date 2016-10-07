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

using log4net;
using System.Collections.Generic;
using System.Text;
using static Hazelcast.Simulator.Utils.Constants;

namespace Hazelcast.Simulator.Test
{
    public class TestCase
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TestCase));

        public string Id { get; set; }

        public IDictionary<string, string> Properties { get; }

        public TestCase(string testId, IDictionary<string, string> properties = null)
        {
            this.Id = testId;
            this.Properties = properties?? new Dictionary<string, string>();
        }

        public string GetClassname() => this.Properties["class"];

        public string GetProperty(string name) => this.Properties[name];

        public string SetProperty(string name, string value) => this.Properties[name] = value;

        public void OverrideProperties(IDictionary<string, string> properties)
        {
            foreach (KeyValuePair<string, string> pair in this.Properties)
            {
                if (properties.ContainsKey(pair.Key))
                {
                    this.SetProperty(pair.Key, pair.Value);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("TestCase{");
            sb.Append(NEW_LINE).Append("    ").Append("id=").Append(this.Id);
            sb.Append(',').Append(NEW_LINE).Append("    ").Append("class=").Append(this.GetClassname());

            var keys = new List<string>(this.Properties.Keys);
            keys.Sort();

            foreach (string key in keys)
            {
                if (!"class".Equals(key))
                {
                    sb.Append(',').Append(NEW_LINE).Append("    ").Append(key).Append('=').Append(this.Properties[key]);
                }
            }
            sb.Append(NEW_LINE).Append('}');
            return sb.ToString();
        }
    }
}