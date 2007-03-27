using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class OneSideRelationProperty: ReflectionBasedMappingTest
  {
    private RdbmsRelationEndPointReflector _relationEndPointReflector;
    private ClassDefinition _classDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();
      _relationEndPointReflector = new RdbmsRelationEndPointReflector ();
    
      _classDefinition = new ClassDefinition (
          "ClassWithOneSideRelationProperties", "ClassWithOneSideRelationProperties", "TestDomain", typeof (ClassWithOneSideRelationProperties));

      _classDefinitions = new ClassDefinitionCollection ();
      _classDefinitions.Add (_classDefinition);
    }

    [Test]
    public void GetMetadata_ForOptional()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NoAttribute");

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
         "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NoAttribute",
         actual.PropertyName);
      Assert.IsFalse (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_ForMandatory()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NotNullable");

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NotNullable",
          actual.PropertyName);
      Assert.IsTrue (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToOne");

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefiniton = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
          relationEndPointDefiniton.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), relationEndPointDefiniton.PropertyType);
      Assert.AreEqual (CardinalityType.One, relationEndPointDefiniton.Cardinality);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToMany");

      IRelationEndPointDefinition actual = _relationEndPointReflector.GetMetadata (_classDefinitions, propertyInfo);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefiniton = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
          relationEndPointDefiniton.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), relationEndPointDefiniton.PropertyType);
      Assert.AreEqual (CardinalityType.Many, relationEndPointDefiniton.Cardinality);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }
  }
}