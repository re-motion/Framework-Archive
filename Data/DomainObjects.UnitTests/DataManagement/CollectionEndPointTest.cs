using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Relations;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Relations
{
[TestFixture]
public class CollectionEndPointTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPointDefinition _orderItemEndPointDefinition;
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

    Order order = Order.GetObject (DomainObjectIDs.Order1);
    ClassDefinition orderItemClass = TestMappingConfiguration.Current.ClassDefinitions.GetByClassID ("OrderItem");
    
    _orderItemEndPointDefinition = 
        (RelationEndPointDefinition) orderItemClass.GetRelationEndPointDefinition ("Order");
    
    _orderItems = order.OrderItems;
    _orderItemEndPoint = new CollectionEndPoint (_orderItems, _orderItemEndPointDefinition);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreSame (_orderItemEndPointDefinition, _orderItemEndPoint.Definition);
    Assert.AreSame (_orderItems, _orderItemEndPoint.DomainObjects);
  }
}
}
