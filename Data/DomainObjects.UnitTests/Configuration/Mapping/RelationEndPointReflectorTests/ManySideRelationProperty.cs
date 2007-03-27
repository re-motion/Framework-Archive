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
    private PropertyReflector _propertyReflector;
    private RdbmsRelationEndPointReflector _relationEndPointReflector;
    private ClassDefinition _classDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();
      _propertyReflector = new PropertyReflector();
      _relationEndPointReflector = new RdbmsRelationEndPointReflector ();
    
      _classDefinition = new ClassDefinition (
          "ClassWithManySideRelationProperties", "ClassWithManySideRelationProperties", "TestDomain", typeof (ClassWithManySideRelationProperties));

      _classDefinitions = new ClassDefinitionCollection ();
      _classDefinitions.Add (_classDefinition);
    }

    [Test]
    public void GetMetadata_ForOptional()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("NoAttribute");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      Assert.IsFalse (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_ForMandatory()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("NotNullable");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      Assert.IsTrue (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToOne");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToMany()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToMany");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToOne");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToMany");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }
  }
}