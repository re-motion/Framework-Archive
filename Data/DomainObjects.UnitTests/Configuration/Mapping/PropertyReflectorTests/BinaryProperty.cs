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
  public class BinaryProperty: StandardMappingTest
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
      PropertyInfo propertyInfo = typeof (ClassWithBinaryProperties).GetProperty ("NoAttribute");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBinaryProperties.NoAttribute", 
          actual.PropertyName);
      Assert.AreEqual ("NoAttribute", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.AreEqual ("binary", actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableFromAttribute ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithBinaryProperties).GetProperty ("NullableFromAttribute");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBinaryProperties.NullableFromAttribute",
          actual.PropertyName);
      Assert.AreEqual ("NullableFromAttribute", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.AreEqual ("binary", actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithBinaryProperties).GetProperty ("NotNullable");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBinaryProperties.NotNullable",
          actual.PropertyName);
      Assert.AreEqual ("NotNullable", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.AreEqual ("binary", actual.MappingTypeName);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithMaximumLength ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithBinaryProperties).GetProperty ("MaximumLength");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBinaryProperties.MaximumLength",
          actual.PropertyName);
      Assert.AreEqual ("MaximumLength", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.AreEqual ("binary", actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (new NaInt32 (100), actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableAndMaximumLength ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithBinaryProperties).GetProperty ("NotNullableAndMaximumLength");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBinaryProperties.NotNullableAndMaximumLength",
          actual.PropertyName);
      Assert.AreEqual ("NotNullableAndMaximumLength", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
      Assert.AreEqual ("binary", actual.MappingTypeName);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (new NaInt32 (100), actual.MaxLength);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.BinaryPropertyAttribute may be only applied to properties of type System.Byte[].\r\n  "
            + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests.BinaryProperty, property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty ()
    {
      PropertyInfo propertyInfo = GetType ().GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);

      _propertyReflector.GetMetadata (propertyInfo);
    }

    [BinaryProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException (); }
    }
  }
}