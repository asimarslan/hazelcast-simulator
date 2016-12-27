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

namespace Hazelcast.Simulator.Protocol.Core
{
    /**
     * Defines the address level of a Simulator component.
     *
     * <pre>
     *                                               +---+
     * REMOTE                                        + R +
     *                                               +---+
     *                                                 |
     *                                                 v
     *                                               +---+
     * COORDINATOR           +-----------------------+ C +----------------------+
     *                       |                       +---+                      |
     *                       |                                                  |
     *                       v                                                  v
     *                    +--+---+                                           +---+--+
     * AGENT              | C_A1 |                              +------------+ C_A2 +------------+
     *                    +--+---+                              |            +------+            |
     *                       |                                 |                                 |
     *                       v                                 v                                 v
     *                  +----+----+                       +----+----+                       +----+----+
     * WORKER       +---+ C_A1_W1 +---+               +---+ C_A2_W1 +---+               +---+ C_A2_W2 +---+
     *              |   +---------+   |               |   +---------+   |               |   +---------+   |
     *              |                 |               |                 |               |                 |
     *              v                 v               v                 v               v                 v
     *        +-----+------+   +------+-----+   +-----+------+   +------+-----+   +-----+------+   +------+-----+
     * TEST   | C_A1_W1_T1 |   | C_A1_W1_T2 |   | C_A2_W1_T1 |   | C_A2_W1_T2 |   | C_A2_W2_T1 |   | C_A2_W2_T2 |
     *        +------------+   +------------+   +------------+   +------------+   +------------+   +------------+
     * </pre>
     */

    public enum AddressLevel
    {
        REMOTE = -1,
        COORDINATOR = 0,
        AGENT = 1,
        WORKER = 2,
        TEST = 3
    }

    public static class AddressLevelUtil
    {
        public static int GetMinLevel(this AddressLevel al)
        {
            return (int)AddressLevel.REMOTE;
        }

        public static int GetMaxLevel(this AddressLevel al) => (int)AddressLevel.TEST;

        public static bool IsParentAddressLevel(this AddressLevel al, AddressLevel addressLevel) => (int)al < (int)addressLevel;
    }
}