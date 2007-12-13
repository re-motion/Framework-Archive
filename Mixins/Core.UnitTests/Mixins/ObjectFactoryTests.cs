using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.Configuration.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.CodeGeneration;
using System.Runtime.Serialization;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ObjectFactoryTests : MixinBaseTest
  {
    [Test]
    public void MixedObjectsCanBeCreated ()
    {
      object o = ObjectFactory.Create<BaseType3> ().With();
      Assert.IsNotNull (o);
      Assert.IsTrue (o is BaseType3);
      Assert.IsTrue (o is IMixinTarget);

      Assert.IsNotNull (((IMixinTarget) o).Mixins[0]);
    }

    [Test]
    public void MixedObjectsCanBeCreatedFromType ()
    {
      object o = ObjectFactory.Create (typeof (BaseType3)).With ();
      Assert.IsNotNull (o);
      Assert.IsTrue (o is IMixinTarget);
      Assert.IsNotNull (((IMixinTarget) o).Mixins[0]);
    }

    [Test]
    public void MixedObjectsCanBeCreatedWithMixinInstances ()
    {
      BT1Mixin1 m1 = new BT1Mixin1 ();
      BaseType1 bt1 = ObjectFactory.CreateWithMixinInstances<BaseType1> (m1).With ();

      Assert.IsNotNull (Mixin.Get<BT1Mixin1> (bt1));
      Assert.AreSame (m1, Mixin.Get<BT1Mixin1> (bt1));
      Assert.IsNotNull (Mixin.Get<BT1Mixin2> (bt1));
      Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> (bt1));
    }

    [Test]
    public void MixedObjectsWithMixinInstancesCanBeCreatedFromType ()
    {
      BT1Mixin1 m1 = new BT1Mixin1 ();
      BaseType1 bt1 = (BaseType1) ObjectFactory.CreateWithMixinInstances (typeof (BaseType1), m1).With ();

      Assert.IsNotNull (Mixin.Get<BT1Mixin1> (bt1));
      Assert.AreSame (m1, Mixin.Get<BT1Mixin1> (bt1));
      Assert.IsNotNull (Mixin.Get<BT1Mixin2> (bt1));
      Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> (bt1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type .* is not valid in the current configuration.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnWrongMixinInstances ()
    {
      BT2Mixin1 m1 = new BT2Mixin1 ();
      ObjectFactory.CreateWithMixinInstances<BaseType3> (m1).With ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type .* is not valid in the current configuration.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnWrongMixinInstancesWithType ()
    {
      BT2Mixin1 m1 = new BT2Mixin1 ();
      ObjectFactory.CreateWithMixinInstances (typeof (BaseType3), m1).With ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin Rubicon.Mixins.UnitTests.SampleTypes.MixinWithProtectedOverrider "
        + "applied to base type Rubicon.Mixins.UnitTests.SampleTypes.BaseType1 needs to have a subclass generated at runtime. It is therefore not "
        + "possible to use the given object of type MixinWithProtectedOverrider as a mixin instance.", MatchType = MessageMatch.Contains)]
    public void ThrowsOnBaseMixinInstanceWhenGeneratedTypeIsNeeded ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (MixinWithProtectedOverrider)))
      {
        BaseType1 bt1 = ObjectFactory.CreateWithMixinInstances<BaseType1> (new MixinWithProtectedOverrider ()).With();
        bt1.VirtualMethod ();
      }
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType1 ()
    {
      TargetClassDefinition configuration = TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers));
      Type generatedMixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (configuration.Mixins[typeof (MixinWithAbstractMembers)]);
      object mixinInstance = Activator.CreateInstance (generatedMixinType);

      ClassOverridingMixinMembers classInstance = ObjectFactory.CreateWithMixinInstances<ClassOverridingMixinMembers> (mixinInstance).With ();
      Assert.AreSame (mixinInstance, Mixin.Get<MixinWithAbstractMembers> (classInstance));
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType2 ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (MixinWithProtectedOverrider)))
      {
        TargetClassDefinition configuration = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Type mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (configuration.Mixins[0]);
        object mixinInstance = Activator.CreateInstance (mixinType);
        BaseType1 bt1 = ObjectFactory.CreateWithMixinInstances<BaseType1> (mixinInstance).With ();
        bt1.VirtualMethod ();
        Assert.AreSame (mixinInstance, Mixin.Get<MixinWithProtectedOverrider> (bt1));
      }
    }

    [Test]
    public void MixinsAreInitializedWithTarget ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin2)))
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
      }
    }

    [Test]
    public void MixinsAreInitializedWithBase ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin1)))
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
        Assert.IsNotNull (mixin.Base);
        Assert.AreNotSame (bt3, mixin.Base);
      }
    }

    [Test]
    public void MixinsAreInitializedWithConfiguration ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin1)))
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
        Assert.IsNotNull (mixin.Base);
        Assert.AreNotSame (bt3, mixin.Base);
        Assert.AreSame (((IMixinTarget) bt3).Configuration.Mixins[typeof (BT3Mixin1)], mixin.Configuration);
      }
    }

    [Test]
    public void CompleteFaceInterfacesAddedByMixins ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin7Face), typeof (BT3Mixin4)))
      {
        ICBaseType3BT3Mixin4 complete = ObjectFactory.Create<BaseType3>().With() as ICBaseType3BT3Mixin4;

        Assert.IsNotNull (complete);
        Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType33) complete).IfcMethod());
        Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType34) complete).IfcMethod());
        Assert.AreEqual ("BaseType3.IfcMethod2", complete.IfcMethod2());
        Assert.AreEqual ("BaseType3.IfcMethod-BT3Mixin4.Foo", Mixin.Get<BT3Mixin7Face> (complete).InvokeThisMethods());
      }
    }

    [Test]
    public void CompleteFaceInterfacesAddedExplicitly ()
    {
      object complete = ObjectFactory.Create<BaseType6> ().With ();

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgument ()
    {
      ICBT6Mixin1 complete = ObjectFactory.Create<ICBT6Mixin1> ().With ();

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    public interface IEmptyInterface { }

    [Test]
    public void CompleteFaceInterfaceAddedImperativelyAsTypeArgument ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        MixinConfiguration.ActiveConfiguration.GetOrAddClassContext (typeof (BaseType6)).AddCompleteInterface (typeof (IEmptyInterface));
        MixinConfiguration.ActiveConfiguration.RegisterInterface (typeof (IEmptyInterface), typeof (BaseType6));

        IEmptyInterface complete = ObjectFactory.Create<IEmptyInterface> ().With ();

        Assert.IsNotNull (complete);
        Assert.IsTrue (complete is BaseType6);
        Assert.IsTrue (complete is IEmptyInterface);
      }
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgumentWithMixins ()
    {
      ICBT6Mixin1 complete = ObjectFactory.CreateWithMixinInstances<ICBT6Mixin1> (new BT6Mixin1 ()).With ();

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not been registered",
        MatchType = MessageMatch.Contains)]
    public void InterfaceAsTypeArgumentWithoutCompleteness ()
    {
      ObjectFactory.Create<IBaseType2> ().With ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not been registered",
        MatchType = MessageMatch.Regex)]
    public void InterfaceAsTypeArgumentWithoutCompletenessWithMixins ()
    {
      ObjectFactory.CreateWithMixinInstances<IBaseType2> ().With ();
    }

    [Test]
    public void MixinWithoutPublicCtor ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinWithPrivateCtorAndVirtualMethod)))
      {
        MixinWithPrivateCtorAndVirtualMethod mixin = MixinWithPrivateCtorAndVirtualMethod.Create ();
        object o = ObjectFactory.CreateWithMixinInstances<NullTarget> (mixin).With ();
        Assert.IsNotNull (o);
        Assert.IsNotNull (Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o));
        Assert.AreSame (mixin, Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o));
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Cannot instantiate mixin Rubicon.Mixins.UnitTests.Configuration."
        + "ValidationSampleTypes.MixinWithPrivateCtorAndVirtualMethod, there is no public default constructor.")]
    public void ThrowsWhenMixinWithoutPublicDefaultCtorShouldBeInstantiated ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinWithPrivateCtorAndVirtualMethod)))
      {
        ObjectFactory.CreateWithMixinInstances<NullTarget> ().With ();
      }
    }

    [Test]
    public void GenerationPolicyOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object), GenerationPolicy.GenerateOnlyIfConfigured).With();
      Assert.AreEqual (typeof (object), o.GetType ());

      o = ObjectFactory.Create<object> (GenerationPolicy.GenerateOnlyIfConfigured).With ();
      Assert.AreEqual (typeof (object), o.GetType ());
    }

    [Test]
    public void GenerationPolicyForce ()
    {
      object o = ObjectFactory.Create (typeof (object), GenerationPolicy.ForceGeneration).With ();
      Assert.AreNotEqual (typeof (object), o.GetType ());
      Assert.AreEqual (typeof (object), o.GetType ().BaseType);

      o = ObjectFactory.Create<object> (GenerationPolicy.ForceGeneration).With ();
      Assert.AreNotEqual (typeof (object), o.GetType ());
      Assert.AreEqual (typeof (object), o.GetType ().BaseType);
    }

    [Test]
    public void DefaultPolicyIsOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object)).With ();
      Assert.AreEqual (typeof (object), o.GetType ());

      o = ObjectFactory.Create<object> ().With ();
      Assert.AreEqual (typeof (object), o.GetType ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no mixin configuration for type System.Object, so no mixin instances "
        + "must be specified.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnMixinInstancesWhenNoGeneration ()
    {
      ObjectFactory.CreateWithMixinInstances (typeof (object), new object()).With ();
    }

    public class MixinThrowingInOnInitialized : Mixin<object>
    {
      protected override void OnInitialized ()
      {
        throw new NotSupportedException ();
      }
    }

    [Test]
    [ExpectedException (typeof (TargetInvocationException))]
    public void TargetInvocationExceptionWhenMixinOnInitializedThrows ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinThrowingInOnInitialized)))
      {
        ObjectFactory.Create<NullTarget> ().With ();
      }
    }

    public class MixinThrowingInCtor : Mixin<object>
    {
      public MixinThrowingInCtor()
      {
        throw new NotSupportedException ();
      }
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void TargetInvocationExceptionWhenMixinCtorThrows ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinThrowingInCtor)))
      {
        ObjectFactory.Create<NullTarget> ().With ();
      }
    }

    public class TargetClassWithProtectedCtors
    {
      protected TargetClassWithProtectedCtors ()
      {
      }

      protected TargetClassWithProtectedCtors (int i)
      {
      }
    }

    [Test]
    [Ignore ("TODO - COMMONS-414")]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Mixins.UnitTests.Mixins.ObjectFactoryTests+"
        + "TargetClassWithProtectedCtors does not contain a public constructor with signature ().")]
    public void ProtectedDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (TargetClassWithProtectedCtors), typeof (NullMixin)))
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> ().With ();
      }
    }

    [Test]
    [Ignore ("TODO - COMMONS-414")]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Mixins.UnitTests.Mixins.ObjectFactoryTests+"
        + "TargetClassWithProtectedCtors does not contain a public constructor with signature (System.Int32).")]
    public void ProtectedNonDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (TargetClassWithProtectedCtors), typeof (NullMixin)))
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> ().With (1);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Mixins.UnitTests.Mixins.ObjectFactoryTests+"
        + "TargetClassWithProtectedCtors does not contain a public constructor with signature ().")]
    public void ProtectedDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> ().With ();
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Rubicon.Mixins.UnitTests.Mixins.ObjectFactoryTests+"
        + "TargetClassWithProtectedCtors does not contain a public constructor with signature (System.Int32).")]
    public void ProtectedNonDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> ().With (1);
      }
    }
  }
}
