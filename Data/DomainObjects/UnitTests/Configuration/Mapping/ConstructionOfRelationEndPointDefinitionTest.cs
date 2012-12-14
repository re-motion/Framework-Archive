using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class ConstructionOfRelationEndPointDefinitionTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ConstructionOfRelationEndPointDefinitionTest ()
  {
  }

  // methods and properties

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Relation definition error: Property 'Name' of class 'Company' is of type 'System.String',"
          + " but non-virtual properties must be of type 'Rubicon.Data.DomainObjects.ObjectID'.")]
  public void PropertyOfWrongType ()
  {
    ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

    RelationEndPointDefinition endPointDefinition = new RelationEndPointDefinition (
        companyDefinition, "Name", false);
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Relation definition error for end point: Class 'Company' has no property 'UndefinedProperty'.")]  
  public void UndefinedProperty ()
  {
    ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

    RelationEndPointDefinition endPointDefinition = new RelationEndPointDefinition (
        companyDefinition, "UndefinedProperty", false);
  }
}
}
