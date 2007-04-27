using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationDefinitionTest : ReflectionBasedMappingTest
  {
    private ClassDefinition _orderClass;
    private ClassDefinition _customerClass;
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;
    private RelationDefinition _customerToOrder;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Customer)];
      _orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _customerToOrder = TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"];
      _customerEndPoint = (VirtualRelationEndPointDefinition) _customerToOrder.EndPointDefinitions[0];
      _orderEndPoint = (RelationEndPointDefinition) _customerToOrder.EndPointDefinitions[1];
    }

    [Test]
    public void GetToString ()
    {
      RelationDefinition relation = new RelationDefinition ("RelationID", _customerEndPoint, _orderEndPoint);

      Assert.That (relation.ToString (), Is.EqualTo (typeof (RelationDefinition).FullName + ": RelationID"));
    }

    [Test]
    public void IsEndPoint ()
    {
      RelationDefinition relation = new RelationDefinition ("myRelation", _customerEndPoint, _orderEndPoint);

      Assert.IsTrue (relation.IsEndPoint ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.IsTrue (relation.IsEndPoint ("Customer", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
      Assert.IsFalse (relation.IsEndPoint ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
      Assert.IsFalse (relation.IsEndPoint ("Customer", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
    }

    [Test]
    public void GetEndPointDefinition ()
    {
      Assert.AreSame (_orderEndPoint, _customerToOrder.GetEndPointDefinition ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.AreSame (_customerEndPoint, _customerToOrder.GetEndPointDefinition ("Customer", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      Assert.AreSame (_customerEndPoint, _customerToOrder.GetOppositeEndPointDefinition ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.AreSame (_orderEndPoint, _customerToOrder.GetOppositeEndPointDefinition ("Customer", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
    }

    [Test]
    public void GetEndPointDefinitionForUndefinedProperty ()
    {
      Assert.IsNull (_customerToOrder.GetEndPointDefinition ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
    }

    [Test]
    public void GetOppositeEndPointDefinitionForUndefinedProperty ()
    {
      Assert.IsNull (_customerToOrder.GetOppositeEndPointDefinition ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
    }

    [Test]
    public void GetEndPointDefinitionForUndefinedClass ()
    {
      Assert.IsNull (_customerToOrder.GetEndPointDefinition ("OrderTicket", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Customer"));
    }

    [Test]
    public void GetOppositeEndPointDefinitionForUndefinedClass ()
    {
      Assert.IsNull (_customerToOrder.GetOppositeEndPointDefinition ("OrderTicket", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Customer"));
    }

    [Test]
    public void GetOppositeClassDefinition ()
    {
      Assert.AreSame (_customerClass, _customerToOrder.GetOppositeClassDefinition ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.AreSame (_orderClass, _customerToOrder.GetOppositeClassDefinition ("Customer", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Relation 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' has no association with class 'Customer' "
        + "and property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders'.")]
    public void GetMandatoryOppositeRelationEndPointDefinitionWithNotAssociatedRelationDefinitionID ()
    {
      RelationDefinition orderToOrderItem = TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"];

      IRelationEndPointDefinition wrongEndPointDefinition =
          orderToOrderItem.GetMandatoryOppositeRelationEndPointDefinition (
              _customerClass.GetMandatoryRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      orderToOrderItem.GetMandatoryOppositeRelationEndPointDefinition (wrongEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Relation 'InvalidRelation' cannot have two null end points.")]
    public void InitializeWithTwoAnonymousRelationEndPointDefinitions ()
    {
      AnonymousRelationEndPointDefinition anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_customerClass);
      RelationDefinition definition = new RelationDefinition ("InvalidRelation", anonymousEndPointDefinition, anonymousEndPointDefinition);
    }

    [Test]
    public void Contains ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"];

      Assert.IsFalse (relationDefinition.Contains (TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].EndPointDefinitions[0]));
      Assert.IsFalse (relationDefinition.Contains (TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].EndPointDefinitions[1]));

      Assert.IsTrue (relationDefinition.Contains (MappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].EndPointDefinitions[0]));
      Assert.IsTrue (relationDefinition.Contains (MappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"].EndPointDefinitions[1]));
    }
  }
}
