using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common: StandardMappingTest
  {
    [Test]
    public void CreateRelationEndPointReflector()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NoAttribute");
      Assert.IsInstanceOfType (
          typeof (RdbmsRelationEndPointReflector), 
          RelationEndPointReflector.CreateRelationEndPointReflector (
              CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties)), propertyInfo));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithoutAttribute ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("NoAttribute");
      RelationEndPointReflector relationEndPointReflector = 
          new RelationEndPointReflector (CreateReflectionBasedClassDefinition (typeof (ClassWithManySideRelationProperties)), propertyInfo);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithCollectionPropertyAndWithoutAttribute ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation", true, false);

      PropertyInfo propertyInfo = type.GetProperty ("LeftSide");
      RelationEndPointReflector relationEndPointReflector = 
          new RelationEndPointReflector (CreateReflectionBasedClassDefinition(type), propertyInfo);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Rubicon.Data.DomainObjects.MandatoryAttribute' may be only applied to properties assignable to types "
        + "'Rubicon.Data.DomainObjects.DomainObject' or 'Rubicon.Data.DomainObjects.ObjectList`1[T]'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties, "
        + "property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      Type type = GetClassWithInvalidProperties();

      PropertyInfo propertyInfo = type.GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = 
          new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo);

      relationEndPointReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Rubicon.Data.DomainObjects.StringPropertyAttribute' may be only applied to properties of type 'System.String'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties, "
        + "property: PropertyWithStringAttribute")]
    public void GetMetadata_WithStringAttributeAppliedToInvalidProperty()
    {
      Type type = GetClassWithInvalidProperties ();
      PropertyInfo propertyInfo = type.GetProperty ("PropertyWithStringAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = 
          new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo);

      relationEndPointReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The 'Rubicon.Data.DomainObjects.BinaryPropertyAttribute' may be only applied to properties of type 'System.Byte[]'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties, "
        + "property: PropertyWithBinaryAttribute")]
    public void GetMetadata_WithBinaryAttributeAppliedToInvalidProperty()
    {
      Type type = GetClassWithInvalidProperties ();
      PropertyInfo propertyInfo = type.GetProperty ("PropertyWithBinaryAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = 
          new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (type), propertyInfo);

      relationEndPointReflector.GetMetadata ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "The classDefinition's class type 'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.BaseClass' is not assignable "
        + "to the property's declaring type.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void Initialize_WithPropertyInfoNotAssignableToTheClassDefinitionsType ()
    {
      Type classType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.BaseClass", true, false);
      Type declaringType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute", true, false);
      PropertyInfo propertyInfo = declaringType.GetProperty ("Int32");

      new RdbmsRelationEndPointReflector (CreateReflectionBasedClassDefinition (classType), propertyInfo);
    }

    private Type GetClassWithInvalidProperties ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidProperties", true, false);
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type> ());
    }
  }
}
