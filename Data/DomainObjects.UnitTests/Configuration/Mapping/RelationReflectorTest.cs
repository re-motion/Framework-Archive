using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
    private ClassDefinition _classWithBothEndPointsOnSameClassClassDefinition;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();
      _classWithManySideRelationPropertiesClassDefinition = new ReflectionBasedClassDefinition (
          "ClassWithManySideRelationProperties", "ClassWithManySideRelationProperties", "TestDomain", typeof (ClassWithManySideRelationProperties));
      _classWithOneSideRelationPropertiesClassDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOneSideRelationProperties", "ClassWithOneSideRelationProperties", "TestDomain", typeof (ClassWithOneSideRelationProperties));
      _classWithBothEndPointsOnSameClassClassDefinition = new ReflectionBasedClassDefinition (
          "ClassWithBothEndPointsOnSameClass", "ClassWithBothEndPointsOnSameClass", "TestDomain", typeof (ClassWithBothEndPointsOnSameClass));

      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_classWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithOneSideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithBothEndPointsOnSameClassClassDefinition);
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToOne");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToOne",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (NullRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      NullRelationEndPointDefinition oppositeEndPointDefinition = (NullRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_UnidirectionalOneToMany()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToMany");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.UnidirectionalOneToMany",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (NullRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      NullRelationEndPointDefinition oppositeEndPointDefinition = (NullRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToOne");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);

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
      VirtualRelationEndPointDefinition oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToMany");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);

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
      VirtualRelationEndPointDefinition oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToManyWithBothEndPointsOnSameClass ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithBothEndPointsOnSameClass).GetProperty ("Parent");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classWithBothEndPointsOnSameClassClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Parent",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithBothEndPointsOnSameClassClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithBothEndPointsOnSameClassClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Children",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithBothEndPointsOnSameClass>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      
      Assert.That (_classWithBothEndPointsOnSameClassClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
    }
  }
}