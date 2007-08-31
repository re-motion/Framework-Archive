using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class CanAscribe_WithNonGenericClass
  {
    [Test]
    public void DerivedType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedType), typeof (DerivedType)));
    }

    [Test]
    public void BaseType ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (BaseType), typeof (DerivedType)));
    }
  }
}