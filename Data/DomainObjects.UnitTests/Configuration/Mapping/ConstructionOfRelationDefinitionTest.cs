using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class ConstructionOfRelationDefinitionTest
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
    ClassDefinition customerDefinition = new ClassDefinition ("Customer", "Customer", typeof (Customer), "TestDomain");

    VirtualRelationEndPointDefinition endPointDefinition1 = new VirtualRelationEndPointDefinition (
        customerDefinition, "Orders", true, CardinalityType.One, typeof (Order));
    
    ClassDefinition orderDefinition = new ClassDefinition (
        "Order", "Order", typeof (Order), "TestDomain");

    VirtualRelationEndPointDefinition endPointDefinition2 = new VirtualRelationEndPointDefinition (
        orderDefinition, "Customer", true, CardinalityType.One, typeof (Customer));
 
    RelationDefinition relationDefinition = new RelationDefinition (
        "CustomerToOrder", endPointDefinition1, endPointDefinition2);        
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Relation 'CustomerToOrder' cannot have two non-virtual end points.")]           
  public void TwoRelationEndPointDefinitions ()
  {
    ClassDefinition customerDefinition = new ClassDefinition ("Customer", "Customer", typeof (Customer), "TestDomain");
    customerDefinition.PropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID"));  

    RelationEndPointDefinition endPointDefinition1 = new RelationEndPointDefinition (
        customerDefinition, "Order", false);

    ClassDefinition orderDefinition = new ClassDefinition (
        "Order", "Order", typeof (Order), "TestDomain");

    orderDefinition.PropertyDefinitions.Add (new PropertyDefinition ("Customer", "CustomerID", "objectID"));

    RelationEndPointDefinition endPointDefinition2 = new RelationEndPointDefinition (
        orderDefinition, "Customer", false);
 
    RelationDefinition relationDefinition = new RelationDefinition (
        "CustomerToOrder", endPointDefinition1, endPointDefinition2);        
  }
}
}
