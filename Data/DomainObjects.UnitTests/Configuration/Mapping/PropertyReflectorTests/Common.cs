using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class Common: BaseTest
  {
    [Test]
    public void GetMetadata_ForSingleProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("BooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreEqual ("Boolean", actual.StorageSpecificName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (bool), actual.PropertyType);
      Assert.AreEqual ("boolean", actual.MappingTypeName);
    }
  }
}