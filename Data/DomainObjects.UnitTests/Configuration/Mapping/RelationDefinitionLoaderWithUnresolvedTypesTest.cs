using System;
using System.Xml;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationDefinitionLoaderWithUnresolvedTypesTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private RelationDefinitionLoader _loader;
    private ClassDefinitionCollection _classDefintions;

    // construction and disposing

    public RelationDefinitionLoaderWithUnresolvedTypesTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      XmlDocument mappingDocument = new XmlDocument ();
      mappingDocument.Load ("MappingWithUnresolvedTypes.xml");

      PrefixNamespace[] namespaces = new PrefixNamespace[] { LegacyPrefixNamespace.MappingNamespace };
      ConfigurationNamespaceManager namespaceManager = new ConfigurationNamespaceManager (mappingDocument, namespaces);

      ClassDefinitionLoader classLoader = new ClassDefinitionLoader (mappingDocument, namespaceManager, false);
      _classDefintions = classLoader.GetClassDefinitions ();
      Assert.IsFalse (_classDefintions.AreResolvedTypesRequired);

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
      Assert.IsNull (orderToOrderTicketEndPoint.PropertyType);
      Assert.AreEqual ("UnresolvedOrderTicketType, UnknownAssembly", orderToOrderTicketEndPoint.PropertyTypeName);
      Assert.AreEqual (_classDefintions["Order"].IsClassTypeResolved, orderToOrderTicketEndPoint.IsPropertyTypeResolved);

      IRelationEndPointDefinition orderTicketToOrderEndPoint = orderToOrderTicket.GetEndPointDefinition ("OrderTicket", "Order");
      Assert.IsNotNull (orderTicketToOrderEndPoint);
      Assert.IsNull (orderTicketToOrderEndPoint.PropertyType);
      Assert.AreEqual (TypeUtility.GetPartialAssemblyQualifiedName (typeof (ObjectID)), orderTicketToOrderEndPoint.PropertyTypeName);
      Assert.AreEqual (_classDefintions["OrderTicket"].IsClassTypeResolved, orderTicketToOrderEndPoint.IsPropertyTypeResolved);

      RelationDefinition orderToOrderItem = relations.GetMandatory ("OrderToOrderItem");

      IRelationEndPointDefinition orderToOrderItemEndPoint = orderToOrderItem.GetEndPointDefinition ("Order", "OrderItems");
      Assert.IsNotNull (orderToOrderItemEndPoint);
      Assert.IsNull (orderToOrderItemEndPoint.PropertyType);
      Assert.AreEqual (TypeUtility.GetPartialAssemblyQualifiedName (typeof (DomainObjectCollection)), orderToOrderItemEndPoint.PropertyTypeName);
      Assert.AreEqual (_classDefintions["Order"].IsClassTypeResolved, orderToOrderItemEndPoint.IsPropertyTypeResolved);

      IRelationEndPointDefinition orderItemToOrderEndPoint = orderToOrderItem.GetEndPointDefinition ("OrderItem", "Order");
      Assert.IsNotNull (orderItemToOrderEndPoint);
      Assert.IsNull (orderItemToOrderEndPoint.PropertyType);
      Assert.AreEqual (TypeUtility.GetPartialAssemblyQualifiedName (typeof (ObjectID)), orderItemToOrderEndPoint.PropertyTypeName);
      Assert.AreEqual (_classDefintions["OrderItem"].IsClassTypeResolved, orderItemToOrderEndPoint.IsPropertyTypeResolved);

      RelationDefinition customerToOrder = relations.GetMandatory ("CustomerToOrder");

      IRelationEndPointDefinition customerToOrderEndPoint = customerToOrder.GetEndPointDefinition ("Customer", "Orders");
      Assert.IsNotNull (customerToOrderEndPoint);
      Assert.IsNull (customerToOrderEndPoint.PropertyType);
      Assert.AreEqual ("UnresolvedCollectionType, UnknownAssembly", customerToOrderEndPoint.PropertyTypeName);
      Assert.AreEqual (_classDefintions["Customer"].IsClassTypeResolved, customerToOrderEndPoint.IsPropertyTypeResolved);

      IRelationEndPointDefinition orderToCustomerEndPoint = customerToOrder.GetEndPointDefinition ("Order", "Customer");
      Assert.IsNotNull (orderToCustomerEndPoint);
      Assert.IsNull (orderToCustomerEndPoint.PropertyType);
      Assert.AreEqual (TypeUtility.GetPartialAssemblyQualifiedName (typeof (ObjectID)), orderToCustomerEndPoint.PropertyTypeName);
      Assert.AreEqual (_classDefintions["Order"].IsClassTypeResolved, orderToCustomerEndPoint.IsPropertyTypeResolved);
    }
  }
}
