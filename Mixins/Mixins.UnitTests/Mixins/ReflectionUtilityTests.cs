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
using Rubicon;
using Rubicon.Utilities;
using ReflectionUtility=Mixins.Utilities.ReflectionUtility;

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

    class Base1
    {
      public virtual void Foo (int i) { }
      public virtual void Bar (int i) { }
      public virtual void Fred (int i) { }

      public virtual int FooP { get { return 0; } set { } }
      public virtual int BarP { get { return 0; } set { } }
      public virtual int FredP { get { return 0; } set { } }

      public virtual event Func<int> FooE;
      public virtual event Func<int> BarE;
      public virtual event Func<int> FredE;
    }

    class Derived1 : Base1
    {
      public new void Foo (int i) { }
      public override void Bar (int i) { }
      public void Baz (int i) { }

      public new int FooP { get { return 0; } set { } }
      public override int BarP { get { return 0; } set { } }
      public int BazP { get { return 0; } set { } }

      public new event Func<int> FooE;
      public override event Func<int> BarE;
      public event Func<int> BazE;
    }

    private bool ContainsNotNull<T> (IList<T> list, T item)
    {
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("item", item);
      return list.Contains (item);
    }

    [Test]
    public void GetAllInstanceMethodsExceptOverridden1()
    {
      List<MethodInfo> methods = new List<MethodInfo> (ReflectionUtility.GetAllInstanceMethodBaseDefinitions (typeof (Derived1)));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Base1).GetMethod ("Foo")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Base1).GetMethod ("Bar")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Base1).GetMethod ("Fred")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Derived1).GetMethod ("Foo")));
      Assert.IsFalse (ContainsNotNull (methods, typeof (Derived1).GetMethod ("Bar")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Derived1).GetMethod ("Baz")));
      Assert.IsFalse (ContainsNotNull(methods, typeof (Derived1).GetMethod ("Fred")));
    }

    [Test]
    [Ignore("TODO")]
    public void GetAllInstancePropertiesExceptOverridden1 ()
    {
      List<PropertyInfo> properties = new List<PropertyInfo> (ReflectionUtility.GetAllInstancePropertyBaseDefinitions (typeof (Derived1)));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Base1).GetProperty ("FooP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Base1).GetProperty ("BarP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Base1).GetProperty ("FredP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Derived1).GetProperty ("FooP")));
      Assert.IsFalse (ContainsNotNull (properties, typeof (Derived1).GetProperty ("BarP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Derived1).GetProperty ("BazP")));
      Assert.IsFalse (ContainsNotNull (properties, typeof (Derived1).GetProperty ("FredP")));
    }

    [Test]
    [Ignore ("TODO")]
    public void GetAllInstanceEventsExceptOverridden1 ()
    {
      List<EventInfo> events = new List<EventInfo> (ReflectionUtility.GetAllInstanceEventBaseDefinitions (typeof (Derived1)));
      Assert.IsTrue (ContainsNotNull (events, typeof (Base1).GetEvent ("FooE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Base1).GetEvent ("BarE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Base1).GetEvent ("FredE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Derived1).GetEvent ("FooE")));
      Assert.IsFalse (ContainsNotNull (events, typeof (Derived1).GetEvent ("BarE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Derived1).GetEvent ("BazE")));
      Assert.IsFalse (ContainsNotNull (events, typeof (Derived1).GetEvent ("FredE")));
    }

    class Base2<T>
    {
      public virtual void Foo (T i) { }
      public virtual void Bar (T i) { }
      public virtual void Fred (T i) { }

      public virtual T FooP { get { return default (T); } set { } }
      public virtual T BarP { get { return default (T); } set { } }
      public virtual T FredP { get { return default (T); } set { } }

      public virtual event Func<T> FooE;
      public virtual event Func<T> BarE;
      public virtual event Func<T> FredE;
    }

    class Derived2 : Base2<int>
    {
      public new void Foo (int i) { }
      public override void Bar (int i) { }
      public void Baz (int i) { }

      public new int FooP { get { return 0; } set { } }
      public override int BarP { get { return 0; } set { } }
      public int BazP { get { return 0; } set { } }

      public new event Func<int> FooE;
      public override event Func<int> BarE;
      public event Func<int> BazE;
    }

    [Test]
    public void GetAllInstanceMethodsExceptOverridden2 ()
    {
      List<MethodInfo> methods = new List<MethodInfo> (ReflectionUtility.GetAllInstanceMethodBaseDefinitions (typeof (Derived2)));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Base2<int>).GetMethod ("Foo")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Base2<int>).GetMethod ("Bar")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Base2<int>).GetMethod ("Fred")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Derived2).GetMethod ("Foo")));
      Assert.IsFalse (ContainsNotNull (methods, typeof (Derived2).GetMethod ("Bar")));
      Assert.IsTrue (ContainsNotNull (methods, typeof (Derived2).GetMethod ("Baz")));
      Assert.IsFalse (ContainsNotNull (methods, typeof (Derived2).GetMethod ("Fred")));
    }

    [Test]
    [Ignore ("TODO")]
    public void GetAllInstancePropertiesExceptOverridden2 ()
    {
      List<PropertyInfo> properties = new List<PropertyInfo> (ReflectionUtility.GetAllInstancePropertyBaseDefinitions (typeof (Derived2)));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Base2<int>).GetProperty ("FooP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Base2<int>).GetProperty ("BarP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Base2<int>).GetProperty ("FredP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Derived2).GetProperty ("FooP")));
      Assert.IsFalse (ContainsNotNull (properties, typeof (Derived2).GetProperty ("BarP")));
      Assert.IsTrue (ContainsNotNull (properties, typeof (Derived2).GetProperty ("BazP")));
      Assert.IsFalse (ContainsNotNull (properties, typeof (Derived2).GetProperty ("FredP")));
    }

    [Test]
    [Ignore ("TODO")]
    public void GetAllInstanceEventsExceptOverridden2 ()
    {
      List<EventInfo> events = new List<EventInfo> (ReflectionUtility.GetAllInstanceEventBaseDefinitions (typeof (Derived2)));
      Assert.IsTrue (ContainsNotNull (events, typeof (Base2<int>).GetEvent ("FooE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Base2<int>).GetEvent ("BarE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Base2<int>).GetEvent ("FredE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Derived2).GetEvent ("FooE")));
      Assert.IsFalse (ContainsNotNull (events, typeof (Derived2).GetEvent ("BarE")));
      Assert.IsTrue (ContainsNotNull (events, typeof (Derived2).GetEvent ("BazE")));
      Assert.IsFalse (ContainsNotNull (events, typeof (Derived2).GetEvent ("FredE")));
    }
  }
}
