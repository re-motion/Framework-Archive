using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [Ignore("TODO: test")]
  [TestFixture]
  public class GenericManySideRelationProperty : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = new ReflectionBasedClassDefinition (
          "ClosedGenericClassWithManySideRelationProperties",
          "ClosedGenericClassWithManySideRelationProperties",
          "TestDomain",
          typeof (ClosedGenericClassWithManySideRelationProperties),
          false);
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (
          GetPropertyDefinition ("BaseUnidirectional"),
          relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (
          GetPropertyDefinition ("BaseBidirectionalOneToOne"),
          relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (
          GetPropertyDefinition ("BaseBidirectionalOneToMany"),
          relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }


    [Test]
    public void IsVirtualEndRelationEndpoint_Unidirectional ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseUnidirectional");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToOne");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      RdbmsRelationEndPointReflector relationEndPointReflector = CreateRelationEndPointReflector ("BaseBidirectionalOneToMany");

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    private RdbmsRelationEndPointReflector CreateRelationEndPointReflector (string propertyName)
    {
      PropertyReflector propertyReflector = CreatePropertyReflector (propertyName);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      return new RdbmsRelationEndPointReflector (_classDefinition, propertyReflector.PropertyInfo);
    }

    private PropertyReflector CreatePropertyReflector (string property)
    {
      Type type = typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>);
      PropertyInfo propertyInfo = type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      return new PropertyReflector (_classDefinition, propertyInfo);
    }

    private PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      return _classDefinition.MyPropertyDefinitions[
          string.Format ("{0}.{1}", typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).FullName, propertyName)];
    }
  }
}