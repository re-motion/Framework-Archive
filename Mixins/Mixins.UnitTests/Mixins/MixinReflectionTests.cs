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
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly());
      ApplicationDefinition applicationDefinition = DefinitionBuilder.CreateApplicationDefinition (context);

      using (new CurrentTypeFactoryScope (applicationDefinition))
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
        IMixinTarget mixinTarget = bt1 as IMixinTarget;
        Assert.IsNotNull (mixinTarget);

        BaseClassDefinition configuration = mixinTarget.Configuration;
        Assert.IsNotNull (configuration);

        Assert.AreSame (applicationDefinition.BaseClasses[typeof (BaseType1)], configuration);

        object[] mixins = mixinTarget.Mixins;
        Assert.IsNotNull (mixins);
        Assert.AreEqual (configuration.Mixins.Count, mixins.Length);
      }
    }

    [Test]
    public void GetInitializationMethod ()
    {
      using (new CurrentTypeFactoryScope (DefinitionBuilder.CreateApplicationDefinition (DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ()))))
      {
        MixinDefinition m1 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType1)].Mixins[typeof (BT1Mixin1)];
        Assert.IsNull (MixinReflector.GetInitializationMethod (m1.Type));

        MixinDefinition m2 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin1)];
        Assert.IsNotNull (MixinReflector.GetInitializationMethod (m2.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType31, IBaseType31>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetInitializationMethod (m2.Type));

        MixinDefinition m3 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
        Assert.IsNotNull (MixinReflector.GetInitializationMethod (m3.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType32>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetInitializationMethod (m3.Type));

        MixinDefinition m4 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
        Assert.IsNotNull (MixinReflector.GetInitializationMethod (m4.Type));
        Assert.AreNotEqual (typeof (Mixin<,>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetInitializationMethod (m4.Type));
        Assert.AreEqual (m4.Type.BaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetInitializationMethod (m4.Type));

        Assert.AreEqual (typeof (Mixin<IBaseType33, IBaseType33>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetInitializationMethod (m4.Type));
      }
    }

    [Test]
    public void GetTargetProperty ()
    {
      using (new CurrentTypeFactoryScope (DefinitionBuilder.CreateApplicationDefinition (DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ()))))
      {
        MixinDefinition m1 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType1)].Mixins[typeof (BT1Mixin1)];
        Assert.IsNull (MixinReflector.GetTargetProperty (m1.Type));

        MixinDefinition m2 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin1)];
        Assert.IsNotNull (MixinReflector.GetTargetProperty (m2.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetTargetProperty (m2.Type));

        MixinDefinition m3 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
        Assert.IsNotNull (MixinReflector.GetTargetProperty (m3.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetTargetProperty (m3.Type));

        MixinDefinition m4 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
        Assert.IsNotNull (MixinReflector.GetTargetProperty (m4.Type));
        Assert.AreNotEqual (
            typeof (Mixin<,>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetTargetProperty (m4.Type));
        Assert.AreEqual (
            m4.Type.BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetTargetProperty (m4.Type));

        Assert.AreEqual (typeof (Mixin<IBaseType33, IBaseType33>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetTargetProperty (m4.Type));
      }
    }

    [Test]
    public void GetBaseProperty ()
    {
      using (
          new CurrentTypeFactoryScope (
              DefinitionBuilder.CreateApplicationDefinition (DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ()))))
      {
        MixinDefinition m1 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType1)].Mixins[typeof (BT1Mixin1)];
        Assert.IsNull (MixinReflector.GetBaseProperty (m1.Type));

        MixinDefinition m2 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin1)];
        Assert.IsNotNull (MixinReflector.GetBaseProperty (m2.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetBaseProperty (m2.Type));

        MixinDefinition m3 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
        Assert.IsNull (MixinReflector.GetBaseProperty (m3.Type));

        MixinDefinition m4 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
        Assert.IsNotNull (MixinReflector.GetBaseProperty (m4.Type));
        Assert.AreNotEqual (
            typeof (Mixin<,>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetBaseProperty (m4.Type));
        Assert.AreEqual (
            m4.Type.BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetBaseProperty (m4.Type));
        Assert.AreEqual (typeof (Mixin<IBaseType33, IBaseType33>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            MixinReflector.GetBaseProperty(m4.Type));
      }
    }
  }
}