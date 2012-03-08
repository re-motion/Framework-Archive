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
using System.Reflection;
using NUnit.Framework;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.BuilderAbstractions;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class ReflectionEmitTypeModifierTest
  {
    private IModuleBuilder _moduleBuilderMock;
    private ISubclassProxyNameProvider _subclassProxyNameProviderMock;
    private ITypeBuilder _typeBuilderMock;
    private ReflectionEmitTypeModifier _reflectionEmitTypeModifier;
    private ITypeTemplate _typeTemplateMock;
    private MutableType _mutableType;
    private Type _requestedType;

    [SetUp]
    public void SetUp ()
    {
      _moduleBuilderMock = MockRepository.GenerateStrictMock<IModuleBuilder>();
      _subclassProxyNameProviderMock = MockRepository.GenerateStrictMock<ISubclassProxyNameProvider> ();
      _typeBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder> ();
      _reflectionEmitTypeModifier = new ReflectionEmitTypeModifier (_moduleBuilderMock, _subclassProxyNameProviderMock);
      _requestedType = typeof (RequestedType);
      _typeTemplateMock = MockRepository.GenerateStrictMock<ITypeTemplate> ();
      _mutableType = new MutableType (_requestedType, _typeTemplateMock);
    }

    [TearDown]
    public void TearDown ()
    {
      //_moduleBuilderMock.VerifyAllExpectations ();
      //_subclassProxyNameProviderMock.VerifyAllExpectations ();
      //_typeBuilderMock.VerifyAllExpectations ();
      //_typeTemplateMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateMutableType ()
    {
      var mutableType = _reflectionEmitTypeModifier.CreateMutableType (_requestedType);

      Assert.That (mutableType.RequestedType, Is.SameAs (_requestedType));
    }

    [Test]
    public void ApplyModifications_NoModifications ()
    {
      var fakeResultType = typeof (string);

      _subclassProxyNameProviderMock.Expect (mock => mock.GetSubclassProxyName (_requestedType)).Return ("foofoo");
      
      var typeBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder>();
      _moduleBuilderMock
          .Expect (mock => mock.DefineType ("foofoo", TypeAttributes.Public | TypeAttributes.BeforeFieldInit, _requestedType, Type.EmptyTypes))
          .Return (typeBuilderMock);
      typeBuilderMock.Expect (mock => mock.CreateType()).Return (fakeResultType);
      
      var result = _reflectionEmitTypeModifier.ApplyModifications (_mutableType);

      Assert.That (result, Is.SameAs (fakeResultType));
    }

    [Test]
    public void ApplyModifications_Interfaces ()
    {
      _typeTemplateMock.Expect (mock => mock.GetInterfaces ()).Return (Type.EmptyTypes);
      _mutableType.AddInterface (typeof (IDisposable));

      var fakeResultType = typeof (string);

      _subclassProxyNameProviderMock.Expect (mock => mock.GetSubclassProxyName (_requestedType)).Return ("foofoo");
      _moduleBuilderMock
          .Expect (
              mock =>
              mock.DefineType ("foofoo", TypeAttributes.Public | TypeAttributes.BeforeFieldInit, _requestedType, new[] { typeof (IDisposable) }))
          .Return (_typeBuilderMock);
      _typeBuilderMock.Expect (mock => mock.CreateType ()).Return (fakeResultType);

      var result = _reflectionEmitTypeModifier.ApplyModifications (_mutableType);

      Assert.That (result, Is.SameAs (fakeResultType));
    }

    [Test]
    public void ApplyModifications_AddField ()
    {
      _typeTemplateMock.Expect (mock => mock.GetFields (Arg<BindingFlags>.Is.Anything)).Return (new FieldInfo[0]);
      _mutableType.AddField ("_newField", typeof (string), FieldAttributes.Private);

      var fakeResultType = typeof (string);

      _subclassProxyNameProviderMock.Expect (mock => mock.GetSubclassProxyName (_requestedType)).Return ("foofoo");
      _moduleBuilderMock
          .Expect (
              mock =>
              mock.DefineType ("foofoo", TypeAttributes.Public | TypeAttributes.BeforeFieldInit, _requestedType, new Type[0]))
          .Return (_typeBuilderMock);
      _typeBuilderMock.Expect (mock => mock.DefineField ("_newField", typeof (string), FieldAttributes.Private));
      _typeBuilderMock.Expect (mock => mock.CreateType ()).Return (fakeResultType);

      var result = _reflectionEmitTypeModifier.ApplyModifications (_mutableType);

      Assert.That (result, Is.SameAs (fakeResultType));
    }

    public class RequestedType
    {
    }
  }
}