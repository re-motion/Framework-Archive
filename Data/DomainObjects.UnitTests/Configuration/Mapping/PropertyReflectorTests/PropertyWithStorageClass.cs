using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class PropertyWithStorageClass: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.NoAttribute",
          actual.PropertyName);
      Assert.IsTrue (actual.IsPersistent);
      Assert.AreEqual ("NoAttribute", actual.StorageSpecificName);
    }

    [Test]
    public void GetMetadata_WithStorageClassPersistent()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Persistent");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.Persistent",
          actual.PropertyName);
      Assert.IsTrue (actual.IsPersistent);
      Assert.AreEqual ("Persistent", actual.StorageSpecificName);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Only StorageClass.Persistent is supported.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute, "
        + "property: Transaction")]
    public void GetMetadata_WithStorageClassTransaction()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("Transaction");

      propertyReflector.GetMetadata();
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Only StorageClass.Persistent is supported.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute, "
        + "property: None")]
    public void GetMetadata_WithStorageClassNone()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> ("None");

      propertyReflector.GetMetadata();
    }
  }
}