using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionTest : ReflectionBasedMappingTest
  {
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      RelationDefinition customerToOrder = TestMappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"];

      _customerEndPoint = (VirtualRelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Customer", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

      _orderEndPoint = (RelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
    }

    [Test]
    public void InitializeWithResolvedPropertyType ()
    {
      RelationEndPointDefinition endPoint = new RelationEndPointDefinition (
          ClassDefinitionFactory.CreateOrderDefinitionWithResolvedCustomerProperty (), 
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", 
          true);

      Assert.IsTrue (endPoint.IsPropertyTypeResolved);
      Assert.AreSame (typeof (ObjectID), endPoint.PropertyType);
      Assert.AreEqual (typeof (ObjectID).AssemblyQualifiedName, endPoint.PropertyTypeName);
    }

    [Test]
    public void IsNull ()
    {
      Assert.IsNotNull (_orderEndPoint as INullableObject);
      Assert.IsFalse (_orderEndPoint.IsNull);
    }

    [Test]
    public void CorrespondsToForVirtualEndPoint ()
    {
      Assert.IsTrue (_customerEndPoint.CorrespondsTo ("Customer", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
      Assert.IsFalse (_customerEndPoint.CorrespondsTo ("Customer", "NonExistingProperty"));
      Assert.IsFalse (_customerEndPoint.CorrespondsTo ("OrderTicket", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
    }

    [Test]
    public void CorrespondsTo ()
    {
      Assert.IsTrue (_orderEndPoint.CorrespondsTo ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Order", "NonExistingProperty"));
      Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Partner", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      RelationEndPointDefinition definition = new RelationEndPointDefinition (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)], 
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", 
          true);

      Assert.IsNull (definition.RelationDefinition);
    }

    [Test]
    public void RelationDefinitionNotNull ()
    {
      Assert.IsNotNull (_orderEndPoint.RelationDefinition);
    }
  }
}
