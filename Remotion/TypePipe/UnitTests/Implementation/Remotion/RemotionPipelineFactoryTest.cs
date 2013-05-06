﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using NUnit.Framework;
using Remotion.TypePipe.CodeGeneration.Implementation.ReflectionEmit;
using Remotion.TypePipe.Implementation.Remotion;
using Remotion.Development.UnitTesting;

namespace Remotion.TypePipe.UnitTests.Implementation.Remotion
{
  [TestFixture]
  public class RemotionPipelineFactoryTest
  {
    private RemotionPipelineFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new RemotionPipelineFactory();
    }

    [Test]
    public void NewModuleBuilderFactory ()
    {
      var result = _factory.Invoke ("NewModuleBuilderFactory");

      Assert.That (result, Is.TypeOf<RemotionModuleBuilderFactoryDecorator>());
      Assert.That (PrivateInvoke.GetNonPublicField (result, "_moduleBuilderFactory"), Is.TypeOf<ModuleBuilderFactory>());
    }
  }
}