using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : StandardMappingTest
  {
    [Test]
    public void InitializeWithResolvedPropertyType ()
    {
      PropertyDefinition actual = new XmlBasedPropertyDefinition ("PropertyName", "StorageSpecificName", "int32", true, true, null);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.AreEqual (NaInt32.Null, actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("int32", actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (NaInt32), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
    }

    [Test]
    public void InitializeWithUnresolvedPropertyType ()
    {
      PropertyDefinition actual = new XmlBasedPropertyDefinition ("PropertyName", "StorageSpecificName", "int32", false, true, null);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("int32", actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.IsNull (actual.PropertyType);
      Assert.IsFalse (actual.IsPropertyTypeResolved);
    }

    [Test]
    public void InitializeWithUnresolvedUnknownPropertyType ()
    {
      PropertyDefinition actual = new XmlBasedPropertyDefinition ("PropertyName", "StorageSpecificName", "UnknownMappingType", false, true, null);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("UnknownMappingType", actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.IsNull (actual.PropertyType);
      Assert.IsFalse (actual.IsPropertyTypeResolved);
    }

    [Test]
    public void StringPropertyWithoutMaxLength ()
    {
      PropertyDefinition definition = new XmlBasedPropertyDefinition ("PropertyName", "StorageSpecificName", "string");
      Assert.IsNull (definition.MaxLength);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "MaxLength parameter cannot be supplied with value of type 'System.Int32'.")]
    public void IntPropertyWithMaxLength ()
    {
      PropertyDefinition definition = new XmlBasedPropertyDefinition ("test", "test", "int32", 10);
    }

    [Test]
    public void MappingTypeName ()
    {
      PropertyDefinition definition = new XmlBasedPropertyDefinition ("test", "test", "date");

      Assert.AreEqual (typeof (DateTime), definition.PropertyType);
      Assert.AreEqual ("date", definition.MappingTypeName);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void InvalidMappingType ()
    {
      PropertyDefinition definition = new XmlBasedPropertyDefinition ("test", "test", "InvalidMappingType");
    }

    [Test]
    [Obsolete]
    public void GetColumnName ()
    {
      PropertyDefinition actual = new XmlBasedPropertyDefinition ("PropertyName", "StorageSpecificName", "int32", true, true, null);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.AreEqual (actual.StorageSpecificName, actual.ColumnName);
    }
  }
}
