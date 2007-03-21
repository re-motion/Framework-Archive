using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class GetMetadataCommon: ReflectionBasedMappingTest
  {
    private PropertyReflector _propertyReflector;

    public override void SetUp()
    {
      base.SetUp();
      _propertyReflector = new PropertyReflector();
    }

    [Test]
    public void GetMetadata_ForSingleProperty()
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
    }
  }
}