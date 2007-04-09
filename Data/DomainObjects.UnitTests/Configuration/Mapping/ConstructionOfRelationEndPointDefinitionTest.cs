using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationEndPointDefinitionTest: ReflectionBasedMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Relation definition error: Property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name' of class 'Company' is of type "
        + "'System.String', but non-virtual properties must be of type 'Rubicon.Data.DomainObjects.ObjectID'.")]
    public void PropertyOfWrongType()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      new RelationEndPointDefinition (companyDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name", false);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Relation definition error for end point: Class 'Company' has no property 'UndefinedProperty'.")]
    public void UndefinedProperty()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      new RelationEndPointDefinition (companyDefinition, "UndefinedProperty", false);
    }
  }
}