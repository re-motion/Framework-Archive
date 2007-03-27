using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationReflectorTest: ReflectionBasedMappingTest
  {
    private ClassDefinition _classWithManySideRelationPropertiesClassDefinition;
    private ClassDefinition _classWithOneSideRelationPropertiesClassDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();
      _classWithManySideRelationPropertiesClassDefinition = new ClassDefinition (
          "ClassWithManySideRelationProperties", "ClassWithManySideRelationProperties", "TestDomain", typeof (ClassWithManySideRelationProperties));
      _classWithOneSideRelationPropertiesClassDefinition = new ClassDefinition (
          "ClassWithOneSideRelationProperties", "ClassWithOneSideRelationProperties", "TestDomain", typeof (ClassWithOneSideRelationProperties));

      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_classWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithOneSideRelationPropertiesClassDefinition);
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToOne");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actual = relationReflector.GetMetadata (_classDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToOne",
          actual.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actual.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actual, endPointDefinition.RelationDefinition);

      Assert.IsInstanceOfType (typeof (NullRelationEndPointDefinition), actual.EndPointDefinitions[1]);
      NullRelationEndPointDefinition oppositeEndPointDefinition = (NullRelationEndPointDefinition) actual.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actual, oppositeEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToMany()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToMany");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actual = relationReflector.GetMetadata (_classDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToMany",
          actual.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actual.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actual, endPointDefinition.RelationDefinition);

      Assert.IsInstanceOfType (typeof (NullRelationEndPointDefinition), actual.EndPointDefinitions[1]);
      NullRelationEndPointDefinition oppositeEndPointDefinition = (NullRelationEndPointDefinition) actual.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actual, oppositeEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToOne");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actual = relationReflector.GetMetadata (_classDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
          actual.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actual.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actual, endPointDefinition.RelationDefinition);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actual.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actual, oppositeEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToMany");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actual = relationReflector.GetMetadata (_classDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
          actual.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actual.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actual, endPointDefinition.RelationDefinition);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actual.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actual, oppositeEndPointDefinition.RelationDefinition);
    }
  }
}