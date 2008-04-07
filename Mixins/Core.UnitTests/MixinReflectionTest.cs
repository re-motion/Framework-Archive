using System;
using System.Reflection;
using Castle.DynamicProxy;
using Remotion.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.SampleTypes;
using Remotion.Mixins.Utilities;
using NUnit.Framework;

namespace Remotion.Mixins.UnitTests
{
  [TestFixture]
  public class MixinReflectionTest
  {
    [Test]
    public void FindMixinInstanceInTarget()
    {
      BaseType3 bt3 = ObjectFactory.Create<BaseType3>().With();
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> ((object) bt3);
      Assert.IsNotNull (mixin);
    }

    [Test]
    public void NullIfMixinNotFound()
    {
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (new object());
      Assert.IsNull (mixin);
    }

    [Test]
    public void IMixinTarget()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      using (context.EnterScope())
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
        IMixinTarget mixinTarget = bt1 as IMixinTarget;
        Assert.IsNotNull (mixinTarget);

        TargetClassDefinition configuration = mixinTarget.Configuration;
        Assert.IsNotNull (configuration);

        Assert.AreSame (TypeFactory.GetActiveConfiguration (typeof (BaseType1)), configuration);

        object[] mixins = mixinTarget.Mixins;
        Assert.IsNotNull (mixins);
        Assert.AreEqual (configuration.Mixins.Count, mixins.Length);
      }
    }

    [Test]
    public void GetInitializationMethod ()
    {
      MixinDefinition m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetInitializationMethod (m1.Type, MixinReflector.InitializationMode.Construction));

      MixinDefinition m2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreEqual (typeof (Mixin<IBaseType31, IBaseType31>).GetMethod ("Initialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Construction));

      MixinDefinition m3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreEqual (
          typeof (Mixin<IBaseType32>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Construction));

      MixinDefinition m4 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreNotEqual (typeof (Mixin<,>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));
      Assert.AreEqual (m4.Type.BaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));

      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetMethod ("Initialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Construction));
    }

    [Test]
    public void GetDeserializationMethod ()
    {
      MixinDefinition m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetInitializationMethod (m1.Type, MixinReflector.InitializationMode.Deserialization));

      MixinDefinition m2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreEqual (typeof (Mixin<IBaseType31, IBaseType31>).GetMethod ("Deserialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m2.Type, MixinReflector.InitializationMode.Deserialization));

      MixinDefinition m3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreEqual (
          typeof (Mixin<IBaseType32>).GetMethod ("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetInitializationMethod (m3.Type, MixinReflector.InitializationMode.Deserialization));

      MixinDefinition m4 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreNotEqual (typeof (Mixin<,>).GetMethod ("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));
      Assert.AreEqual (m4.Type.BaseType.GetMethod ("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
          MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));

      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetMethod ("Deserialize",
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m4.Type, MixinReflector.InitializationMode.Deserialization));
    }

    [Test]
    public void GetTargetProperty ()
    {
      MixinDefinition m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetTargetProperty (m1.Type));

      MixinDefinition m2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetTargetProperty (m2.Type));
      Assert.AreEqual (
          typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m2.Type));

      MixinDefinition m3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNotNull (MixinReflector.GetTargetProperty (m3.Type));
      Assert.AreEqual (
          typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m3.Type));

      MixinDefinition m4 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetTargetProperty (m4.Type));
      Assert.AreNotEqual (
          typeof (Mixin<,>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m4.Type));
      Assert.AreEqual (
          m4.Type.BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m4.Type));

      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetTargetProperty (m4.Type));
    }

    [Test]
    public void GetBaseProperty ()
    {
      MixinDefinition m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetBaseProperty (m1.Type));

      MixinDefinition m2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetBaseProperty (m2.Type));
      Assert.AreEqual (
          typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty (m2.Type));

      MixinDefinition m3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNull (MixinReflector.GetBaseProperty (m3.Type));

      MixinDefinition m4 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetBaseProperty (m4.Type));
      Assert.AreNotEqual (
          typeof (Mixin<,>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty (m4.Type));
      Assert.AreEqual (
          m4.Type.BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty (m4.Type));
      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetBaseProperty(m4.Type));
    }

    [Test]
    public void GetConfigurationProperty ()
    {
      MixinDefinition m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.IsNull (MixinReflector.GetConfigurationProperty (m1.Type));

      MixinDefinition m2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
      Assert.IsNotNull (MixinReflector.GetConfigurationProperty (m2.Type));
      Assert.AreEqual (
          typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Configuration", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetConfigurationProperty (m2.Type));

      MixinDefinition m3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsNotNull (MixinReflector.GetConfigurationProperty (m3.Type));

      MixinDefinition m4 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.IsNotNull (MixinReflector.GetConfigurationProperty (m4.Type));
      Assert.AreNotEqual (
          typeof (Mixin<,>).GetProperty ("Configuration", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetConfigurationProperty (m4.Type));
      Assert.AreEqual (
          m4.Type.BaseType.GetProperty ("Configuration", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetConfigurationProperty (m4.Type));
      Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("Configuration", BindingFlags.NonPublic | BindingFlags.Instance),
          MixinReflector.GetConfigurationProperty (m4.Type));
    }

    [Test]
    public void GetMixinBaseCallProxyType()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With ();
      Type bcpt = MixinReflector.GetBaseCallProxyType (bt1);
      Assert.IsNotNull (bcpt);
      Assert.AreEqual (bt1.GetType ().GetNestedType ("BaseCallProxy"), bcpt);
    }

    [Test]
    [ExpectedException(typeof (ArgumentException), ExpectedMessage = "not a mixin target", MatchType = MessageMatch.Contains)]
    public void GetMixinBaseCallProxyTypeThrowsIfWrongType1 ()
    {
      MixinReflector.GetBaseCallProxyType (new object());
    }

    [Test]
    public void GetMixinConfigurationFromConcreteType ()
    {
      Type bt1Type = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreEqual (TypeFactory.GetActiveConfiguration (typeof (BaseType1)).ConfigurationContext,
          Mixin.GetMixinConfigurationFromConcreteType (bt1Type));
    }

    [Test]
    public void GetMixinConfigurationFromConcreteTypeNullWhenNoMixedType ()
    {
      Assert.IsNull (Mixin.GetMixinConfigurationFromConcreteType (typeof (object)));
    }

    [Test]
    public void GetMixinConfigurationFromDerivedConcreteType ()
    {
      Type concreteType = TypeUtility.GetConcreteMixedType (typeof (BaseType1));
      CustomClassEmitter customClassEmitter = new CustomClassEmitter (new ModuleScope (false), "Test", concreteType);
      Type derivedType = customClassEmitter.BuildType ();
      Assert.AreEqual (TypeFactory.GetActiveConfiguration (typeof (BaseType1)).ConfigurationContext,
          Mixin.GetMixinConfigurationFromConcreteType (derivedType));
    }
  }
}