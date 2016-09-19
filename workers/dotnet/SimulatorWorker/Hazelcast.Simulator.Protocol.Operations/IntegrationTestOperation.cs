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

using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Operations
{
    public class IntegrationTestOperation : ISimulatorOperation
    {
        public enum Type
        {
            EQUALS,
            NESTED_SYNC,
            NESTED_ASYNC,
            DEEP_NESTED_SYNC,
            DEEP_NESTED_ASYNC
        }

        ///Defines the <see ref="Type">Type</see>  of this operation.
        public string IntegrationType { get; }

        ///Defines the payload of this operation.
        public string TestData { get; }

        public Task Run(OperationContext operationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}