// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class MixinTypeGeneratorTest
  {
    private IModuleManager _moduleMock;
    private IClassEmitter _classEmitterMock;
    private ITypeGenerator _typeGeneratorMock;

    private TargetClassDefinition _simpleClassDefinition;
    private MixinDefinition _simpleMixinDefinition;
    private MixinDefinition _signedMixinDefinition;
    private MixinTypeGenerator _mixinTypeGenerator;

    [SetUp]
    public void SetUp ()
    {
      _moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      _classEmitterMock = MockRepository.GenerateMock<IClassEmitter> ();
      _moduleMock.Stub (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Anything)).Return (_classEmitterMock);
      _typeGeneratorMock = MockRepository.GenerateMock<ITypeGenerator> ();

      _simpleClassDefinition = new TargetClassDefinition (new ClassContext (typeof (BaseType1), typeof (BT1Mixin1)));
      _simpleMixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (BT1Mixin1), _simpleClassDefinition, false);
      _signedMixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (object), _simpleClassDefinition, false);
      PrivateInvoke.InvokeNonPublicMethod (_simpleClassDefinition.Mixins, "Add", _simpleMixinDefinition);

      _mixinTypeGenerator = new MockRepository().PartialMock<MixinTypeGenerator> (_moduleMock, _typeGeneratorMock, _simpleMixinDefinition, GuidNameProvider.Instance);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mixinTypeGenerator.Configuration, Is.SameAs (_simpleMixinDefinition));
      Assert.That (_mixinTypeGenerator.Emitter, Is.Not.Null);
      Assert.That (_mixinTypeGenerator.Emitter, Is.SameAs (_classEmitterMock));
    }

    [Test]
    public void Initialization_ForceUnsignedFalse ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (_signedMixinDefinition.Type.Assembly), Is.True);

      IModuleManager moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      moduleMock.Expect (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (false)))
              .Return (_classEmitterMock);

      _typeGeneratorMock.Stub (mock => mock.IsAssemblySigned).Return (true);
      new MixinTypeGenerator (moduleMock, _typeGeneratorMock, _signedMixinDefinition, GuidNameProvider.Instance);

      moduleMock.VerifyAllExpectations ();
    }

    [Test]
    public void Initialization_ForceUnsignedTrue_BecauseOfTargetType ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (_signedMixinDefinition.Type.Assembly), Is.True);

      IModuleManager moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      moduleMock.Expect (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (true)))
              .Return (_classEmitterMock);

      _typeGeneratorMock.Stub (mock => mock.IsAssemblySigned).Return (false);
      new MixinTypeGenerator (moduleMock, _typeGeneratorMock, _signedMixinDefinition, GuidNameProvider.Instance);

      moduleMock.VerifyAllExpectations();
    }

    [Test]
    public void GetBuiltType_Type ()
    {
      DisableGenerate();

      _classEmitterMock.Stub (mock => mock.BuildType()).Return (typeof (string));
      ConcreteMixinType mixinType = _mixinTypeGenerator.GetBuiltType();
      Assert.That (mixinType.GeneratedType, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetBuiltType_ReturnsWrappersForAllProtectedOverriders ()
    {
      DisableGenerate ();

      var finalizeMethodInfo = typeof (object).GetMethod ("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);
      var protectedOverrider = new MethodDefinition (finalizeMethodInfo, _simpleMixinDefinition);
      var overridden = new MethodDefinition (finalizeMethodInfo, _simpleClassDefinition);

      PrivateInvoke.SetPublicProperty (protectedOverrider, "Base", overridden);
      PrivateInvoke.InvokeNonPublicMethod (_simpleMixinDefinition.Methods, "Add", protectedOverrider);
      PrivateInvoke.InvokeNonPublicMethod (_simpleClassDefinition.Methods, "Add", overridden);

      var fakeMethodWrapper = typeof (DateTime).GetMethod ("get_Now");
      _classEmitterMock.Stub (mock => mock.GetPublicMethodWrapper (finalizeMethodInfo)).Return (fakeMethodWrapper);
      _classEmitterMock.Stub (mock => mock.BuildType ()).Return (typeof (string));

      var result = _mixinTypeGenerator.GetBuiltType ();
      Assert.That (result.GetMethodWrapper (finalizeMethodInfo), Is.SameAs (fakeMethodWrapper));
    }

    private void DisableGenerate ()
    {
      _mixinTypeGenerator.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "Generate"));
    }
  }
}
