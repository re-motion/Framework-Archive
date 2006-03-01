using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class RelationEndPointDefinitionTest
{
  // types

  // static members and constants

  // member fields

  private VirtualRelationEndPointDefinition _customerEndPoint;
  private RelationEndPointDefinition _orderEndPoint;

  // construction and disposing

  public RelationEndPointDefinitionTest ()
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
  public void InitializeWithUnresolvedPropertyType ()
  {
    RelationEndPointDefinition endPoint = new RelationEndPointDefinition (
        ClassDefinitionFactory.CreateWithUnresolvedRelationProperty (), "PropertyName", true);

    Assert.IsFalse (endPoint.IsPropertyTypeResolved);
    Assert.IsNull (endPoint.PropertyType);
    Assert.AreEqual (typeof(ObjectID).AssemblyQualifiedName, endPoint.PropertyTypeName);
  }

  [Test]
  public void InitializeWithResolvedPropertyType ()
  {
    RelationEndPointDefinition endPoint = new RelationEndPointDefinition (
        ClassDefinitionFactory.CreateOrderDefinitionWithResolvedCustomerProperty (), "Customer", true);

    Assert.IsTrue (endPoint.IsPropertyTypeResolved);
    Assert.AreSame (typeof (ObjectID), endPoint.PropertyType);
    Assert.AreEqual (typeof(ObjectID).AssemblyQualifiedName, endPoint.PropertyTypeName);
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
    Assert.IsTrue (_customerEndPoint.CorrespondsTo ("Customer", "Orders"));
    Assert.IsFalse (_customerEndPoint.CorrespondsTo ("Customer", "NonExistingProperty"));
    Assert.IsFalse (_customerEndPoint.CorrespondsTo ("OrderTicket", "Orders"));
  }

  [Test]
  public void CorrespondsTo ()
  {
    Assert.IsTrue (_orderEndPoint.CorrespondsTo ("Order", "Customer"));
    Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Order", "NonExistingProperty"));
    Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Partner", "Customer"));
  }

  [Test]
  public void RelationDefinitionNull ()
  {
    RelationEndPointDefinition definition = new RelationEndPointDefinition (
        TestMappingConfiguration.Current.ClassDefinitions["OrderTicket"], "Order", true);

    Assert.IsNull (definition.RelationDefinition);
  }

  [Test]
  public void RelationDefinitionNotNull ()
  {
    Assert.IsNotNull (_orderEndPoint.RelationDefinition);
  }
}
}
