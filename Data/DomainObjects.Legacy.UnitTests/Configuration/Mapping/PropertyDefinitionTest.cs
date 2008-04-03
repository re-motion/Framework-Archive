using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.NullableValueTypes;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : StandardMappingTest
  {
    private XmlBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = new XmlBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order));
    }

    [Test]
    public void InitializeWithResolvedPropertyType ()
    {
      XmlBasedPropertyDefinition actual = new XmlBasedPropertyDefinition (_classDefinition, "PropertyName", "StorageSpecificName", "int32", true, true, null);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.AreEqual (NaInt32.Null, actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("int32", actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (NaInt32), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithUnresolvedPropertyType ()
    {
      XmlBasedPropertyDefinition actual = new XmlBasedPropertyDefinition (_classDefinition, "PropertyName", "StorageSpecificName", "int32", false, true, null);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("int32", actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.IsNull (actual.PropertyType);
      Assert.IsFalse (actual.IsPropertyTypeResolved);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithUnresolvedUnknownPropertyType ()
    {
      XmlBasedPropertyDefinition actual = new XmlBasedPropertyDefinition (_classDefinition, "PropertyName", "StorageSpecificName", "UnknownMappingType", false, true, null);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual ("UnknownMappingType", actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.IsNull (actual.PropertyType);
      Assert.IsFalse (actual.IsPropertyTypeResolved);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void StringPropertyWithoutMaxLength ()
    {
      XmlBasedPropertyDefinition definition = new XmlBasedPropertyDefinition (_classDefinition, "PropertyName", "StorageSpecificName", "string");
      Assert.IsNull (definition.MaxLength);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "MaxLength parameter cannot be supplied with value of type 'System.Int32'.")]
    public void IntPropertyWithMaxLength ()
    {
      new XmlBasedPropertyDefinition (_classDefinition, "test", "test", "int32", 10);
    }

    [Test]
    public void ObjectIDProperty ()
    {
      XmlBasedPropertyDefinition definition = new XmlBasedPropertyDefinition (_classDefinition, "PropertyName", "StorageSpecificName", TypeInfo.ObjectIDMappingTypeName);
      Assert.IsTrue (definition.IsObjectID);
    }

    [Test]
    public void MappingTypeName ()
    {
      XmlBasedPropertyDefinition definition = new XmlBasedPropertyDefinition (_classDefinition, "test", "test", "date");

      Assert.AreEqual (typeof (DateTime), definition.PropertyType);
      Assert.AreEqual ("date", definition.MappingTypeName);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void InvalidMappingType ()
    {
      new XmlBasedPropertyDefinition (_classDefinition, "test", "test", "InvalidMappingType");
    }

    [Test]
    [Obsolete]
    public void GetColumnName ()
    {
      XmlBasedPropertyDefinition actual = new XmlBasedPropertyDefinition (_classDefinition, "PropertyName", "StorageSpecificName", "int32", true, true, null);

      Assert.AreEqual ("StorageSpecificName", actual.StorageSpecificName);
      Assert.AreEqual (actual.StorageSpecificName, actual.ColumnName);
    }
  }
}
