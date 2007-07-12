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
  public class GetRelationDefintionsForClosedGeneric : TestBase
  {
    private RelationDefinitionChecker _relationDefinitionChecker;
    private RelationDefinitionCollection _relationDefinitions;
    private ClassDefinitionCollection _classDefinitions;
    private ClassDefinition _closedGenericClassWithManySideRelationPropertiesClassDefinition;
    private ClassDefinition _closedGenericClassWithOneSideRelationPropertiesClassDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _relationDefinitionChecker = new RelationDefinitionChecker();
      _relationDefinitions = new RelationDefinitionCollection();
      _closedGenericClassWithManySideRelationPropertiesClassDefinition = CreateClosedGenericClassWithManySideRelationPropertiesClassDefinition ();
      _closedGenericClassWithOneSideRelationPropertiesClassDefinition = CreateClosedGenericClassWithOneSideRelationPropertiesClassDefinition ();
      _classDefinitions = new ClassDefinitionCollection ();
      _classDefinitions.Add (_closedGenericClassWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_closedGenericClassWithOneSideRelationPropertiesClassDefinition);
    }

    [Test]
    public void GetRelationDefinitions_ForManySide ()
    {
      RelationDefinitionCollection expectedDefinitions = new RelationDefinitionCollection ();
      expectedDefinitions.Add (CreateBaseUnidirectionalRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToOneRelationDefinition ());
      expectedDefinitions.Add (CreateBaseBidirectionalOneToManyRelationDefinition ());

      ClassReflector classReflector = new ClassReflector (typeof (ClosedGenericClassWithManySideRelationProperties));

      List<RelationDefinition> actualList = classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      _relationDefinitionChecker.Check (expectedDefinitions, _relationDefinitions);
      Assert.That (actualList, Is.EqualTo (_relationDefinitions));
    }

    [Test]
    public void GetRelationDefinitions_ForOneSide ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClosedGenericClassWithOneSideRelationProperties));

      List<RelationDefinition> actual = classReflector.GetRelationDefinitions (_classDefinitions, _relationDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Count);
    }


    private ClassDefinition CreateClosedGenericClassWithOneSideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClosedGenericClassWithOneSideRelationProperties",
          "ClosedGenericClassWithOneSideRelationProperties",
          c_testDomainProviderID,
          typeof (ClosedGenericClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }

    private ClassDefinition CreateClosedGenericClassWithManySideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClosedGenericClassWithManySideRelationProperties",
          "ClosedGenericClassWithManySideRelationProperties",
          c_testDomainProviderID,
          typeof (ClosedGenericClassWithManySideRelationProperties),
          false);

      CreatePropertyDefinitionsForClosedGenericClassWithManySideRelationProperties (classDefinition);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClosedGenericClassWithManySideRelationProperties (ReflectionBasedClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseUnidirectional",
              "BaseUnidirectionalID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToOne",
              "BaseBidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToMany",
              "BaseBidirectionalOneToManyID",
              typeof (ObjectID),
              true,
              null,
              true));

    }

    private RelationDefinition CreateBaseUnidirectionalRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseUnidirectional",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseUnidirectional",
              false),
          CreateAnonymousRelationEndPointDefinition ());
    }

    private RelationDefinition CreateBaseBidirectionalOneToOneRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToOne",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToOne", 
              false),
          CreateVirtualRelationEndPointDefinitionForManySide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithOneSideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToOne", 
              false));
    }

    private RelationDefinition CreateBaseBidirectionalOneToManyRelationDefinition ()
    {
      return new RelationDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToMany",
          CreateRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithManySideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToMany",
              false),
          CreateVirtualRelationEndPointDefinitionForOneSide ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.GenericClassWithOneSideRelationPropertiesNotInMapping`1.BaseBidirectionalOneToMany",
              false, "The Sort Expression"));
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (string propertyName, bool isMandatory)
    {
      return new RelationEndPointDefinition (_closedGenericClassWithManySideRelationPropertiesClassDefinition, propertyName, isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForManySide (string propertyName, bool isMandatory)
    {
      return new VirtualRelationEndPointDefinition (
          _closedGenericClassWithOneSideRelationPropertiesClassDefinition,
          propertyName,
          isMandatory,
          CardinalityType.One,
          typeof (ClosedGenericClassWithManySideRelationProperties));
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinitionForOneSide (string propertyName, bool isMandatory, string sortExpression)
    {
      return new VirtualRelationEndPointDefinition (
          _closedGenericClassWithOneSideRelationPropertiesClassDefinition,
          propertyName,
          isMandatory,
          CardinalityType.Many,
          typeof (ObjectList<ClosedGenericClassWithManySideRelationProperties>),
          sortExpression);
    }

    private AnonymousRelationEndPointDefinition CreateAnonymousRelationEndPointDefinition ()
    {
      return new AnonymousRelationEndPointDefinition (_closedGenericClassWithOneSideRelationPropertiesClassDefinition);
    }
  }
}