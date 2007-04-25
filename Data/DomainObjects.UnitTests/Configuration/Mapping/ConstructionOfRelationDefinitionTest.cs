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
      ClassDefinition customerDefinition = new ReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false);

      VirtualRelationEndPointDefinition endPointDefinition1 = new VirtualRelationEndPointDefinition (
          customerDefinition, "Orders", true, CardinalityType.One, typeof (Order));

      ClassDefinition orderDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);

      VirtualRelationEndPointDefinition endPointDefinition2 = new VirtualRelationEndPointDefinition (
          orderDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", true, CardinalityType.One, typeof (Customer));

      new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", endPointDefinition1, endPointDefinition2);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson' cannot have two non-virtual end points.")]
    public void TwoRelationEndPointDefinitions ()
    {
      ClassDefinition partnerDefinition = new ReflectionBasedClassDefinition ("Partner", "Partner", "TestDomain", typeof (Partner), false);
      partnerDefinition.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", "ContactPersonID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition1 = new RelationEndPointDefinition (
          partnerDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", false);

      ClassDefinition personDefinition = new ReflectionBasedClassDefinition ("Person", "Person", "TestDomain", typeof (Person), false);
      personDefinition.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", "AssociatedPartnerCompanyID", typeof (ObjectID)));

      RelationEndPointDefinition endPointDefinition2 = new RelationEndPointDefinition (
          personDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", false);

      new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", endPointDefinition1, endPointDefinition2);
    }
  }
}
