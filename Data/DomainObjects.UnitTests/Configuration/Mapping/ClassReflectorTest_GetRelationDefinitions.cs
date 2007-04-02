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

      List<RelationDefinition> actual = classReflector.GetRelationDefinitions (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (6, actual.Count);

      _relationDefinitionChecker.Check (CreateNoAttributeRelationDefinition(), actual[0]);
      _relationDefinitionChecker.Check (CreateNotNullableRelationDefinition(), actual[1]);
      _relationDefinitionChecker.Check (CreateUnidirectionalOneToOneRelationDefinition(), actual[2]);
      _relationDefinitionChecker.Check (CreateUnidirectionalOneToManyRelationDefinition(), actual[3]);
      _relationDefinitionChecker.Check (CreateBidirectionalOneToOneRelationDefinition(), actual[4]);
      _relationDefinitionChecker.Check (CreateBidirectionalOneToManyRelationDefinition(), actual[5]);
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
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          "TestDomain",
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithManySideRelationPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
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
      ClassDefinition classDefinition = new ClassDefinition (
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
          CreateVirtualRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NoAttribute",
              false,
              CardinalityType.Many,
              typeof (ObjectList<ClassWithManySideRelationProperties>)));
    }

    private RelationDefinition CreateNotNullableRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
              true),
          CreateVirtualRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NotNullable",
              true,
              CardinalityType.Many,
              typeof (ObjectList<ClassWithManySideRelationProperties>)));
    }

    private RelationDefinition CreateUnidirectionalOneToOneRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToOne",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToOne",
              false),
          CreateNullRelationEndPointDefinition());
    }

    private RelationDefinition CreateUnidirectionalOneToManyRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToMany",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToMany",
              false),
          CreateNullRelationEndPointDefinition());
    }

    private RelationDefinition CreateBidirectionalOneToOneRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
              false),
          CreateVirtualRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
              false,
              CardinalityType.One,
              typeof (ClassWithManySideRelationProperties)));
    }

    private RelationDefinition CreateBidirectionalOneToManyRelationDefinition()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
          CreateRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
              false,
              CardinalityType.Many,
              typeof (ObjectList<ClassWithManySideRelationProperties>)));
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (string propertyName, bool isMandatory)
    {
      return new RelationEndPointDefinition (_classWithManySideRelationPropertiesClassDefinition, propertyName, isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (
        string propertyName, bool isMandatory, CardinalityType cardinality, Type propertyType)
    {
      return new VirtualRelationEndPointDefinition (
          _classWithOneSideRelationPropertiesClassDefinition, propertyName, isMandatory, cardinality, propertyType);
    }

    private NullRelationEndPointDefinition CreateNullRelationEndPointDefinition()
    {
      return new NullRelationEndPointDefinition (_classWithOneSideRelationPropertiesClassDefinition);
    }
  }
}