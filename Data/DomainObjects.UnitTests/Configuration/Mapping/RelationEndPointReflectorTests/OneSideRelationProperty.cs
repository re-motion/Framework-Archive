using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class OneSideRelationProperty: StandardMappingTest
  {
    private ClassDefinition _classDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOneSideRelationProperties", "ClassWithOneSideRelationProperties", "TestDomain", typeof (ClassWithOneSideRelationProperties), false);

      _classDefinitions = new ClassDefinitionCollection ();
      _classDefinitions.Add (_classDefinition);
    }

    [Test]
    public void GetMetadata_ForOptional()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NoAttribute");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

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
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

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
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

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
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata (_classDefinitions);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefiniton = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
          relationEndPointDefiniton.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), relationEndPointDefiniton.PropertyType);
      Assert.AreEqual (CardinalityType.Many, relationEndPointDefiniton.Cardinality);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
      Assert.AreEqual ("The Sort Expression", relationEndPointDefiniton.SortExpression);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Only relation end points with a property type of 'Rubicon.Data.DomainObjects.DomainObject' can contain the foreign key.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: CollectionPropertyContainsKeyLeftSide")]
    public void GetMetadata_WithCollectionPropertyContainingTheKey ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("CollectionPropertyContainsKeyLeftSide");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Only relation end points with a property type of 'Rubicon.Data.DomainObjects.ObjectList`1[T]' can have a sort expression.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: NonCollectionPropertyHavingASortExpressionLeftSide")]
    public void GetMetadata_WithNonCollectionPropertyHavingASortExpression ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("NonCollectionPropertyHavingASortExpressionLeftSide");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
    }
  }
}