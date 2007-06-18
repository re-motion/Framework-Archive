using System;
using System.Collections.Generic;
using Mixins.Utilities;
using NUnit.Framework;
using Rubicon;
using Rubicon.Collections;

namespace Mixins.UnitTests.Utilities
{
  [TestFixture]
  public class ReflectionUtilityTests
  {
    class Base
    {
      public void Foo (int i) { }
      public virtual void Bar (int i) { }

      public int FooP { get { return 0; } set { } }
      public virtual int BarP { get { return 0; } set { } }

      public event Func<int> FooE;
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

    public class BlaAttribute : Attribute { }

    class C<T1, T2, [Bla]T3> : Mixin<T2>
        where T2 : class
    {
    }

    [Test]
    public void GetGenericArgumentsBoundToAttribute()
    {
      List<Type> arguments = new List<Type> (ReflectionUtility.GetGenericParametersAssociatedWithAttribute (typeof (C<,,>), typeof (BlaAttribute)));
      Assert.AreEqual (1, arguments.Count);
      Assert.IsNotNull (arguments.Find (delegate (Type arg) { return arg.Name == "T3"; }));

      Type thisAttribute = typeof (Mixin).Assembly.GetType ("Mixins.ThisAttribute");
      arguments = new List<Type> (ReflectionUtility.GetGenericParametersAssociatedWithAttribute (typeof (C<,,>), thisAttribute));
      Assert.AreEqual (1, arguments.Count);
      Assert.IsNotNull (arguments.Find (delegate (Type arg) { return arg.Name == "T2"; }));
    }

    class GenericBase<T>
    {
      public virtual void Foo () { }
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
    public void IsSameTypeIgnoreGenerics ()
    {
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<>)));
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<string>)));
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericBase<int>), typeof (GenericBase<string>)));

      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<>)));
      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<string>)));
      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<int>), typeof (GenericBase<string>)));

      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<>), typeof (object)));
      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<int>), typeof (object)));

      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (object), typeof (object)));
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (object), typeof (object)));
    }

    [Test]
    public void IsSubclassIgnoreGenerics ()
    {
      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<>)));
      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<string>)));
      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericBase<int>), typeof (GenericBase<string>)));

      Assert.IsTrue (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<>)));
      Assert.IsTrue (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<string>)));
      Assert.IsTrue (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericSub<int>), typeof (GenericBase<string>)));

      Assert.IsTrue (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericSub<>), typeof (object)));
      Assert.IsTrue (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericSub<int>), typeof (object)));

      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericSub<>)));
      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericSub<string>)));
      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (GenericBase<int>), typeof (GenericSub<string>)));

      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (object), typeof (GenericSub<>)));
      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (object), typeof (GenericSub<string>)));
      Assert.IsFalse (ReflectionUtility.IsSubclassIgnoreGenerics (typeof (object), typeof (GenericSub<string>)));
    }

    [Test]
    public void IsSameOrSubclassIgnoreGenerics ()
    {
      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<>)));
      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<string>)));
      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericBase<int>), typeof (GenericBase<string>)));

      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<>)));
      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<string>)));
      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericSub<int>), typeof (GenericBase<string>)));

      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericSub<>), typeof (object)));
      Assert.IsTrue (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericSub<int>), typeof (object)));

      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (object), typeof (object)));
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (object), typeof (object)));

      Assert.IsFalse (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericSub<>)));
      Assert.IsFalse (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericBase<>), typeof (GenericSub<string>)));
      Assert.IsFalse (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (GenericBase<int>), typeof (GenericSub<string>)));

      Assert.IsFalse (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (object), typeof (GenericSub<>)));
      Assert.IsFalse (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (object), typeof (GenericSub<string>)));
      Assert.IsFalse (ReflectionUtility.IsSameOrSubclassIgnoreGenerics (typeof (object), typeof (GenericSub<string>)));
    }
  }
}
