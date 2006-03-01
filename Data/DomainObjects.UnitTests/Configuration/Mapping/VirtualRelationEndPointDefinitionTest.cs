using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class VirtualRelationEndPointDefinitionTest
{
  // types

  // static members and constants

  // member fields

  private VirtualRelationEndPointDefinition _customerEndPoint;
  private RelationEndPointDefinition _orderEndPoint;

  // construction and disposing

  public VirtualRelationEndPointDefinitionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void Setup ()
  {
    RelationDefinition customerToOrder = TestMappingConfiguration.Current.RelationDefinitions["CustomerToOrder"];
    
    _customerEndPoint = (VirtualRelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
        "Customer", "Orders");

    _orderEndPoint = (RelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
        "Order", "Customer");
  }

  [Test]
  public void InitializeWithPropertyTypeName ()
  {
    VirtualRelationEndPointDefinition endPoint = new VirtualRelationEndPointDefinition (
        ClassDefinitionFactory.CreateOrderDefinition (),
        "VirtualEndPoint", true, CardinalityType.One, 
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem, Rubicon.Data.DomainObjects.UnitTests", null);

    Assert.IsTrue (endPoint.IsPropertyTypeResolved);
    Assert.AreSame (typeof (OrderItem), endPoint.PropertyType);
    Assert.AreEqual (typeof(OrderItem).AssemblyQualifiedName, endPoint.PropertyTypeName);
  }

  // TODO: Write test for unresolved VirtualEndPointDefinition.

  [Test]
  public void IsNull ()
  {
    Assert.IsNotNull (_customerEndPoint as INullableObject);
    Assert.IsFalse (_customerEndPoint.IsNull);
  }

  [Test]
  public void RelationDefinitionNull ()
  {
    VirtualRelationEndPointDefinition definition = new VirtualRelationEndPointDefinition (
        TestMappingConfiguration.Current.ClassDefinitions["Order"], "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

    Assert.IsNull (definition.RelationDefinition);
  }

  [Test]
  public void RelationDefinitionNotNull ()
  {
    Assert.IsNotNull (_customerEndPoint.RelationDefinition);
  }
}
}
