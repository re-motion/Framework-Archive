using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class CanAscribe_WithInterface
  {
    [Test]
    public void ClosedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericInterface_WithDerivedType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericInterface_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<,>)));
    }

    [Test]
    public void OpenGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenGenericInterface_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<,>)));
    }

    [Test]
    public void OpenGenericInterface_WithOneOpenTypeParameter ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<,>)));
    }

    [Test]
    public void ClosedDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void NonGenericDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenGenericDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void BaseInterface ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithBaseInterface), typeof (IGenericInterface<>)));
    }
  }
}