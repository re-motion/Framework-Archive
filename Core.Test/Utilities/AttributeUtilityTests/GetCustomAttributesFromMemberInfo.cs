using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{
  [TestFixture]
  public class GetCustomAttributesFromMemberInfo
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
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_basePropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    public void Test_FromBaseWithInterface()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_basePropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The type parameter must be assignable to System.Attribute or an interface.\r\nParameter name: T")
    ]
    public void Test_FromBaseWithInvalidType()
    {
      AttributeUtility.GetCustomAttributes<object> (_basePropertyWithSingleAttribute, true);
    }

    [Test]
    public void Test_FromOverrideWithAttribute()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_derivedPropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    public void Test_FromOverrideWithInterface()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndMultiple()
    {
      MultipleAttribute[] attributes = AttributeUtility.GetCustomAttributes<MultipleAttribute> (_derivedPropertyWithMultipleAttribute, true);

      Assert.AreEqual (2, attributes.Length);
      Assert.IsNotNull (attributes[0]);
      Assert.IsNotNull (attributes[1]);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndMultiple()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithMultipleAttribute, true);

      Assert.AreEqual (2, attributes.Length);
      Assert.IsNotNull (attributes[0]);
      Assert.IsNotNull (attributes[1]);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_derivedPropertyWithSingleAttribute, false);

      Assert.IsEmpty (attributes);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithSingleAttribute, false);

      Assert.IsEmpty (attributes);
    }
  }
}