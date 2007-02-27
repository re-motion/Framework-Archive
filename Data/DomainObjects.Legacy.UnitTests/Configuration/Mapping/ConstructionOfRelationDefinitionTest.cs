using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfRelationDefinitionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ConstructionOfRelationDefinitionTest ()
    {
    }

    // methods and properties

    [Test]
    [ExpectedException (typeof (MappingException), "Relation 'CustomerToOrder' cannot have two virtual end points.")]
    public void TwoVirtualRelationEndPointDefinitions ()
    {
      ClassDefinition customerDefinition = new ClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer));

      VirtualRelationEndPointDefinition endPointDefinition1 = new VirtualRelationEndPointDefinition (
          customerDefinition, "Orders", true, CardinalityType.One, typeof (Order));

      ClassDefinition orderDefinition = new ClassDefinition ("Order", "Order", "TestDomain", typeof (Order));

      VirtualRelationEndPointDefinition endPointDefinition2 = new VirtualRelationEndPointDefinition (
          orderDefinition, "Customer", true, CardinalityType.One, typeof (Customer));

      RelationDefinition relationDefinition = new RelationDefinition (
          "CustomerToOrder", endPointDefinition1, endPointDefinition2);
    }

    [Test]
    [ExpectedException (typeof (MappingException), "Relation 'CustomerToOrder' cannot have two non-virtual end points.")]
    public void TwoRelationEndPointDefinitions ()
    {
      ClassDefinition customerDefinition = new ClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer));
      customerDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", TypeInfo.ObjectIDMappingTypeName));

      RelationEndPointDefinition endPointDefinition1 = new RelationEndPointDefinition (
          customerDefinition, "Order", false);

      ClassDefinition orderDefinition = new ClassDefinition ("Order", "Order", "TestDomain", typeof (Order));

      orderDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName));

      RelationEndPointDefinition endPointDefinition2 = new RelationEndPointDefinition (orderDefinition, "Customer", false);

      RelationDefinition relationDefinition = new RelationDefinition ("CustomerToOrder", endPointDefinition1, endPointDefinition2);
    }
  }
}
