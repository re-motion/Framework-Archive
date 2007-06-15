using System;
using System.Reflection;
using Mixins.CodeGeneration;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using Mixins.Utilities;
using NUnit.Framework;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class MixinReflectionTests: MixinTestBase
  {
    [Test]
    public void FindMixinInstanceInTarget()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin2)).With();
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
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());

      using (new MixinConfiguration (context))
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
        IMixinTarget mixinTarget = bt1 as IMixinTarget;
        Assert.IsNotNull (mixinTarget);

        BaseClassDefinition configuration = mixinTarget.Configuration;
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
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        MixinDefinition m1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
        Assert.IsNull (MixinReflector.GetInitializationMethod (m1.Type));

        MixinDefinition m2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
        Assert.IsNotNull (MixinReflector.GetInitializationMethod (m2.Type));
        Assert.AreEqual (typeof (Mixin<IBaseType31, IBaseType31>).GetMethod ("Initialize",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m2.Type));

        MixinDefinition m3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
        Assert.IsNotNull (MixinReflector.GetInitializationMethod (m3.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType32>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetInitializationMethod (m3.Type));

        MixinDefinition m4 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
        Assert.IsNotNull (MixinReflector.GetInitializationMethod (m4.Type));
        Assert.AreNotEqual (typeof (Mixin<,>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            MixinReflector.GetInitializationMethod (m4.Type));
        Assert.AreEqual (m4.Type.BaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            MixinReflector.GetInitializationMethod (m4.Type));

        Assert.AreEqual (typeof (Mixin<BaseType3, IBaseType33>).GetMethod ("Initialize",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), MixinReflector.GetInitializationMethod (m4.Type));
      }
    }

    [Test]
    public void GetTargetProperty ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
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
    }

    [Test]
    public void GetBaseProperty ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
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
    }

    [Test]
    public void GetConfigurationProperty ()
    {
      using (
          new MixinConfiguration (Assembly.GetExecutingAssembly ()))
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
    }

    [Test]
    public void GetMixinBaseCallProxyType()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> ().With ();
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
  }
}