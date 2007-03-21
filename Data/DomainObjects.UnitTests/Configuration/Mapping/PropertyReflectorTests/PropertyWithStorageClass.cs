using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.Development.UnitTesting;

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
    public void GetMetadata_WithStorageClassPersistent()
    {
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesForEachStorageClass).GetProperty ("Persistent");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesForEachStorageClass.Persistent",
          actual.PropertyName);
      Assert.IsTrue (actual.IsPersistent);
      Assert.AreEqual ("Persistent", actual.ColumnName);
    }
  
    [Test]
    [ExpectedException (typeof (MappingException), 
      "Only StorageClass.Persistent is supported.\r\n  "
      + "Type: Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesForEachStorageClass, "
      + "property: Transaction")]
    public void GetMetadata_WithStorageClassTransaction ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesForEachStorageClass).GetProperty ("Transaction");

      _propertyReflector.GetMetadata (propertyInfo);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Only StorageClass.Persistent is supported.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithPropertiesForEachStorageClass, property: None")]
    public void GetMetadata_WithStorageClassNone ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithPropertiesForEachStorageClass).GetProperty ("None");

      _propertyReflector.GetMetadata (propertyInfo);
    }
  }
}