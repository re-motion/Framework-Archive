using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class ManySideRelationProperty: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NoAttribute",
          actual.PropertyName);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.AreEqual (TypeInfo.ObjectIDMappingTypeName, actual.MappingTypeName);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("NotNullable");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
          actual.PropertyName);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.AreEqual (TypeInfo.ObjectIDMappingTypeName, actual.MappingTypeName);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.MandatoryAttribute may be only applied to properties assignable to types "
        + "Rubicon.Data.DomainObjects.DomainObject or Rubicon.Data.DomainObjects.DomainObjectCollection.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests.ManySideRelationProperty, "
        + "property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ManySideRelationProperty> ("Int32Property");

      propertyReflector.GetMetadata();
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}