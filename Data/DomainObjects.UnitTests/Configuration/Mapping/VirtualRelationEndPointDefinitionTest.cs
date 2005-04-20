using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

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
  public void IsNull ()
  {
    Assert.IsNotNull (_customerEndPoint as INullableObject);
    Assert.IsFalse (_customerEndPoint.IsNull);
  }
}
}
