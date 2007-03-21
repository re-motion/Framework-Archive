using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class NSideRelationProperty: ReflectionBasedMappingTest
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
      PropertyInfo propertyInfo = typeof (ClassWithNSideRelationProperties).GetProperty ("NoAttribute");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithNSideRelationProperties.NoAttribute",
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
      PropertyInfo propertyInfo = typeof (ClassWithNSideRelationProperties).GetProperty ("NotNullable");

      PropertyDefinition actual = _propertyReflector.GetMetadata (propertyInfo);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithNSideRelationProperties.NotNullable",
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
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests.NSideRelationProperty, property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);

      _propertyReflector.GetMetadata (propertyInfo);
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}