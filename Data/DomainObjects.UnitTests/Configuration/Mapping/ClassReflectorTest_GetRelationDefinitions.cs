using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ClassReflectorTest_GetRelationDefintions: ClassReflectorTestBase
  {
    private RelationDefinitionChecker _relationDefinitionChecker;
    private ClassDefinitionCollection _classDefinitions;
    private ClassDefinition _classWithManySideRelationPropertiesClassDefinition;
    private ClassDefinition _classWithOneSideRelationPropertiesClassDefinition;

    public override void SetUp()
    {
      base.SetUp();

      _relationDefinitionChecker = new RelationDefinitionChecker();
      _classWithManySideRelationPropertiesClassDefinition = CreateClassWithManySideRelationPropertiesClassDefinition();
      _classWithOneSideRelationPropertiesClassDefinition = CreateClassWithOneSideRelationPropertiesClassDefinition();
      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_classWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithOneSideRelationPropertiesClassDefinition);
    }

    [Test]
    public void GetRelationDefinitions_ForManySide()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManySideRelationProperties));

      List<RelationDefinition> actualList = classReflector.GetRelationDefinitions (_classDefinitions);
      Assert.IsNotNull (actualList);

      RelationDefinitionCollection actualDefinitions = new RelationDefinitionCollection();
      foreach (RelationDefinition definition in actualList)
        actualDefinitions.Add (definition);

      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection();
      expectedDefinitions.Add (CreateNoAttributeRelationDefinition());
      expectedDefinitions.Add (CreateNotNullableRelationDefinition());
      expectedDefinitions.Add (CreateUnidirectionalOneToOneRelationDefinition());
      expectedDefinitions.Add (CreateUnidirectionalOneToManyRelationDefinition());
      expectedDefinitions.Add (CreateBidirectionalOneToOneRelationDefinition());
      expectedDefinitions.Add (CreateBidirectionalOneToManyRelationDefinition());

      _relationDefinitionChecker.Check (expectedDefinitions, actualDefinitions);
    }

    [Test]
    public void GetRelationDefinitions_ForOneSide()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithOneSideRelationProperties));

      List<RelationDefinition> actual = classReflector.GetRelationDefinitions (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Count);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void GetRelationDefinitions_WithMissingClassDefinition()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManySideRelationProperties));

      try
      {
        classReflector.GetRelationDefinitions (new ClassDefinitionCollection());
      }
      catch (MappingException e)
      {
        StringAssert.StartsWith (
            "Mapping does not contain class 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties'.",
            e.Message);

        throw;
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void GetRelationDefinitions_WithMissingOppositeClassDefinitionForBidirectionalRelation()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManySideRelationProperties));
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      classDefinitions.Add (CreateClassWithManySideRelationPropertiesClassDefinition());

      try
      {
        classReflector.GetRelationDefinitions (classDefinitions);
      }
      catch (MappingException e)
      {
        StringAssert.StartsWith (
            "Mapping does not contain class 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties'.",
            e.Message);

        throw;
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void GetRelationDefinitions_WithMissingOppositeClassDefinitionForUnidirectionalRelation()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperties));
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      classDefinitions.Add (CreateClassWithMixedPropertiesClassDefinition());

      try
      {
        classReflector.GetRelationDefinitions (classDefinitions);
      }
      catch (MappingException e)
      {
        StringAssert.StartsWith (
            "Mapping does not contain class 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties'.",
            e.Message);

        throw;
      }
    }

    private ClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          "TestDomain",
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithManySideRelationPropertiesClassDefinition()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithManySideRelationProperties",
          "ClassWithManySideRelationProperties",
          "TestDomain",
          typeof (ClassWithManySideRelationProperties),
          false);

      CreatePropertyDefinitionsForClassWithManySideRelationProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithMixedPropertiesClassDefinition()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithMixedProperties",
          "ClassWithMixedProperties",
          "TestDomain",
          typeof (ClassWithMixedProperties),
          false);

      CreatePropertyDefinitionsForClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClassWithManySideRelationProperties (ClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NoAttribute",
              "NoAttributeID",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
              "NotNullableID",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              false,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToOne",
              "UnidirectionalOneToOneID",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToMany",
              "UnidirectionalOneToManyID",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
              "BidirectionalOneToOneID",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
              "BidirectionalOneToManyID",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              true,
              NaInt32.Null,
              true));
    }

    private RelationDefinition CreateNoAttributeRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NoAttribute",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NoAttribute",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NoAttribute",
              false,
              null));
    }

    private RelationDefinition CreateNotNullableRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
              true),
          CreateVirtualRelationEndPointDefinitionForOneSide (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NotNullable",
              true,
              null));
    }

    private RelationDefinition CreateUnidirectionalOneToOneRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToOne",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToOne",
              false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateUnidirectionalOneToManyRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToMany",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToMany",
              false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateBidirectionalOneToOneRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
              false),
          CreateVirtualRelationEndPointDefinitionForManySide (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
              false));
    }

    private RelationDefinition CreateBidirectionalOneToManyRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
              false,
              "The Sort Expression"));
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (string propertyName, bool isMandatory)
    {
      return new RelationEndPointDefinition (_classWithManySideRelationPropertiesClassDefinition, propertyName, isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForManySide (string propertyName, bool isMandatory)
    {
      return new VirtualRelationEndPointDefinition (
          _classWithOneSideRelationPropertiesClassDefinition,
          propertyName,
          isMandatory,
          CardinalityType.One,
          typeof (ClassWithManySideRelationProperties));
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForOneSide (
        string propertyName, bool isMandatory, string sortExpression)
    {
      return new VirtualRelationEndPointDefinition (
          _classWithOneSideRelationPropertiesClassDefinition,
          propertyName,
          isMandatory,
          CardinalityType.Many,
          typeof (ObjectList<ClassWithManySideRelationProperties>),
          sortExpression);
    }

    private AnonymousRelationEndPointDefinition CreateAnonymousRelationEndPointDefinition()
    {
      return new AnonymousRelationEndPointDefinition (_classWithOneSideRelationPropertiesClassDefinition);
    }
  }
}