using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StorageSpecificName: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageSpecificNameAttribute> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageSpecificNameAttribute.NoAttribute",
          actual.PropertyName);
      Assert.AreEqual ("NoAttribute", actual.StorageSpecificName);
    }

    [Test]
    public void GetMetadata_WithStorageSpecificName()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageSpecificNameAttribute> ("StorageSpecificName");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageSpecificNameAttribute.StorageSpecificName",
          actual.PropertyName);
      Assert.AreEqual ("CustomName", actual.StorageSpecificName);
    }
  }
}