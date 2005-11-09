using System;
using NUnit.Framework;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
[TestFixture]
public class UndefinedEnumValueAttributeTest
{
  // types

  private enum TestEnum
  {
    Undefined = 0,
    Value1 = 1,
    Value2 = 2
  }

  // static members and constants

  // member fields

  // construction and disposing

  public UndefinedEnumValueAttributeTest ()
  {
  }

  // methods and properties

  [Test]
  public void Initialize ()
  {
    UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (TestEnum.Undefined);
    
    Assert.AreEqual (TestEnum.Undefined, undefinedValueAttribute.Value);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void InitializeWithInvalidValue ()
  {
    TestEnum invalidValue = (TestEnum) (-1);
    UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (invalidValue);
  }

  [Test]
  [ExpectedException (typeof (ArgumentTypeException))]
  public void InitializeWithObjectOfInvalidType ()
  {
    UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (this);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithNull ()
  {
    UndefinedEnumValueAttribute undefinedValueAttribute = new UndefinedEnumValueAttribute (null);
  }
}
}
