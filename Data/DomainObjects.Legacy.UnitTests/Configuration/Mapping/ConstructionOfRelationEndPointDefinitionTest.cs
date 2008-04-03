using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Factories;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationEndPointDefinitionTest : StandardMappingTest
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
        ExpectedMessage = "Relation definition error: Property 'Name' of class 'Company' is of type 'System.String',"
            + " but non-virtual properties must be of type 'Remotion.Data.DomainObjects.ObjectID'.")]
    public void PropertyOfWrongType ()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      RelationEndPointDefinition endPointDefinition = new RelationEndPointDefinition (
          companyDefinition, "Name", false);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Relation definition error for end point: Class 'Company' has no property 'UndefinedProperty'.")]
    public void UndefinedProperty ()
    {
      ClassDefinition companyDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      RelationEndPointDefinition endPointDefinition = new RelationEndPointDefinition (
          companyDefinition, "UndefinedProperty", false);
    }
  }
}
