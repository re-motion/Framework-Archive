using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationDefinitionTest : ReflectionBasedMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' cannot have two virtual end points.")]
    public void TwoVirtualRelationEndPointDefinitions ()
    {
      ClassDefinition customerDefinition = new ReflectionBasedClassDefinition ((string) "Customer", (string) "Customer", (string) "TestDomain", typeof (Customer), (bool) false);

      VirtualRelationEndPointDefinition endPointDefinition1 = new VirtualRelationEndPointDefinition (
          customerDefinition, "Orders", true, CardinalityType.One, typeof (Order));

      ClassDefinition orderDefinition = new ReflectionBasedClassDefinition ((string) "Order", (string) "Order", (string) "TestDomain", typeof (Order), (bool) false);

      VirtualRelationEndPointDefinition endPointDefinition2 = new VirtualRelationEndPointDefinition (
          orderDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", true, CardinalityType.One, typeof (Customer));

      new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", endPointDefinition1, endPointDefinition2);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson' cannot have two non-virtual end points.")]
    public void TwoRelationEndPointDefinitions ()
    {
      ClassDefinition partnerDefinition = new ReflectionBasedClassDefinition ((string) "Partner", (string) "Partner", (string) "TestDomain", typeof (Partner), (bool) false);
      partnerDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", "ContactPersonID", TypeInfo.ObjectIDMappingTypeName));

      RelationEndPointDefinition endPointDefinition1 = new RelationEndPointDefinition (
          partnerDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", false);

      ClassDefinition personDefinition = new ReflectionBasedClassDefinition ((string) "Person", (string) "Person", (string) "TestDomain", typeof (Person), (bool) false);
      personDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", "AssociatedPartnerCompanyID", TypeInfo.ObjectIDMappingTypeName));

      RelationEndPointDefinition endPointDefinition2 = new RelationEndPointDefinition (
          personDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", false);

      new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", endPointDefinition1, endPointDefinition2);
    }
  }
}
