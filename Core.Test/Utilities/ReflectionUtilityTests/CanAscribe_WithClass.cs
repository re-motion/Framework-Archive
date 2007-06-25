using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class CanAscribe_WithClass
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<>)));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<,>)));
    }

    [Test]
    public void OpenGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)));
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)));
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedOpenGenericType<>), typeof (GenericType<,>)));
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<>)));
    }

    [Test]
    public void OpenDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<>), typeof (GenericType<>)));
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<>)));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<>)));
    }

    [Test]
    public void BaseType ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (BaseType), typeof (GenericType<>)));
    }
  }
}