using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class PersistenceManagerTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  PersistenceManager _persistenceManager;

  // construction and disposing

  public PersistenceManagerTest ()
  {
  }

  // methods and properties

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp ();
    _persistenceManager = new PersistenceManager ();
  }

  public override void TearDown ()
  {
    base.TearDown ();
    _persistenceManager.Dispose ();
  }

  [Test]
  public void LoadDataContainer ()
  {
    DataContainer actualDataContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
    Assert.AreEqual (DomainObjectIDs.Order1, actualDataContainer.ID);
  }

  [Test]
  [ExpectedException (typeof (PersistenceException),
      "Storage Provider with ID 'NonExistingProviderID' could not be created.")]
  public void LoadDataContainerWithNonExistingProviderID ()
  {
    ObjectID id = new ObjectID ("NonExistingProviderID", "ClassWithAllDataTypes",
        new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    _persistenceManager.LoadDataContainer (id);
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), 
      "Mapping does not contain a class definition with ID 'ThisClassIDDoesNotExist'.")]
  public void LoadDataContainerWithNotConfiguredClassID ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, "ThisClassIDDoesNotExist",
        new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    _persistenceManager.LoadDataContainer (id);
  }

  [Test]
  public void LoadRelatedDataContainer ()
  {
    DataContainer orderTicketContainer = TestDataContainerFactory.CreateOrderTicket1DataContainer ();

    DataContainer orderContainer = _persistenceManager.LoadRelatedDataContainer (
        orderTicketContainer, new RelationEndPointID (orderTicketContainer.ID, "Order"));

    DataContainerChecker checker = new DataContainerChecker ();
    checker.Check (TestDataContainerFactory.CreateOrder1DataContainer (), orderContainer);
  }

  [Test]
  public void LoadDataContainerOverVirtualEndPoint ()
  {
    DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();

    DataContainer orderTicketContainer = _persistenceManager.LoadRelatedDataContainer (
        orderContainer, new RelationEndPointID (orderContainer.ID, "OrderTicket"));

    DataContainerChecker checker = new DataContainerChecker ();
    checker.Check (TestDataContainerFactory.CreateOrderTicket1DataContainer (), orderTicketContainer);
  }

  [Test]
  public void LoadRelatedDataContainerByOptionalNullID ()
  {
    ObjectID id = new ObjectID (
        c_testDomainProviderID, "ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

    DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
        dataContainer, new RelationEndPointID (dataContainer.ID, "ClassWithGuidKeyOptional"));

    Assert.IsNull (relatedDataContainer);
  }

  [Test]
  public void LoadRelatedDataContainerByOptionalNullIDVirtual ()
  {
    ObjectID id = new ObjectID (
        c_testDomainProviderID, "ClassWithGuidKey",
        new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

    DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
        dataContainer, new RelationEndPointID (dataContainer.ID, "ClassWithValidRelationsOptional"));

    Assert.IsNull (relatedDataContainer);
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "Cannot load related DataContainer of object"
      + " 'TestDomain|ClassWithValidRelations|3e5aed0e-c6f9-4dca-a901-4da50f5a97ab|System.Guid'"
      + " over mandatory relation 'ClassWithGuidKeyToClassWithValidRelationsNonOptional'.")] 
  public void LoadRelatedDataContainerByNonOptionalNullID ()
  {
    ObjectID id = new ObjectID (
        c_testDomainProviderID, "ClassWithValidRelations", new Guid ("{3E5AED0E-C6F9-4dca-A901-4DA50F5A97AB}"));

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

    _persistenceManager.LoadRelatedDataContainer (
        dataContainer, new RelationEndPointID (dataContainer.ID, "ClassWithGuidKeyNonOptional"));
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "Cannot load related DataContainer of object"
      + " 'TestDomain|Distributor|1514d668-a0a5-40e9-ac22-f24900e0eb39|System.Guid'"
      + " over mandatory relation 'PartnerToPerson'.")] 
  public void LoadRelatedDataContainerByNonOptionalNullIDWithInheritance ()
  {
    DataContainer dataContainer = _persistenceManager.LoadDataContainer (
        DomainObjectIDs.DistributorWithoutContactPersonAndCeo);
    
    _persistenceManager.LoadRelatedDataContainer (
        dataContainer, new RelationEndPointID (dataContainer.ID, "ContactPerson"));
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "Cannot load related DataContainer of object"
      + " 'TestDomain|ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid'"
      + " over mandatory relation 'ClassWithGuidKeyToClassWithValidRelationsNonOptional'.")] 
  public void LoadRelatedDataContainerByNonOptionalNullIDVirtual ()
  {
    ObjectID id = new ObjectID (
        c_testDomainProviderID, "ClassWithGuidKey",
        new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

    _persistenceManager.LoadRelatedDataContainer (
        dataContainer, new RelationEndPointID (dataContainer.ID, "ClassWithValidRelationsNonOptional"));
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "Cannot load related DataContainer of object"
      + " 'TestDomain|Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid'"
      + " over mandatory relation 'CompanyToCeo'.")] 
  public void LoadRelatedDataContainerByNonOptionalNullIDVirtualWithInheritance ()
  {
    DataContainer dataContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.PartnerWithoutCeo);
    
    _persistenceManager.LoadRelatedDataContainer (
        dataContainer, new RelationEndPointID (dataContainer.ID, "Ceo"));
  }

  [Test]
  public void LoadRelatedDataContainerOverValidMandatoryRelation ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, "ClassWithGuidKey", 
        new Guid ("{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}"));

    DataContainer classWithGuidKey = _persistenceManager.LoadDataContainer (id);

    DataContainer relatedContainer = _persistenceManager.LoadRelatedDataContainer (
        classWithGuidKey, new RelationEndPointID (classWithGuidKey.ID, "ClassWithValidRelationsNonOptional"));

    ObjectID expectedID = new ObjectID (c_testDomainProviderID, "ClassWithValidRelations", 
        new Guid ("{35BA182C-C836-490e-AF79-74C72145BCE5}"));

    Assert.IsNotNull (relatedContainer);
    Assert.AreEqual (expectedID, relatedContainer.ID);
  }

  [Test]
  [ExpectedException (typeof (PersistenceException),
      "Cannot load related DataContainer of object"
      + " 'TestDomain|ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid'"
      + " over mandatory relation 'ClassWithGuidKeyToClassWithValidRelationsNonOptional'.")]
  public void LoadRelatedDataContainerOverInvalidNonOptionalRelation ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, "ClassWithGuidKey", 
        new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

    DataContainer classWithGuidKey = _persistenceManager.LoadDataContainer (id);

    DataContainer relatedContainer = _persistenceManager.LoadRelatedDataContainer (
        classWithGuidKey, new RelationEndPointID (classWithGuidKey.ID, "ClassWithValidRelationsNonOptional"));
  }

  [Test]
  [ExpectedException (typeof (PersistenceException),
      "Property 'ClassWithGuidKey' of class 'ClassWithInvalidRelation' refers"
      + " to non-existing object with ID 'a53f679d-0e91-4504-aee8-59250de249b3'.")]
  public void LoadRelatedDataContainerByInvalidID ()
  {
    ObjectID id = new ObjectID (
        c_testDomainProviderID, "ClassWithInvalidRelation",
        new Guid ("{35BA182C-C836-490e-AF79-74C72145BCE5}"));

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);

    _persistenceManager.LoadRelatedDataContainer (
        dataContainer, new RelationEndPointID (dataContainer.ID, "ClassWithGuidKey"));
  }

  [Test]
  public void LoadRelatedDataContainerFromDifferentProvider ()
  {
    DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
 
    DataContainer officialContainer = _persistenceManager.LoadRelatedDataContainer (
        orderContainer, new RelationEndPointID (orderContainer.ID, "Official"));

    Assert.IsNotNull (officialContainer);
    Assert.AreEqual ("UnitTestStorageProviderStub", officialContainer.ID.StorageProviderID, "StorageProviderID");
    Assert.AreEqual ("Official", officialContainer.ID.ClassID, "ClassID");
    Assert.AreEqual (1, officialContainer.ID.Value, "Value");    
  }

  [Test]
  public void LoadRelatedDataContainers ()
  {
    DataContainerCollection collection = _persistenceManager.LoadRelatedDataContainers (
        new RelationEndPointID (DomainObjectIDs.Customer1, "Orders"));

    Assert.IsNotNull (collection);
    Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
    Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
    Assert.IsNotNull (collection[DomainObjectIDs.OrderWithoutOrderItem], "ID of Order with OrdnerNo 2");
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point,"
      + " relation: 'CustomerToOrder', property: 'Customer'. Check your mapping configuration.")]
  public void LoadRelatedDataContainersForNonVirtualEndPoint ()
  {
    _persistenceManager.LoadRelatedDataContainers (new RelationEndPointID (DomainObjectIDs.Order1, "Customer"));
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "Collection for mandatory relation 'OrderToOrderItem' (property: 'OrderItems') contains no elements.")]
  public void LoadEmptyRelatedDataContainersForMandatoryRelation ()
  {
    _persistenceManager.LoadRelatedDataContainers (
        new RelationEndPointID (DomainObjectIDs.OrderWithoutOrderItem, "OrderItems"));
  }

  [Test]
  public void LoadEmptyRelatedDataContainersForMandatoryRelationWithOptionalOppositeEndPoint ()  
  {
    DataContainerCollection orderContainers = _persistenceManager.LoadRelatedDataContainers (
        new RelationEndPointID (DomainObjectIDs.Customer2, "Orders"));

    Assert.AreEqual (0, orderContainers.Count);
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "Cannot load a single related data container for one-to-many relation 'OrderToOrderItem'.")]
  public void LoadRelatedDataContainerForOneToManyRelation ()
  {
    DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
    _persistenceManager.LoadRelatedDataContainer (
        orderContainer, new RelationEndPointID (orderContainer.ID, "OrderItems"));
  }

  [Test]
  [ExpectedException (typeof (PersistenceException), 
      "Cannot load multiple related data containers for one-to-one relation 'OrderToOrderTicket'.")]
  public void LoadRelatedDataContainersForOneToOneRelation ()
  {
    _persistenceManager.LoadRelatedDataContainers (new RelationEndPointID (DomainObjectIDs.Order1, "OrderTicket"));
  }

  [Test]
  [ExpectedException (typeof (PersistenceException),
      "Save does not support multiple storage providers.")]
  public void SaveInDifferentStorageProviders ()
  {
    DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
    DataContainer officialContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Official1);

    DataContainerCollection dataContainers = new DataContainerCollection ();
    dataContainers.Add (orderContainer);
    dataContainers.Add (officialContainer);

    orderContainer["OrderNumber"] = 42;
    officialContainer["Name"] = "Zaphod";

    _persistenceManager.Save (dataContainers);
  }

  [Test]
  public void CreateNewDataContainer ()
  {
    ClassDefinition orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
    DataContainer container = _persistenceManager.CreateNewDataContainer (orderClass);

    Assert.IsNotNull (container);
    Assert.AreEqual (StateType.New, container.State);
    Assert.IsNotNull (container.ID);
  }

  [Test]
  [ExpectedException (typeof (ObjectNotFoundException), 
      "Object 'TestDomain|Order|c3f486ae-ba6a-4bac-a084-0ccbf445523e|System.Guid' could not be found.")]
  public void LoadDataContainerWithNonExistingValue ()
  {
    Guid nonExistingID = new Guid ("{C3F486AE-BA6A-4bac-A084-0CCBF445523E}");
    ObjectID id = new ObjectID (c_testDomainProviderID, "Order", nonExistingID);
 
    _persistenceManager.LoadDataContainer (id);
  }
}
}