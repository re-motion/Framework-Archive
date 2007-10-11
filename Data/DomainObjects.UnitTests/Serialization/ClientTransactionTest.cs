using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class ClientTransactionTest : SerializationBaseTest
  {
    [Test]
    public void ObjectIDTest ()
    {
      ObjectID objectID = new ObjectID ("Company", Guid.NewGuid ());

      ObjectID deserializedObjectID = (ObjectID) SerializeAndDeserialize (objectID);

      Assert.AreEqual (objectID, deserializedObjectID);
      Assert.AreEqual (objectID.Value.GetType (), deserializedObjectID.Value.GetType ());
      Assert.AreSame (objectID.ClassDefinition, deserializedObjectID.ClassDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Company"), deserializedObjectID.ClassDefinition);
    }

    [Test]
    public void PropertyValueTest ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions["Customer"];
      PropertyDefinition propertyDefinition = classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince"];
      PropertyValue value = new PropertyValue (propertyDefinition);

      PropertyValue deserializedValue = (PropertyValue) SerializeAndDeserialize (value);

      Assert.AreEqual (value.Name, deserializedValue.Name);
      Assert.AreEqual (value.OriginalValue, deserializedValue.OriginalValue);
      Assert.AreEqual (value.Value, deserializedValue.Value);
      Assert.AreEqual (value.Definition, deserializedValue.Definition);
      Assert.AreEqual (propertyDefinition, deserializedValue.Definition);
    }

    [Test]
    public void PropertyValueCollection ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions["Customer"];
      PropertyDefinition propertyDefinition = classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince"];
      PropertyValue value = new PropertyValue (propertyDefinition);

      PropertyValueCollection collection = new PropertyValueCollection ();

      collection.Add (value);

      PropertyValueCollection deserializedCollection = (PropertyValueCollection) SerializeAndDeserialize (collection);

      Assert.IsNotNull (deserializedCollection);
      Assert.AreEqual (collection.Count, deserializedCollection.Count);
    }

    [Test]
    public void DataContainerTest ()
    {
      ObjectID objectID = new ObjectID ("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);

      DataContainer deserializedDataContainer = (DataContainer) SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
    }

    [Test]
    public void DomainObjectTest ()
    {
      DomainObject domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObject deserializedDomainObject = (DomainObject) SerializeAndDeserialize (domainObject);

      Assert.AreEqual (domainObject.ID, deserializedDomainObject.ID);
    }

    [Test]
    public void DomainObject_IDeserializationCallbackTest ()
    {
      Customer domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      Customer deserializedDomainObject = Serializer.SerializeAndDeserialize (domainObject);
      Assert.IsTrue (deserializedDomainObject.OnDeserializationCalled);
    }

    [Test]
    [Ignore ("TODO: Fix this bug")]
    public void DomainObject_DeserializationCallbackAttributesTest ()
    {
      Customer domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      Customer deserializedDomainObject = Serializer.SerializeAndDeserialize (domainObject);
      Assert.IsTrue (deserializedDomainObject.OnDeserializingAttributeCalled);
      Assert.IsTrue (deserializedDomainObject.OnDeserializedAttributeCalled);
    }

    [Test]
    public void DomainObjectCollectionTest ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      collection.Add (customer);

      DomainObjectCollection deserializedCollection = (DomainObjectCollection) SerializeAndDeserialize (collection);

      Assert.AreEqual (collection.Count, deserializedCollection.Count);
    }

    [Test]
    public void RelationEndPointIDTest ()
    {
      RelationEndPointID endPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

      RelationEndPointID deserializedEndPointID = (RelationEndPointID) SerializeAndDeserialize (endPointID);

      Assert.AreEqual (endPointID.ObjectID, deserializedEndPointID.ObjectID);
      Assert.AreEqual (endPointID.PropertyName, deserializedEndPointID.PropertyName);
    }

    [Test]
    public void ObjectEndPointTest ()
    {
      ObjectEndPoint endPoint = CreateObjectEndPoint (Company.GetObject (DomainObjectIDs.Company1), "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", DomainObjectIDs.IndustrialSector1);

      ObjectEndPoint deserializedEndPoint = (ObjectEndPoint) SerializeAndDeserialize (endPoint);

      Assert.AreEqual (endPoint.ID, deserializedEndPoint.ID);
    }

    [Test]
    public void CollectionEndPointTest ()
    {
      DomainObjectCollection oppositeDomainObjects = new DomainObjectCollection ();
      CollectionEndPoint endPoint = new CollectionEndPoint (ClientTransaction.Current, new RelationEndPointID (DomainObjectIDs.Customer1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"), oppositeDomainObjects);

      CollectionEndPoint deserializedEndPoint = (CollectionEndPoint) SerializeAndDeserialize (endPoint);

      Assert.AreEqual (endPoint.ID, deserializedEndPoint.ID);
    }

    [Test]
    public void RelationEndPointCollectionTest ()
    {
      RelationEndPointCollection collection = new RelationEndPointCollection ();
      ObjectEndPoint endPoint = CreateObjectEndPoint (Company.GetObject (DomainObjectIDs.Company1), "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", DomainObjectIDs.IndustrialSector1);
      collection.Add (endPoint);

      RelationEndPointCollection deserializedCollection = (RelationEndPointCollection) SerializeAndDeserialize (collection);

      Assert.AreEqual (collection.Count, deserializedCollection.Count);
    }

    private ObjectEndPoint CreateObjectEndPoint (DomainObject domainObject, string propertyName, ObjectID oppositeObjectID)
    {
      return new ObjectEndPoint (ClientTransaction.Current, domainObject.ID, propertyName, oppositeObjectID);
    }

    [Test]
    public void RelationEndPointMapTest ()
    {
      RelationEndPointMap relationEndPointMap = new RelationEndPointMap (ClientTransactionScope.CurrentTransaction);

      RelationEndPointMap deserializedRelationEndPointMap = (RelationEndPointMap) SerializeAndDeserialize (relationEndPointMap);

      Assert.AreEqual (relationEndPointMap.Count, deserializedRelationEndPointMap.Count);
    }

    [Test]
    public void DataContainerCollectionTest ()
    {
      DataContainerCollection collection = new DataContainerCollection ();
      DataContainer dataContainer = DataContainer.CreateNew (DomainObjectIDs.Customer1);
      collection.Add (dataContainer);

      DataContainerCollection deserializedCollection = (DataContainerCollection) SerializeAndDeserialize (collection);

      Assert.AreEqual (collection.Count, deserializedCollection.Count);
    }

    [Test]
    public void DataContainerMapTest ()
    {
      DataContainerMap dataContainerMap = new DataContainerMap (ClientTransactionScope.CurrentTransaction);
      DataContainer dataContainer = DataContainer.CreateNew (DomainObjectIDs.Customer1);
      dataContainerMap.Register (dataContainer);

      DataContainerMap deserializedDataContainerMap = (DataContainerMap) SerializeAndDeserialize (dataContainerMap);

      Assert.AreEqual (dataContainerMap.Count, deserializedDataContainerMap.Count);
    }

    [Test]
    public void DataManagerTest ()
    {
      DataManager dataManager = new DataManager (ClientTransactionScope.CurrentTransaction);

      DataManager deserializedDataManager = (DataManager) SerializeAndDeserialize (dataManager);

      Assert.IsNotNull (deserializedDataManager);
    }

    [Test]
    public void QueryManagerTest ()
    {
      RootQueryManager queryManager = new RootQueryManager ((RootClientTransaction) ClientTransactionScope.CurrentTransaction);

      RootQueryManager deserializedQueryManager = (RootQueryManager) SerializeAndDeserialize (queryManager);

      Assert.IsNotNull (deserializedQueryManager);
    }

    [Test]
    public void ClientTransactionSerializationTest ()
    {
      ClientTransaction clientTransaction = ClientTransaction.NewTransaction();

      ClientTransaction deserializedClientTransaction = (ClientTransaction) SerializeAndDeserialize (clientTransaction);

      Assert.IsNotNull (deserializedClientTransaction);
    }

    [Test]
    public void SubClientTransactionSerializationTest ()
    {
      ClientTransaction clientTransaction = ClientTransaction.NewTransaction ().CreateSubTransaction();

      ClientTransaction deserializedClientTransaction = (ClientTransaction) SerializeAndDeserialize (clientTransaction);

      Assert.IsNotNull (deserializedClientTransaction);
      Assert.IsNotNull (deserializedClientTransaction.ParentTransaction);
    }
  }
}
