using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class CollectionEndPointTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private VirtualRelationEndPointDefinition _orderEndPointDefinition;
  private Order _order;
  private DomainObjectCollection _orderItems;
  private CollectionEndPoint _orderItemEndPoint;
  
  // construction and disposing

  public CollectionEndPointTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _order = Order.GetObject (DomainObjectIDs.Order1);
    ClassDefinition orderItemClass = MappingConfiguration.Current.ClassDefinitions.GetByClassID ("OrderItem");
    
    _orderEndPointDefinition = 
        (VirtualRelationEndPointDefinition) orderItemClass.GetOppositeEndPointDefinition ("Order");
    
    _orderItems = _order.OrderItems;
    _orderItemEndPoint = new CollectionEndPoint (_order, _orderEndPointDefinition, _orderItems);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreSame (_order, _orderItemEndPoint.DomainObject);
    Assert.AreSame (_orderEndPointDefinition, _orderItemEndPoint.Definition);
    Assert.AreSame (_orderItems, _orderItemEndPoint.OppositeDomainObjects);
  }
}
}
