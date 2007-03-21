using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StringProperty: StandardMappingTest
  {
    private PropertyReflector _propertyReflector;

    public override void SetUp()
    {
      base.SetUp();
      _propertyReflector = new PropertyReflector();
    }

    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyInfo propertyInfo = typeof (ClassWithStringProperties).GetProperty ("NoAttribute");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreEqual ("NoAttribute", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableFromAttribute()
    {
      PropertyInfo propertyInfo = typeof (ClassWithStringProperties).GetProperty ("NullableFromAttribute");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NullableFromAttribute",
          actual.PropertyName);
      Assert.AreEqual ("NullableFromAttribute", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute()
    {
      PropertyInfo propertyInfo = typeof (ClassWithStringProperties).GetProperty ("NotNullable");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullable",
          actual.PropertyName);
      Assert.AreEqual ("NotNullable", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithMaximumLength()
    {
      PropertyInfo propertyInfo = typeof (ClassWithStringProperties).GetProperty ("MaximumLength");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.MaximumLength",
          actual.PropertyName);
      Assert.AreEqual ("MaximumLength", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (new NaInt32 (100), actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableAndMaximumLength()
    {
      PropertyInfo propertyInfo = typeof (ClassWithStringProperties).GetProperty ("NotNullableAndMaximumLength");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullableAndMaximumLength",
          actual.PropertyName);
      Assert.AreEqual ("NotNullableAndMaximumLength", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (string), actual.PropertyType);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (new NaInt32 (100), actual.MaxLength);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.StringPropertyAttribute may be only applied to properties of type System.String.\r\n  "
            + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests.StringProperty, property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);

      _propertyReflector.GetMetadata (propertyInfo);
    }

    [StringProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}