using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StorageSpecificName: ReflectionBasedMappingTest
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
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesHavingStorageSpecificNameAttribute).GetProperty ("NoAttribute");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageSpecificNameAttribute.NoAttribute",
          actual.PropertyName);
      Assert.AreEqual ("NoAttribute", actual.StorageSpecificName);
    }

    [Test]
    public void GetMetadata_WithStorageSpecificName()
    {
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesHavingStorageSpecificNameAttribute).GetProperty ("StorageSpecificName");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageSpecificNameAttribute.StorageSpecificName",
          actual.PropertyName);
      Assert.AreEqual ("CustomName", actual.StorageSpecificName);
    }
  }
}