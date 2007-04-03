using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{
  [TestFixture]
  public class GetCustomAttributeFromMemberInfo
  {
    private PropertyInfo _basePropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithMultipleAttribute;

    [SetUp]
    public void SetUp()
    {
      _basePropertyWithSingleAttribute = typeof (SampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithSingleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithMultipleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithMultipleAttribute");
    }

    [Test]
    public void Test_FromBaseWithAttribute()
    {
      InheritedAttribute attribute = AttributeUtility.GetCustomAttribute<InheritedAttribute> (_basePropertyWithSingleAttribute, true);
      Assert.IsNotNull (attribute);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Multiple custom attributes of the same type found.")]
    public void Test_FromOverrideWithAttribute_ExpectAmbigousMatch()
    {
      AttributeUtility.GetCustomAttribute<MultipleAttribute> (_derivedPropertyWithMultipleAttribute, true);
    }

    [Test]
    public void Test_FromBaseWithInterface()
    {
      ICustomAttribute attribute = AttributeUtility.GetCustomAttribute<ICustomAttribute> (_basePropertyWithSingleAttribute, true);
      Assert.IsNotNull (attribute);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Multiple custom attributes of the same type found.")]
    public void Test_FromOverrideWithInterface_ExpectAmbigousMatch()
    {
      AttributeUtility.GetCustomAttribute<ICustomAttribute> (_derivedPropertyWithMultipleAttribute, true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The type parameter must be assignable to System.Attribute or an interface.\r\nParameter name: T")
    ]
    public void Test_FromBaseWithInvalidType()
    {
      AttributeUtility.GetCustomAttribute<object> (_basePropertyWithSingleAttribute, true);
    }

    [Test]
    public void Test_FromOverrideWithAttribute()
    {
      Assert.IsNotNull (AttributeUtility.GetCustomAttribute<InheritedAttribute> (_derivedPropertyWithSingleAttribute, true));
    }

    [Test]
    public void Test_FromOverrideWithInterface()
    {
      Assert.IsNotNull (AttributeUtility.GetCustomAttribute<ICustomAttribute> (_derivedPropertyWithSingleAttribute, true));
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited()
    {
      Assert.IsNull (AttributeUtility.GetCustomAttribute<InheritedAttribute> (_derivedPropertyWithSingleAttribute, false));
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited()
    {
      Assert.IsNull (AttributeUtility.GetCustomAttribute<ICustomAttribute> (_derivedPropertyWithSingleAttribute, false));
    }
  }
}