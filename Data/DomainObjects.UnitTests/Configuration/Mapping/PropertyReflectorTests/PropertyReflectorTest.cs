using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class PropertyReflectorTest: StandardMappingTest
  {
    private PropertyReflector _propertyReflector;

    public override void SetUp()
    {
      base.SetUp();
      _propertyReflector = new PropertyReflector();
    }

    [Test]
    public void GetPropertyName()
    {
      Assert.AreEqual ("System.DateTime.Year", PropertyReflector.GetPropertyName (typeof (DateTime).GetProperty ("Year")));
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty",
          PropertyReflector.GetPropertyName (typeof (ClassWithAllDataTypes).GetProperty ("DateTimeProperty")));
    }

    [Test]
    public void GetMetadata_FromSingleProperty_WithBasicType()
    {
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreEqual ("BooleanProperty", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (bool), actual.PropertyType);
      Assert.AreEqual ("boolean", actual.MappingTypeName);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (false, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_FromSingleProperty_WithNullableBasicType()
    {
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("NaBooleanProperty");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty", actual.PropertyName);
      Assert.AreEqual ("NaBooleanProperty", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (NaBoolean), actual.PropertyType);
      Assert.AreEqual ("boolean", actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (NaBoolean.Null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_FromSingleProperty_WithEnumProperty()
    {
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("EnumProperty");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty", actual.PropertyName);
      Assert.AreEqual ("EnumProperty", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes+EnumType, Rubicon.Data.DomainObjects.UnitTests",
          actual.MappingTypeName);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_FromSingleProperty_WithOptionalRelationProperty()
    {
      PropertyInfo propertyInfo = typeof (ClassWithGuidKey).GetProperty ("ClassWithValidRelationsOptional");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional", actual.PropertyName);
      Assert.AreEqual ("ClassWithValidRelationsOptional", actual.ColumnName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.AreEqual (TypeInfo.ObjectIDMappingTypeName, actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The property type System.Object is not supported.\r\n  "
            + "Type: Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes, property: ObjectProperty")]
    public void GetMetadata_FromSingleProperty_WithInvalidPropertyType()
    {
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("ObjectProperty");

      _propertyReflector.GetMetadata (propertyInfo);
    }
  }
}