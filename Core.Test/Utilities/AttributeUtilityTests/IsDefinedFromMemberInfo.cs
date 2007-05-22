using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class IsDefinedFromMemberInfo
  {
    private PropertyInfo _basePropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithMultipleAttribute;

    [SetUp]
    public void SetUp ()
    {
      _basePropertyWithSingleAttribute = typeof (SampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithSingleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithMultipleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithMultipleAttribute");
    }

    [Test]
    public void Test_FromBaseWithAttribute ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), true));
    }

    [Test]
    public void TestGeneric_FromBaseWithAttribute ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined<InheritedAttribute> (_basePropertyWithSingleAttribute, true));
    }

    [Test]
    public void Test_FromOverrideWithAttribute_ExpectAmbigousMatch ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined (_derivedPropertyWithMultipleAttribute, typeof (MultipleAttribute), true));
    }

    [Test]
    public void Test_FromBaseWithInterface ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined (_basePropertyWithSingleAttribute, typeof (ICustomAttribute), true));
    }

    [Test]
    public void TestGeneric_FromBaseWithInterface ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined<ICustomAttribute> (_basePropertyWithSingleAttribute, true));
    }

    [Test]
    public void Test_FromOverrideWithInterface_ExpectAmbigousMatch ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined (_derivedPropertyWithMultipleAttribute, typeof (ICustomAttribute), true));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: T")]
    public void TestGeneric_FromBaseWithInvalidType ()
    {
      AttributeUtility.IsDefined<object> (_basePropertyWithSingleAttribute, true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = 
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: attributeType")]
    public void Test_FromBaseWithInvalidType ()
    {
      AttributeUtility.IsDefined (_basePropertyWithSingleAttribute, typeof (object), true);
    }

    [Test]
    public void Test_FromOverrideWithAttribute ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), true));
    }

    [Test]
    public void Test_FromOverrideWithInterface ()
    {
      Assert.IsTrue (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), true));
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited ()
    {
      Assert.IsFalse (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), false));
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited ()
    {
      Assert.IsFalse (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), false));
    }
  }
}