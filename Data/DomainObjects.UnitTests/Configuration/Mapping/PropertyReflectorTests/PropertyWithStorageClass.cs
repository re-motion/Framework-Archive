using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class PropertyWithStorageClass: ReflectionBasedMappingTest
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
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesHavingStorageClassAttribute).GetProperty ("NoAttribute");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.NoAttribute",
          actual.PropertyName);
      Assert.IsTrue (actual.IsPersistent);
      Assert.AreEqual ("NoAttribute", actual.StorageSpecificName);
    }

    [Test]
    public void GetMetadata_WithStorageClassPersistent()
    {
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesHavingStorageClassAttribute).GetProperty ("Persistent");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

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
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesHavingStorageClassAttribute).GetProperty ("Transaction");

      _propertyReflector.GetMetadata (propertyInfo);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Only StorageClass.Persistent is supported.\r\n  "
            + "Type: Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute, "
                + "property: None")]
    public void GetMetadata_WithStorageClassNone()
    {
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesHavingStorageClassAttribute).GetProperty ("None");

      _propertyReflector.GetMetadata (propertyInfo);
    }
  }
}