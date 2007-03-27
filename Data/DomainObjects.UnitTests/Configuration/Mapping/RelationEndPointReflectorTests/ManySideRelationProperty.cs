using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class ManySideRelationProperty: ReflectionBasedMappingTest
  {
    private ClassDefinition _classDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinition = new ClassDefinition (
          "ClassWithManySideRelationProperties", "ClassWithManySideRelationProperties", "TestDomain", typeof (ClassWithManySideRelationProperties));

      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_classDefinition);
    }

    [Test]
    public void GetMetadata_ForOptional()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("NoAttribute");
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
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("NotNullable");
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
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("UnidirectionalOneToOne");
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
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("UnidirectionalOneToMany");
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
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("BidirectionalOneToOne");
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
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithManySideRelationProperties> ("BidirectionalOneToMany");
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

    private PropertyReflector CreatePropertyReflector<T> (string property)
    {
      PropertyInfo propertyInfo = typeof (T).GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      return new PropertyReflector (propertyInfo);
    }
  }
}