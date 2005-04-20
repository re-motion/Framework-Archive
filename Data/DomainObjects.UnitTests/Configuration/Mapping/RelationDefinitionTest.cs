using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class RelationDefinitionTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _orderClass;
  private ClassDefinition _customerClass;
  private VirtualRelationEndPointDefinition _customerEndPoint;
  private RelationEndPointDefinition _orderEndPoint;
  private RelationDefinition _customerToOrder;

  // construction and disposing

  public RelationDefinitionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void Setup ()
  {
    _customerClass = TestMappingConfiguration.Current.ClassDefinitions["Customer"];
    _orderClass = TestMappingConfiguration.Current.ClassDefinitions["Order"];
    _customerToOrder = TestMappingConfiguration.Current.RelationDefinitions["CustomerToOrder"];
    _customerEndPoint = (VirtualRelationEndPointDefinition) _customerToOrder.EndPointDefinitions[0];
    _orderEndPoint = (RelationEndPointDefinition) _customerToOrder.EndPointDefinitions[1];
  }

  [Test]
  public void IsEndPoint ()
  {
    RelationDefinition relation = new RelationDefinition ("myRelation", _customerEndPoint, _orderEndPoint);

    Assert.IsTrue (relation.IsEndPoint ("Order", "Customer"));
    Assert.IsTrue (relation.IsEndPoint ("Customer", "Orders"));
    Assert.IsFalse (relation.IsEndPoint ("Order", "Orders"));
    Assert.IsFalse (relation.IsEndPoint ("Customer", "Customer"));
  }

  [Test]
  public void GetEndPointDefinition ()
  {
    Assert.AreSame (_orderEndPoint, _customerToOrder.GetEndPointDefinition ("Order", "Customer"));
    Assert.AreSame (_customerEndPoint, _customerToOrder.GetEndPointDefinition ("Customer", "Orders"));
  }

  [Test]
  public void GetOppositeEndPointDefinition ()
  {
    Assert.AreSame (_customerEndPoint, _customerToOrder.GetOppositeEndPointDefinition ("Order", "Customer"));
    Assert.AreSame (_orderEndPoint, _customerToOrder.GetOppositeEndPointDefinition ("Customer", "Orders"));
  }

  [Test]
  public void GetEndPointDefinitionForUndefinedProperty ()
  {
    Assert.IsNull (_customerToOrder.GetEndPointDefinition ("Order", "OrderNumber"));
  }

  [Test]
  public void GetOppositeEndPointDefinitionForUndefinedProperty ()
  {
    Assert.IsNull (_customerToOrder.GetOppositeEndPointDefinition ("Order", "OrderNumber"));
  }

  [Test]
  public void GetEndPointDefinitionForUndefinedClass ()
  {
    Assert.IsNull (_customerToOrder.GetEndPointDefinition ("OrderTicket", "Customer"));
  }

  [Test]
  public void GetOppositeEndPointDefinitionForUndefinedClass ()
  {
    Assert.IsNull (_customerToOrder.GetOppositeEndPointDefinition ("OrderTicket", "Customer"));
  }

  [Test]
  public void GetOppositeClassDefinition ()
  {
    Assert.AreSame (_customerClass, _customerToOrder.GetOppositeClassDefinition ("Order", "Customer"));
    Assert.AreSame (_orderClass, _customerToOrder.GetOppositeClassDefinition ("Customer", "Orders"));
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Relation 'OrderToOrderItem' has no association with class 'Customer' and property 'Orders'.")]
  public void GetMandatoryOppositeRelationEndPointDefinitionWithNotAssociatedRelationDefinitionID ()
  {
    RelationDefinition orderToOrderItem = TestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"]; 
    
    IRelationEndPointDefinition wrongEndPointDefinition = 
        orderToOrderItem.GetMandatoryOppositeRelationEndPointDefinition (
            _customerClass.GetMandatoryRelationEndPointDefinition ("Orders"));

    orderToOrderItem.GetMandatoryOppositeRelationEndPointDefinition (wrongEndPointDefinition);
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Relation 'InvalidRelation' cannot have two null end points.")]
  public void InitializeWithTwoNullRelationEndPointDefinitions ()
  {
    NullRelationEndPointDefinition nullEndPointDefinition = new NullRelationEndPointDefinition (_customerClass);
    RelationDefinition definition = new RelationDefinition ("InvalidRelation", nullEndPointDefinition, nullEndPointDefinition);
  }
}
}
