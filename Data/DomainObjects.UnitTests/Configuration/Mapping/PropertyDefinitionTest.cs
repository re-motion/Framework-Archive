using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : ReflectionBasedMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PropertyDefinitionTest ()
    {
    }

    // methods and properties

    [Test]
    public void InitializeWithResolvedPropertyType ()
    {
      PropertyDefinition actual = new PropertyDefinition ("PropertyName", "ColumnName", "int32", true, true, NaInt32.Null, true);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (NaInt32.Null, actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("int32", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (NaInt32), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
    }

    [Test]
    public void InitializeWithUnresolvedPropertyType ()
    {
      PropertyDefinition actual = new PropertyDefinition ("PropertyName", "ColumnName", "int32", false, true, NaInt32.Null, true);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("int32", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.IsNull (actual.PropertyType);
      Assert.IsFalse (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
    }

    [Test]
    public void InitializeWithUnresolvedUnknownPropertyType ()
    {
      PropertyDefinition actual = new PropertyDefinition ("PropertyName", "ColumnName", "UnknownMappingType", false, true, NaInt32.Null, true);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("UnknownMappingType", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.IsNull (actual.PropertyType);
      Assert.IsFalse (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
    }

    [Test]
    public void StringPropertyWithoutMaxLength ()
    {
      PropertyDefinition definition = new PropertyDefinition ("PropertyName", "ColumnName", "string");
      Assert.AreEqual (NaInt32.Null, definition.MaxLength);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access property 'StorageSpecificName' for non-persistent property definitions.")]
    public void NonPersistentProperty ()
    {
      PropertyDefinition actual = new PropertyDefinition ("ThePropertyName", "TheColumnName", "int32", true, true, NaInt32.Null, false);
      Assert.IsFalse (actual.IsPersistent);
      Dev.Null = actual.StorageSpecificName;
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "MaxLength parameter cannot be supplied with value of type 'System.Int32'.")]
    public void IntPropertyWithMaxLength ()
    {
      PropertyDefinition definition = new PropertyDefinition ("test", "test", "int32", new NaInt32 (10));
    }

    [Test]
    public void MappingTypeName ()
    {
      PropertyDefinition definition = new PropertyDefinition ("test", "test", "date");

      Assert.AreEqual (typeof (DateTime), definition.PropertyType);
      Assert.AreEqual ("date", definition.MappingTypeName);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void InvalidMappingType ()
    {
      PropertyDefinition definition = new PropertyDefinition ("test", "test", "InvalidMappingType");
    }
  }
}
