using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetAscribedGenericArguments_WithInterface
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericType_WithDerivedType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.AreEqual (
          new Type[] { typeof (ParameterType), typeof (int) },
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<,>)));
    }

    [Test]
    public void OpenGenericType ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<,>));
      Assert.AreEqual (2, types.Length);
      Assert.AreEqual ("T1", types[0].Name);
      Assert.AreEqual ("T2", types[1].Name);
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<,>));
      Assert.AreEqual (2, types.Length);
      Assert.AreEqual (typeof (ParameterType), types[0]);
      Assert.AreEqual ("T", types[1].Name);
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenDerivedGenericType ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "Argument type has type Rubicon.Core.UnitTests.Utilities.ReflectionUtilityTests.TypeWithBaseInterface when type "
        + "Rubicon.Core.UnitTests.Utilities.ReflectionUtilityTests.IGenericInterface`1[T] was expected.\r\n"
        + "Parameter name: type")]
    public void BaseType ()
    {
      ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithBaseInterface), typeof (IGenericInterface<>));
    }
  }
}