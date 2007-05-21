using System;
using System.Collections.Generic;
using System.Reflection;
using Mixins.Context;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using Mixins.Utilities;
using NUnit.Framework;
using Mixins.CodeGeneration;
using Rubicon;
using Rubicon.Utilities;
using ReflectionUtility=Mixins.Utilities.ReflectionUtility;
using Rubicon.Collections;

namespace Mixins.UnitTests.Utilities
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

    class Base
    {
      public void Foo (int i) { }
      public virtual void Bar (int i) { }

      public new int FooP { get { return 0; } set { } }
      public virtual int BarP { get { return 0; } set { } }

      public new event Func<int> FooE;
      public virtual event Func<int> BarE;
    }

    class Derived : Base
    {
      public virtual new void Foo (int i) { }
      public override void Bar (int i) { }
      public void Baz (int i) { }

      public virtual new int FooP { get { return 0; } set { } }
      public override int BarP { get { return 0; } set { } }
      public int BazP { get { return 0; } set { } }

      public virtual new event Func<int> FooE;
      public override event Func<int> BarE;
      public event Func<int> BazE;
    }

    [Test]
    public void IsNewSlotMember()
    {
      Assert.IsTrue (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Foo")));
      Assert.IsTrue (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("FooP")));
      Assert.IsTrue (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("FooE")));

      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Bar")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("BarP")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("BarE")));

      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Baz")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("BazP")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("BazE")));
    }

    [Test]
    public void IsVirtualMember ()
    {
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Foo")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("FooP")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("FooE")));

      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Bar")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("BarP")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("BarE")));

      Assert.IsFalse (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Baz")));
      Assert.IsFalse (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("BazP")));
      Assert.IsFalse (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("BazE")));
    }

    [Test]
    public void GetMethodSignature()
    {
      Tuple<Type, Type[]> methodSignature = ReflectionUtility.GetMethodSignature (typeof (object).GetMethod ("ReferenceEquals"));
      Assert.AreEqual (typeof (bool), methodSignature.A);
      Assert.AreEqual (2, methodSignature.B.Length);
      Assert.AreEqual (typeof (object), methodSignature.B[0]);
      Assert.AreEqual (typeof (object), methodSignature.B[1]);
    }
  }
}
