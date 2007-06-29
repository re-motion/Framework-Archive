using System;
using NUnit.Framework;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class UndefinedEnumValueAttributeTest
  {
    private enum TestEnum
    {
      Undefined = 0,
      Value1 = 1,
      Value2 = 2
    }

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