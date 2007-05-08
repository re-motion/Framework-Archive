using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.CodeGeneration;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ReflectionUtilityTests
  {
    class GenericBase<T>
    {
      public virtual void Foo() { }
    }

    class GenericSub<T> : GenericBase<T>
    {
      public override void Foo () { }
    }

    class NonGenericSub : GenericSub<int>
    {
      public new void Foo () { }
    }

    [Test]
    public void FindGenericTypeDefinitionInClosedHierarchy()
    {
      Assert.AreEqual (typeof (GenericBase<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericBase<>),
        typeof (GenericBase<int>)));
      Assert.AreEqual (typeof (GenericBase<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericBase<>),
        typeof (GenericSub<int>)));
      Assert.AreEqual (typeof (GenericBase<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericBase<>),
        typeof (NonGenericSub)));

      Assert.AreEqual (typeof (GenericSub<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericSub<>),
        typeof (NonGenericSub)));
    }

    [Test]
    public void MapMethodInfoOfGenericTypeDefinitionToClosedHierarchy ()
    {
      Assert.AreEqual (typeof (GenericBase<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName(typeof (GenericBase<>).GetMethod ("Foo"),
          typeof (GenericBase<int>)));
      Assert.AreEqual (typeof (GenericBase<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericBase<>).GetMethod ("Foo"),
          typeof (GenericSub<int>)));
      Assert.AreEqual (typeof (GenericBase<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericBase<>).GetMethod ("Foo"),
          typeof (NonGenericSub)));

      Assert.AreEqual (typeof (GenericSub<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericSub<>).GetMethod ("Foo"),
          typeof (GenericSub<int>)));
      Assert.IsNull (ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericSub<>).GetMethod ("Foo"),
          typeof (GenericBase<int>)));
    }

    [Test]
    public void GetInitializationMethod ()
    {
      using (new CurrentTypeFactoryScope (DefinitionBuilder.CreateApplicationDefinition (DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ()))))
      {
        MixinDefinition m1 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType1)].Mixins[typeof (BT1Mixin1)];
        Assert.IsNull (ReflectionUtility.GetInitializationMethod (m1.Type));

        MixinDefinition m2 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin1)];
        Assert.IsNotNull (ReflectionUtility.GetInitializationMethod (m2.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType31, IBaseType31>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetInitializationMethod (m2.Type));

        MixinDefinition m3 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
        Assert.IsNotNull (ReflectionUtility.GetInitializationMethod (m3.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType32>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetInitializationMethod (m3.Type));

        MixinDefinition m4 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin3<,>)];
        Assert.IsNotNull (ReflectionUtility.GetInitializationMethod (m4.Type));
        Assert.AreNotEqual (
            typeof (Mixin<,>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetInitializationMethod (m4.Type));
        Assert.AreEqual (
            m4.Type.BaseType.GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetInitializationMethod (m4.Type));

        Type concreteType = m4.Type.MakeGenericType (typeof (IBaseType33), typeof (IBaseType33));
        Assert.IsNotNull (ReflectionUtility.GetInitializationMethod (concreteType));
        Assert.AreEqual (
            typeof (Mixin<IBaseType33, IBaseType33>).GetMethod ("Initialize", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetInitializationMethod (concreteType));
      }
    }

    [Test]
    public void GetTargetProperty ()
    {
      using (new CurrentTypeFactoryScope (DefinitionBuilder.CreateApplicationDefinition (DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ()))))
      {
        MixinDefinition m1 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType1)].Mixins[typeof (BT1Mixin1)];
        Assert.IsNull (ReflectionUtility.GetTargetProperty (m1.Type));

        MixinDefinition m2 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin1)];
        Assert.IsNotNull (ReflectionUtility.GetTargetProperty (m2.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetTargetProperty (m2.Type));

        MixinDefinition m3 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
        Assert.IsNotNull (ReflectionUtility.GetTargetProperty (m3.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetTargetProperty (m3.Type));

        MixinDefinition m4 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin3<,>)];
        Assert.IsNotNull (ReflectionUtility.GetTargetProperty (m4.Type));
        Assert.AreNotEqual (
            typeof (Mixin<,>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetTargetProperty (m4.Type));
        Assert.AreEqual (
            m4.Type.BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetTargetProperty (m4.Type));

        Type concreteType = m4.Type.MakeGenericType (typeof (IBaseType33), typeof (IBaseType33));
        Assert.IsNotNull (ReflectionUtility.GetTargetProperty (concreteType));
        Assert.AreEqual (
            typeof (Mixin<IBaseType33, IBaseType33>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetTargetProperty (concreteType));
      }
    }

    [Test]
    public void GetBaseProperty ()
    {
      using (
          new CurrentTypeFactoryScope (
              DefinitionBuilder.CreateApplicationDefinition (DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly()))))
      {
        MixinDefinition m1 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType1)].Mixins[typeof (BT1Mixin1)];
        Assert.IsNull (ReflectionUtility.GetBaseProperty (m1.Type));

        MixinDefinition m2 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin1)];
        Assert.IsNotNull (ReflectionUtility.GetBaseProperty (m2.Type));
        Assert.AreEqual (
            typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetBaseProperty (m2.Type));

        MixinDefinition m3 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin2)];
        Assert.IsNull (ReflectionUtility.GetBaseProperty (m3.Type));

        MixinDefinition m4 = TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin3<,>)];
        Assert.IsNotNull (ReflectionUtility.GetBaseProperty (m4.Type));
        Assert.AreNotEqual (
            typeof (Mixin<,>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetBaseProperty (m4.Type));
        Assert.AreEqual (
            m4.Type.BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetBaseProperty (m4.Type));

        Type concreteType = m4.Type.MakeGenericType (typeof (IBaseType33), typeof (IBaseType33));
        Assert.IsNotNull (ReflectionUtility.GetBaseProperty (concreteType));
        Assert.AreEqual (
            typeof (Mixin<IBaseType33, IBaseType33>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance),
            ReflectionUtility.GetBaseProperty (concreteType));
      }
    }
  }
}
