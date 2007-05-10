using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class ManySideRelationProperty: StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithManySideRelationProperties", "ClassWithManySideRelationProperties", "TestDomain", typeof (ClassWithManySideRelationProperties), false);

      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_classDefinition);
    }

    [Test]
    public void GetMetadata_ForOptional()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("NoAttribute");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      Assert.IsFalse (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_ForMandatory()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("NotNullable");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      Assert.IsTrue (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToOne()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("UnidirectionalOneToOne");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToMany()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("UnidirectionalOneToMany");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("BidirectionalOneToOne");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("BidirectionalOneToMany");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }


    [Test]
    public void IsVirtualEndRelationEndpoint_UnidirectionalOneToOne ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("UnidirectionalOneToOne");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_UnidirectionalOneToMany ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("UnidirectionalOneToMany");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("BidirectionalOneToOne");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector ("BidirectionalOneToMany");
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyReflector.PropertyInfo);

      Assert.IsFalse (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    private PropertyReflector CreatePropertyReflector (string property)
    {
      Type type = typeof (ClassWithManySideRelationProperties);
      PropertyInfo propertyInfo = type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (type.Name, type.Name, c_testDomainProviderID, type, true);

      return new PropertyReflector (_classDefinition, propertyInfo);
    }
  }
}