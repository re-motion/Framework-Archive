using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ClassReflectorTests
{
  [TestFixture]
  public class GetRelationDefintions : TestBase
  {
    private RelationDefinitionChecker _relationDefinitionChecker;
    private RelationDefinitionCollection _relationDefinitions;
    private ClassDefinitionCollection _classDefinitions;
    private ClassDefinition _classWithManySideRelationPropertiesClassDefinition;
    private ClassDefinition _classWithOneSideRelationPropertiesClassDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _relationDefinitionChecker = new RelationDefinitionChecker();
      _relationDefinitions = new RelationDefinitionCollection();
      _classWithManySideRelationPropertiesClassDefinition = CreateClassWithManySideRelationPropertiesClassDefinition();
      _classWithOneSideRelationPropertiesClassDefinition = CreateClassWithOneSideRelationPropertiesClassDefinition();
      _classDefinitions = new ClassDefinitionCollection ();
      _classDefinitions.Add (_classWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithOneSideRelationPropertiesClassDefinition);
    }

    [Test]
    public void GetRelationDefinitions_ForManySide ()
    {
      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection();
      expectedDefinitions.Add (CreateBaseUnidirectionalRelationDefinition());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition());
      expectedDefinitions.Add (CreateBasePrivateUnidirectionalRelationDefinition());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToOneRelationDefinition());
      expectedDefinitions.Add (CreateBasePrivateBidirectionalOneToManyRelationDefinition());
      expectedDefinitions.Add (CreateNoAttributeRelationDefinition());
      expectedDefinitions.Add (CreateNotNullableRelationDefinition());
      expectedDefinitions.Add (CreateUnidirectionalRelationDefinition());
      expectedDefinitions.Add (CreateBidirectionalOneToOneRelationDefinition());
      expectedDefinitions.Add (CreateBidirectionalOneToManyRelationDefinition());

      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManySideRelationProperties));

      List<RelationDefinition> actualList = classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
      Assert.That (actualList, Is.EqualTo (_relationDefinitions));
    }

    [Test]
    public void GetRelationDefinitions_ForOneSide ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithOneSideRelationProperties));

      List<RelationDefinition> actual = classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Count);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Mapping does not contain class 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties'.",
        MatchType = MessageMatch.Contains)]
    public void GetRelationDefinitions_WithMissingClassDefinition ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManySideRelationProperties));
      classReflector.GetRelationDefinitions (new ClassDefinitionCollection(), _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Mapping does not contain class 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties'.",
        MatchType = MessageMatch.Contains)]
    public void GetRelationDefinitions_WithMissingOppositeClassDefinitionForBidirectionalRelation ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManySideRelationProperties));
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      classDefinitions.Add (CreateClassWithManySideRelationPropertiesClassDefinition());
      classReflector.GetRelationDefinitions (classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Mapping does not contain class 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties'.",
        MatchType = MessageMatch.Contains)]
    public void GetRelationDefinitions_WithMissingOppositeClassDefinitionForUnidirectionalRelation ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperties));
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      classDefinitions.Add (CreateClassWithMixedPropertiesClassDefinition());
      classReflector.GetRelationDefinitions (classDefinitions, _relationDefinitions);
    }

    private ClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          c_testDomainProviderID,
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithManySideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithManySideRelationProperties",
          "ClassWithManySideRelationProperties",
          c_testDomainProviderID,
          typeof (ClassWithManySideRelationProperties),
          false);

      CreatePropertyDefinitionsForClassWithManySideRelationProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithMixedPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithMixedProperties",
          "ClassWithMixedProperties",
          c_testDomainProviderID,
          typeof (ClassWithMixedProperties),
          false);

      CreatePropertyDefinitionsForClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClassWithManySideRelationProperties (ReflectionBasedClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseUnidirectional",
              "BaseUnidirectionalID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne",
              "BaseBidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany",
              "BaseBidirectionalOneToManyID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateUnidirectional",
              "BasePrivateUnidirectionalID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToOne",
              "BasePrivateBidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToMany",
              "BasePrivateBidirectionalOneToManyID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NoAttribute",
              "NoAttributeID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
              "NotNullableID",
              typeof (ObjectID),
              false,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.Unidirectional",
              "UnidirectionalID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
              "BidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
              "BidirectionalOneToManyID",
              typeof (ObjectID),
              true,
              null,
              true));
    }

    private RelationDefinition CreateNoAttributeRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NoAttribute",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NoAttribute", 
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NoAttribute",
              false, null));
    }

    private RelationDefinition CreateNotNullableRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.NotNullable", 
              true),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.NotNullable",
              true, null));
    }

    private RelationDefinition CreateBaseUnidirectionalRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseUnidirectional",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseUnidirectional", 
              false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateBasePrivateUnidirectionalRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateUnidirectional",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateUnidirectional",
              false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateUnidirectionalRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.Unidirectional",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.Unidirectional", false),
          CreateAnonymousRelationEndPointDefinition());
    }

    private RelationDefinition CreateBaseBidirectionalOneToOneRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBasePrivateBidirectionalOneToOneRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBidirectionalOneToOneRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBaseBidirectionalOneToManyRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany", 
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany",
              false, "The Sort Expression"));
    }

    private RelationDefinition CreateBasePrivateBidirectionalOneToManyRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BasePrivateBidirectionalOneToMany",
              false, "The Sort Expression"));
    }

    private RelationDefinition CreateBidirectionalOneToManyRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany", 
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
              false, "The Sort Expression"));
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

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForOneSide (string propertyName, bool isMandatory, string sortExpression)
    {
      return new VirtualRelationEndPointDefinition (
          _classWithOneSideRelationPropertiesClassDefinition,
          propertyName,
          isMandatory,
          CardinalityType.Many,
          typeof (ObjectList<ClassWithManySideRelationProperties>),
          sortExpression);
    }

    private AnonymousRelationEndPointDefinition CreateAnonymousRelationEndPointDefinition ()
    {
      return new AnonymousRelationEndPointDefinition (_classWithOneSideRelationPropertiesClassDefinition);
    }
  }
}