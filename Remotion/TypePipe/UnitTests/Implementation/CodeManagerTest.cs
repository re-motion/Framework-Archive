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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Implementation
{
  [TestFixture]
  public class CodeManagerTest
  {
    private IGeneratedCodeFlusher _generatedCodeFlusherMock;
    private ITypeCache _typeCacheMock;

    private CodeManager _manager;

    [SetUp]
    public void SetUp ()
    {
      _generatedCodeFlusherMock = MockRepository.GenerateStrictMock<IGeneratedCodeFlusher>();
      _typeCacheMock = MockRepository.GenerateStrictMock<ITypeCache>();

      _manager = new CodeManager (_generatedCodeFlusherMock, _typeCacheMock);
    }

    [Test]
    public void FlushCodeToDisk ()
    {
      var assemblyAttribute = CustomAttributeDeclarationObjectMother.Create();
      var configID = "config";
      var fakeResult = "assembly path";
      _typeCacheMock.Expect (mock => mock.ParticipantConfigurationID).Return (configID);
      _generatedCodeFlusherMock
          .Expect (mock => mock.FlushCodeToDisk (Arg<IEnumerable<CustomAttributeDeclaration>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (
              mi =>
              {
                var assemblyAttributes = mi.Arguments[0].As<IEnumerable<CustomAttributeDeclaration>>().ToList();
                Assert.That (assemblyAttributes, Has.Count.EqualTo (2));
                Assert.That (assemblyAttributes[0], Is.SameAs (assemblyAttribute));
                Assert.That (assemblyAttributes[1].Type, Is.SameAs (typeof (TypePipeAssemblyAttribute)));
                Assert.That (assemblyAttributes[1].ConstructorArguments, Is.EqualTo (new[] { configID }));
                Assert.That (assemblyAttributes[1].NamedArguments, Is.Empty);
              });

      var result = _manager.FlushCodeToDisk (new[] { assemblyAttribute });

      _typeCacheMock.VerifyAllExpectations ();
      _generatedCodeFlusherMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void LoadFlushedCode ()
    {
      var type = ReflectionObjectMother.GetSomeType ();
      var assemblyMock = CreateAssemblyMock ("config", type);
      _typeCacheMock.Expect (mock => mock.ParticipantConfigurationID).Return ("config");
      _typeCacheMock.Expect (mock => mock.LoadTypes (new[] { type }));

      _manager.LoadFlushedCode (assemblyMock);

      assemblyMock.VerifyAllExpectations ();
      _typeCacheMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The specified assembly was not generated by the pipeline.\r\nParameter name: assembly")]
    public void LoadFlushedCode_MissingTypePipeAssemblyAttribute ()
    {
      _manager.LoadFlushedCode (GetType ().Assembly);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The specified assembly was generated with a different participant configuration: 'different config'.\r\nParameter name: assembly")]
    public void LoadFlushedCode_InvalidParticipantConfigurationID ()
    {
      _typeCacheMock.Stub (stub => stub.ParticipantConfigurationID).Return ("config");
      var assemblyMock = CreateAssemblyMock ("different config");

      _manager.LoadFlushedCode (assemblyMock);
    }

    private _Assembly CreateAssemblyMock (string participantConfigurationID, params Type[] types)
    {
      var assemblyMock = MockRepository.GenerateStrictMock<_Assembly> ();
      var assemblyAttribute = new TypePipeAssemblyAttribute (participantConfigurationID);
      assemblyMock.Expect (mock => mock.GetCustomAttributes (typeof (TypePipeAssemblyAttribute), false)).Return (new object[] { assemblyAttribute });
      assemblyMock.Expect (mock => mock.GetTypes ()).Return (types);

      return assemblyMock;
    }
  }
}