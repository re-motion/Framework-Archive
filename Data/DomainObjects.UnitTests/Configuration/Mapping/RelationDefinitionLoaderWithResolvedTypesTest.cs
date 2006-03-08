using System;
using System.Xml;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{

[TestFixture]
public class RelationDefinitionLoaderWithResolvedTypeNamesTest
{
  // types

  // static members and constants

  // member fields

  private RelationDefinitionLoader _loader;
  private ClassDefinitionCollection _classDefintions;

  // construction and disposing

  public RelationDefinitionLoaderWithResolvedTypeNamesTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    XmlDocument mappingDocument = new XmlDocument ();
    mappingDocument.Load ("mappingWithResolvedTypes.xml");

    PrefixNamespace[] namespaces = new PrefixNamespace[] {PrefixNamespace.MappingNamespace};
    ConfigurationNamespaceManager namespaceManager = new ConfigurationNamespaceManager (mappingDocument, namespaces);
  
    ClassDefinitionLoader classLoader = new ClassDefinitionLoader (mappingDocument, namespaceManager, true);
    _classDefintions = classLoader.GetClassDefinitions ();
    Assert.IsTrue (_classDefintions.AreResolvedTypesRequired);

    _loader = new RelationDefinitionLoader (mappingDocument, namespaceManager, _classDefintions);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreEqual (_classDefintions.AreResolvedTypesRequired, _loader.ResolveTypes);
  }

  [Test]
  public void GetRelationDefinitions ()
  {
    RelationDefinitionCollection relations = _loader.GetRelationDefinitions ();
    Assert.AreEqual (3, relations.Count);

    RelationDefinition orderToOrderTicket = relations.GetMandatory ("OrderToOrderTicket");
    
    IRelationEndPointDefinition orderToOrderTicketEndPoint = orderToOrderTicket.GetEndPointDefinition ("Order", "OrderTicket");
    Assert.IsNotNull (orderToOrderTicketEndPoint);
    Assert.AreSame (typeof (OrderTicket), orderToOrderTicketEndPoint.PropertyType);
    Assert.AreEqual (typeof(OrderTicket).AssemblyQualifiedName, orderToOrderTicketEndPoint.PropertyTypeName);
    Assert.AreEqual (_classDefintions["Order"].IsClassTypeResolved, orderToOrderTicketEndPoint.IsPropertyTypeResolved);

    IRelationEndPointDefinition orderTicketToOrderEndPoint = orderToOrderTicket.GetEndPointDefinition ("OrderTicket", "Order");
    Assert.IsNotNull (orderTicketToOrderEndPoint);
    Assert.AreSame (typeof (ObjectID), orderTicketToOrderEndPoint.PropertyType);
    Assert.AreEqual (typeof(ObjectID).AssemblyQualifiedName, orderTicketToOrderEndPoint.PropertyTypeName);
    Assert.AreEqual (_classDefintions["OrderTicket"].IsClassTypeResolved, orderTicketToOrderEndPoint.IsPropertyTypeResolved);

    RelationDefinition orderToOrderItem = relations.GetMandatory ("OrderToOrderItem");

    IRelationEndPointDefinition orderToOrderItemEndPoint = orderToOrderItem.GetEndPointDefinition ("Order", "OrderItems");
    Assert.IsNotNull (orderToOrderItemEndPoint);
    Assert.AreSame (typeof (DomainObjectCollection), orderToOrderItemEndPoint.PropertyType);
    Assert.AreEqual (typeof(DomainObjectCollection).AssemblyQualifiedName, orderToOrderItemEndPoint.PropertyTypeName);
    Assert.AreEqual (_classDefintions["Order"].IsClassTypeResolved, orderToOrderItemEndPoint.IsPropertyTypeResolved);

    IRelationEndPointDefinition orderItemToOrderEndPoint = orderToOrderItem.GetEndPointDefinition ("OrderItem", "Order");
    Assert.IsNotNull (orderItemToOrderEndPoint);
    Assert.AreSame (typeof (ObjectID), orderItemToOrderEndPoint.PropertyType);
    Assert.AreEqual (typeof(ObjectID).AssemblyQualifiedName, orderItemToOrderEndPoint.PropertyTypeName);
    Assert.AreEqual (_classDefintions["OrderItem"].IsClassTypeResolved, orderItemToOrderEndPoint.IsPropertyTypeResolved);

    RelationDefinition customerToOrder = relations.GetMandatory ("CustomerToOrder");

    IRelationEndPointDefinition customerToOrderEndPoint = customerToOrder.GetEndPointDefinition ("Customer", "Orders");
    Assert.IsNotNull (customerToOrderEndPoint);
    Assert.AreSame (typeof (OrderCollection), customerToOrderEndPoint.PropertyType);
    Assert.AreEqual (typeof(OrderCollection).AssemblyQualifiedName, customerToOrderEndPoint.PropertyTypeName);
    Assert.AreEqual (_classDefintions["Customer"].IsClassTypeResolved, customerToOrderEndPoint.IsPropertyTypeResolved);

    IRelationEndPointDefinition orderToCustomerEndPoint = customerToOrder.GetEndPointDefinition ("Order", "Customer");
    Assert.IsNotNull (orderToCustomerEndPoint);
    Assert.AreSame (typeof (ObjectID), orderToCustomerEndPoint.PropertyType);
    Assert.AreEqual (typeof(ObjectID).AssemblyQualifiedName, orderToCustomerEndPoint.PropertyTypeName);
    Assert.AreEqual (_classDefintions["Order"].IsClassTypeResolved, orderToCustomerEndPoint.IsPropertyTypeResolved);
  }
}
}
