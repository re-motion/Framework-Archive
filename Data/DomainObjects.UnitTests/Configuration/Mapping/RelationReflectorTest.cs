using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationReflectorTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classWithManySideRelationPropertiesClassDefinition;
    private ReflectionBasedClassDefinition _classWithOneSideRelationPropertiesClassDefinition;
    private ReflectionBasedClassDefinition _classWithBothEndPointsOnSameClassClassDefinition;
    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    public override void SetUp ()
    {
      base.SetUp();
      _classWithManySideRelationPropertiesClassDefinition = CreateReflectionBasedClassDefinition(typeof (ClassWithManySideRelationProperties));
      _classWithOneSideRelationPropertiesClassDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties));
      _classWithBothEndPointsOnSameClassClassDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithBothEndPointsOnSameClass));

      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_classWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithOneSideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithBothEndPointsOnSameClassClassDefinition);

      _relationDefinitions = new RelationDefinitionCollection();
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("Unidirectional");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo, _classDefinitions);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_relationDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.Unidirectional",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (AnonymousRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      AnonymousRelationEndPointDefinition oppositeEndPointDefinition =
          (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToOne");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo, _classDefinitions);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_relationDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToOne");
      RelationReflector relationReflector = new RelationReflector (propertyInfo, _classDefinitions);

      Assert.IsNull (relationReflector.GetMetadata (_relationDefinitions));
      //Assert.IsNotNull (actualRelationDefinition);


      //Assert.AreEqual (
      //    "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
      //    actualRelationDefinition.ID);

      //Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      //RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      //Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      //Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      //Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      //Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      //Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      //VirtualRelationEndPointDefinition oppositeEndPointDefinition =
      //    (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      //Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      //Assert.AreEqual (
      //    "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
      //    oppositeEndPointDefinition.PropertyName);
      //Assert.AreSame (typeof (ClassWithManySideRelationProperties), oppositeEndPointDefinition.PropertyType);
      //Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      //Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToMany");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo, _classDefinitions);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_relationDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToMany");
      RelationReflector relationReflector = new RelationReflector (propertyInfo, _classDefinitions);

      Assert.IsNull (relationReflector.GetMetadata (_relationDefinitions));

      //Assert.AreEqual (
      //    "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
      //    actualRelationDefinition.ID);

      //Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      //RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      //Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      //Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      //Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      //Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      //Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      //VirtualRelationEndPointDefinition oppositeEndPointDefinition =
      //    (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      //Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      //Assert.AreEqual (
      //    "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
      //    oppositeEndPointDefinition.PropertyName);
      //Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), oppositeEndPointDefinition.PropertyType);
      //Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      //Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      //Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      //Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithBothEndPointsOnSameClass).GetProperty ("Parent");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithBothEndPointsOnSameClassClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithBothEndPointsOnSameClassClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo, _classDefinitions);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_relationDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Parent",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithBothEndPointsOnSameClassClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithBothEndPointsOnSameClassClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Children",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithBothEndPointsOnSameClass>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);

      Assert.That (_classWithBothEndPointsOnSameClassClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The property type of an uni-directional relation property must be assignable to Rubicon.Data.DomainObjects.DomainObject.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation, "
        + "property: LeftSide")]
    public void GetMetadata_WithUnidirectionalCollectionProperty ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation", true, false);
      PropertyInfo propertyInfo = type.GetProperty ("LeftSide");
      
      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Opposite relation property 'Invalid' could not be found on type "
        + "'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositePropertyNameLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyName ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidOppositePropertyNameLeftSide");

      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Opposite relation property 'InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide' declared on type declared on type "
        + "'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "defines a 'Rubicon.Data.DomainObjects.DBBidirectionalRelationAttribute' whose opposite property does not match.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide")]
    public void GetMetadata_WithInvalidPropertyNameInBidirectionalRelationAttributeOnOppositeProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide");

      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection ());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The declaring type does not match the type of the opposite relation propery 'InvalidOppositePropertyTypeRightSide' declared on type "
        + "'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositePropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyType ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidOppositePropertyTypeLeftSide");
     
      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The declaring type does not match the type of the opposite relation propery 'InvalidOppositeCollectionPropertyTypeRightSide' declared on type "
        + "'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositeCollectionPropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyTypeForCollectionProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidOppositeCollectionPropertyTypeLeftSide");

      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection ());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Opposite relation property 'MissingBidirectionalRelationAttributeRightSide' declared on type "
        + "'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "does not define a matching 'Rubicon.Data.DomainObjects.DBBidirectionalRelationAttribute'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: MissingBidirectionalRelationAttributeLeftSide")]
    public void GetMetadata_WithMissingBidirectionalRelationAttributeOnOppositeProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("MissingBidirectionalRelationAttributeLeftSide");

      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection ());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Opposite relation property 'MissingBidirectionalRelationAttributeForCollectionPropertyRightSide' declared on type "
        + "'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "does not define a matching 'Rubicon.Data.DomainObjects.DBBidirectionalRelationAttribute'.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide")]
    public void GetMetadata_WithMissingBidirectionalRelationAttributeOnOppositeCollectionProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide");

      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection ());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "A bidirectional relation can only have one virtual relation end point.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: NoContainsKeyLeftSide")]
    public void GetMetadata_WithTwoVirtualEndPoints ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
      PropertyInfo propertyInfo = type.GetProperty ("NoContainsKeyLeftSide");

      RelationReflector relationReflector = new RelationReflector (propertyInfo, new ClassDefinitionCollection ());
      relationReflector.GetMetadata (_relationDefinitions);
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false);
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
    }
  }
}