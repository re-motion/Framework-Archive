// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Implementation
{
  [TestFixture]
  public class CodeManagerTest
  {
    private ICodeGenerator _codeGeneratorMock;
    private object _codeGeneratorLock;
    private ITypeCache _typeCacheMock;

    private CodeManager _manager;

    [SetUp]
    public void SetUp ()
    {
      _codeGeneratorMock = MockRepository.GenerateStrictMock<ICodeGenerator>();
      _codeGeneratorLock = new object();
      _typeCacheMock = MockRepository.GenerateStrictMock<ITypeCache>();

      _manager = new CodeManager (_codeGeneratorMock, _codeGeneratorLock, _typeCacheMock);
    }

    [Test]
    public void FlushCodeToDisk ()
    {
      var configID = "config";
      var fakeResult = "assembly path";
      _typeCacheMock.Expect (mock => mock.ParticipantConfigurationID).Return (configID).WhenCalled (_ => CheckLock (false));
      _codeGeneratorMock
          .Expect (mock => mock.FlushCodeToDisk (Arg<CustomAttributeDeclaration>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (
              mi =>
              {
                var assemblyAttribute = (CustomAttributeDeclaration) mi.Arguments[0];
                Assert.That (assemblyAttribute.Type, Is.SameAs (typeof (TypePipeAssemblyAttribute)));
                Assert.That (assemblyAttribute.ConstructorArguments, Is.EqualTo (new[] { configID }));
                Assert.That (assemblyAttribute.NamedArguments, Is.Empty);
              })
          .WhenCalled (_ => CheckLock (true));

      var result = _manager.FlushCodeToDisk();

      _typeCacheMock.VerifyAllExpectations();
      _codeGeneratorMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void LoadFlushedCode ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      var assemblyMock = CreateAssemblyMock ("config", type);
      _typeCacheMock.Expect (mock => mock.ParticipantConfigurationID).Return ("config").WhenCalled (_ => CheckLock (false));
      _typeCacheMock.Expect (mock => mock.LoadTypes (new[] { type })).WhenCalled (_ => CheckLock (false));

      _manager.LoadFlushedCode (assemblyMock);

      assemblyMock.VerifyAllExpectations();
      _typeCacheMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The specified assembly was not generated by the pipeline.\r\nParameter name: assembly")]
    public void LoadFlushedCode_MissingTypePipeAssemblyAttribute ()
    {
      _manager.LoadFlushedCode (GetType().Assembly);
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

    [Test]
    public void DelegatingMembers_GuardedByLock ()
    {
      _codeGeneratorMock.Expect (mock => mock.AssemblyDirectory).Return ("get dir").WhenCalled (_ => CheckLock (true));
      Assert.That (_manager.AssemblyDirectory, Is.EqualTo ("get dir"));
      _codeGeneratorMock.Expect (mock => mock.AssemblyName).Return ("get name").WhenCalled (_ => CheckLock (true));
      Assert.That (_manager.AssemblyName, Is.EqualTo ("get name"));

      _codeGeneratorMock.Expect (mock => mock.SetAssemblyDirectory ("set dir")).WhenCalled (_ => CheckLock (true));
      _manager.SetAssemblyDirectory ("set dir");
      _codeGeneratorMock.Expect (mock => mock.SetAssemblyName ("set name")).WhenCalled (_ => CheckLock (true));
      _manager.SetAssemblyName ("set name");

      _codeGeneratorMock.VerifyAllExpectations();
      _typeCacheMock.VerifyAllExpectations();
    }

    private void CheckLock (bool lockIsHeld)
    {
      if (lockIsHeld)
        LockTestHelper.CheckLockIsHeld (_codeGeneratorLock);
      else
        LockTestHelper.CheckLockIsNotHeld (_codeGeneratorLock);
    }

    private _Assembly CreateAssemblyMock (string participantConfigurationID, params Type[] types)
    {
      var assemblyMock = MockRepository.GenerateStrictMock<_Assembly>();
      var assemblyAttribute = new TypePipeAssemblyAttribute (participantConfigurationID);
      assemblyMock.Expect (mock => mock.GetCustomAttributes (typeof (TypePipeAssemblyAttribute), false)).Return (new object[] { assemblyAttribute });
      assemblyMock.Expect (mock => mock.GetTypes()).Return (types);

      return assemblyMock;
    }
  }
}