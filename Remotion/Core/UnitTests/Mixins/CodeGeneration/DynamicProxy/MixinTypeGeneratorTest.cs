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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Samples;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.Definitions.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class MixinTypeGeneratorTest
  {
    private IModuleManager _moduleMock;
    private IClassEmitter _classEmitterMock;

    private TargetClassDefinition _simpleClassDefinition;
    private MixinDefinition _simpleMixinDefinition;
    private MixinDefinition _signedMixinDefinition;

    private MethodInfo _publicOverrider;
    private MethodInfo _protectedOverrider;
    private MethodInfo _protectedInternalOverrider;

    private ConcreteMixinTypeIdentifier _identifier;
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

      _simpleClassDefinition = new TargetClassDefinition (new ClassContext (typeof (BaseType1), typeof (BT1Mixin1)));
      _simpleMixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (BT1Mixin1), _simpleClassDefinition, false);
      _signedMixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (object), _simpleClassDefinition, false);
      PrivateInvoke.InvokeNonPublicMethod (_simpleClassDefinition.Mixins, "Add", _simpleMixinDefinition);

      _publicOverrider = typeof (ClassWithDifferentMemberVisibilities).GetMethod ("PublicMethod");
      _protectedOverrider = typeof (ClassWithDifferentMemberVisibilities).GetMethod (
          "ProtectedMethod", 
          BindingFlags.NonPublic | BindingFlags.Instance);
      _protectedInternalOverrider = typeof (ClassWithDifferentMemberVisibilities).GetMethod (
          "ProtectedInternalMethod",
          BindingFlags.NonPublic | BindingFlags.Instance);

      _identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1), 
          new HashSet<MethodInfo> { _publicOverrider, _protectedOverrider, _protectedInternalOverrider },
          new HashSet<MethodInfo> ());

      _mixinTypeGenerator = new MockRepository().PartialMock<MixinTypeGenerator> (_moduleMock, _simpleMixinDefinition, _identifier, GuidNameProvider.Instance);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mixinTypeGenerator.Identifier, Is.SameAs (_identifier));
      Assert.That (_mixinTypeGenerator.Emitter, Is.Not.Null);
      Assert.That (_mixinTypeGenerator.Emitter, Is.SameAs (_classEmitterMock));
    }

    [Test]
    public void Initialization_ForceUnsignedFalse ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      Assert.That (ReflectionUtility.IsAssemblySigned (identifier.MixinType.Assembly), Is.True);

      var moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      moduleMock.Expect (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (false)))
              .Return (_classEmitterMock);

      new MixinTypeGenerator (moduleMock, _signedMixinDefinition, identifier, GuidNameProvider.Instance);

      moduleMock.VerifyAllExpectations ();
    }

    [Test]
    public void Initialization_ForceUnsignedTrue_BecauseOfMixinType ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (typeof (BT1Mixin1), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      Assert.That (ReflectionUtility.IsAssemblySigned (identifier.MixinType.Assembly), Is.False);

      var moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      moduleMock.Expect (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (true)))
              .Return (_classEmitterMock);

      new MixinTypeGenerator (moduleMock, _simpleMixinDefinition, identifier, GuidNameProvider.Instance);

      moduleMock.VerifyAllExpectations ();
    }

    [Test]
    public void Initialization_ForceUnsignedTrue_BecauseOfGenericTypeArgument ()
    {
      var mixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (EquatableMixin<BaseType1>), _simpleClassDefinition, false);
      Assert.That (ReflectionUtility.IsAssemblySigned (mixinDefinition.Type.Assembly), Is.True);
      Assert.That (ReflectionUtility.IsAssemblySigned (mixinDefinition.Type.GetGenericArguments()[0].Assembly), Is.False);
      var identifier = new ConcreteMixinTypeIdentifier (typeof (EquatableMixin<BaseType1>), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      var moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      moduleMock.Expect (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (true)))
              .Return (_classEmitterMock);

      new MixinTypeGenerator (moduleMock, mixinDefinition, identifier, GuidNameProvider.Instance);

      moduleMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetBuiltType_Type ()
    {
      DisableGenerate();
      _mixinTypeGenerator.Replay ();

      _classEmitterMock.Stub (mock => mock.BuildType()).Return (typeof (string));
      ConcreteMixinType mixinType = _mixinTypeGenerator.GetBuiltType();
      Assert.That (mixinType.GeneratedType, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetBuiltType_ReturnsWrappersForAllProtectedOverriders ()
    {
      StubGenerateTypeFeatures ();
      StubGenerateOverrides (typeof (string));
      _mixinTypeGenerator.Replay ();

      var fakeMethodWrapper1 = typeof (DateTime).GetMethod ("get_Now");
      var fakeMethodWrapper2 = typeof (DateTime).GetMethod ("get_Date");
      _classEmitterMock.Expect (mock => mock.GetPublicMethodWrapper (_protectedOverrider)).Return (fakeMethodWrapper1);
      _classEmitterMock.Expect (mock => mock.GetPublicMethodWrapper (_protectedInternalOverrider)).Return (fakeMethodWrapper2);
      _classEmitterMock.Expect (mock => mock.BuildType ()).Return (typeof (string));
      _classEmitterMock.Replay ();

      var result = _mixinTypeGenerator.GetBuiltType ();
      _classEmitterMock.VerifyAllExpectations ();

      Assert.That (result.GetMethodWrapper (_protectedOverrider), Is.SameAs (fakeMethodWrapper1));
      Assert.That (result.GetMethodWrapper (_protectedInternalOverrider), Is.SameAs (fakeMethodWrapper2));
    }

    [Test]
    public void GetBuiltType_OverrideInterface ()
    {
      StubGenerateTypeFeatures ();
      StubGenerateOverrides (typeof (int));
      StubGenerateMethodWrappers (new Tuple<MethodInfo, MethodInfo>[0]);
      _mixinTypeGenerator.Replay ();

      _classEmitterMock.Stub (mock => mock.BuildType ()).Return (typeof (string));

      ConcreteMixinType mixinType = _mixinTypeGenerator.GetBuiltType ();
      Assert.That (mixinType.GeneratedOverrideInterface, Is.SameAs (typeof (int)));
    }

    [Test]
    public void GetBuiltType_Identifier ()
    {
      StubGenerateTypeFeatures ();
      StubGenerateOverrides (typeof (int));
      StubGenerateMethodWrappers (new Tuple<MethodInfo, MethodInfo>[0]);
      _mixinTypeGenerator.Replay ();

      _classEmitterMock.Stub (mock => mock.BuildType ()).Return (typeof (string));

      ConcreteMixinType mixinType = _mixinTypeGenerator.GetBuiltType ();
      Assert.That (mixinType.Identifier, Is.EqualTo (_identifier));
    }

    private void DisableGenerate ()
    {
      StubGenerateTypeFeatures();
      StubGenerateOverrides (typeof (string));
      StubGenerateMethodWrappers (new Tuple<MethodInfo, MethodInfo>[0]);
    }

    private void StubGenerateOverrides (Type overrideInterfaceType)
    {
      var fakeEmitter = MockRepository.GenerateStub<IClassEmitter> ();
      fakeEmitter.Stub (stub => stub.BuildType ()).Return (overrideInterfaceType);
      var fakeOverrideInterfaceGenerator = new OverrideInterfaceGenerator (fakeEmitter);

      _mixinTypeGenerator.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GenerateOverrides")).Return (fakeOverrideInterfaceGenerator);
    }

    private void StubGenerateTypeFeatures ()
    {
      _mixinTypeGenerator.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GenerateTypeFeatures"));
    }

    private void StubGenerateMethodWrappers (Tuple<MethodInfo, MethodInfo>[] wrappers)
    {
      _mixinTypeGenerator.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GenerateMethodWrappers")).Return (wrappers);
    }
  }
}
