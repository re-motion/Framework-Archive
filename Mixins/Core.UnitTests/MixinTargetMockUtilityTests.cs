using System;
using Castle.DynamicProxy;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.UnitTests
{
  [TestFixture]
  public class MixinTargetMockUtilityTests
  {
    [SetUp]
    public void SetUp ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      // ensure compatibility with Rhino.Mocks
      ((Rubicon.Mixins.CodeGeneration.DynamicProxy.ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope = new ModuleScope (false);
    }

    [TearDown]
    public void TearDown ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
    }

    [Test]
    public void Mock_ThisBaseConfig ()
    {
      MockRepository repository = new MockRepository();

      IBaseType31 thisMock = repository.CreateMock<IBaseType31>();
      IBaseType31 baseMock = repository.CreateMock<IBaseType31>();
      TargetClassDefinition targetClassConfiguration = new TargetClassDefinition (new ClassContext (typeof (BaseType3)));
      MixinDefinition mixinConfiguration = new MixinDefinition (typeof (BT3Mixin1), targetClassConfiguration, false);

      BT3Mixin1 mixin = new BT3Mixin1();

      MixinTargetMockUtility.MockMixinTarget (mixin, thisMock, baseMock, mixinConfiguration);

      Assert.AreSame (thisMock, mixin.This);
      Assert.AreSame (baseMock, mixin.Base);
      Assert.AreSame (mixinConfiguration, mixin.Configuration);
    }

    [Test]
    public void Mock_ThisConfig ()
    {
      MockRepository repository = new MockRepository ();

      IBaseType32 thisMock = repository.CreateMock<IBaseType32> ();
      TargetClassDefinition targetClassConfiguration = new TargetClassDefinition (new ClassContext (typeof (BaseType3)));
      MixinDefinition mixinConfiguration = new MixinDefinition (typeof (BT3Mixin1), targetClassConfiguration, false);

      BT3Mixin2 mixin = new BT3Mixin2 ();

      MixinTargetMockUtility.MockMixinTarget (mixin, thisMock, mixinConfiguration);

      Assert.AreSame (thisMock, mixin.This);
      Assert.AreSame (mixinConfiguration, mixin.Configuration);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_This ()
    {
      BT3Mixin1 mixin = new BT3Mixin1 ();
      Dev.Null = mixin.This;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_Base ()
    {
      BT3Mixin1 mixin = new BT3Mixin1 ();
      Dev.Null = mixin.Base;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixinConfiguration ()
    {
      BT3Mixin1 mixin = new BT3Mixin1 ();
      Dev.Null = mixin.Configuration;
    }

    [Test]
    public void CreateMixinWithMockedTarget_ThisBase_WithGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      ClassOverridingMixinMembers thisMock = repository.CreateMock<ClassOverridingMixinMembers>();
      object baseMock = new object();

      Expect.Call (thisMock.AbstractMethod (25)).Return ("Mocked!");

      repository.ReplayAll();

      MixinWithAbstractMembers mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<MixinWithAbstractMembers, object, object> (thisMock, baseMock);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedMethod-Mocked!", mixin.ImplementedMethod ());

      repository.VerifyAll ();
    }

    [Test]
    public void CreateMixinWithMockedTarget_This_WithGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      ClassOverridingSpecificMixinMember thisMock = repository.CreateMock<ClassOverridingSpecificMixinMember> ();

      Expect.Call (thisMock.VirtualMethod ()).Return ("Mocked, bastard!");

      repository.ReplayAll ();

      MixinWithVirtualMethod mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<MixinWithVirtualMethod, object> (thisMock);
      Assert.AreEqual ("Mocked, bastard!", mixin.VirtualMethod ());

      repository.VerifyAll ();
    }

    [Test]
    public void CreateMixinWithMockedTarget_ThisBase_WithNonGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      IBaseType31 thisMock = repository.CreateMock<IBaseType31> ();
      IBaseType31 baseMock = repository.CreateMock<IBaseType31> ();

      BT3Mixin1 mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31> (thisMock, baseMock);
      Assert.AreSame (thisMock, mixin.This);
      Assert.AreSame (baseMock, mixin.Base);
    }

    [Test]
    public void CreateMixinWithMockedTarget_This_WithNonGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      IBaseType32 thisMock = repository.CreateMock<IBaseType32> ();

      BT3Mixin2 mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock);
      Assert.AreSame (thisMock, mixin.This);
    }
  }
}