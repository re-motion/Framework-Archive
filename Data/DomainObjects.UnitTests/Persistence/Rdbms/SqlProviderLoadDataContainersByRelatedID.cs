using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderLoadDataContainersByRelatedID : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderLoadDataContainersByRelatedID ()
  {
  }

  // methods and properties

  [Test]
  public void Loading ()
  {
    DataContainerCollection collection = Provider.LoadDataContainersByRelatedID (
        TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)],
        "Customer", 
        DomainObjectIDs.Customer1);

    Assert.IsNotNull (collection);
    Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
    Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
    Assert.IsNotNull (collection[DomainObjectIDs.OrderWithoutOrderItem], "ID of Order with OrdnerNo 2");    
  }

  [Test]
  public void LoadOverInheritedProperty ()
  {
    DataContainer personContainer = Provider.LoadDataContainer (DomainObjectIDs.Person6);

    DataContainerCollection collection = Provider.LoadDataContainersByRelatedID (
        TestMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)],
        "ContactPerson",
        DomainObjectIDs.Person6);

    Assert.AreEqual (1, collection.Count);
    Assert.AreEqual (DomainObjectIDs.Distributor2, collection[0].ID);
  }

  [Test]
  public void LoadWithOrderBy ()
  {    
    ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions.GetByClassID ("Order");

    DataContainerCollection orderContainers = Provider.LoadDataContainersByRelatedID (
        orderDefinition, "Customer", DomainObjectIDs.Customer1);

    Assert.AreEqual (2, orderContainers.Count);
    Assert.AreEqual (DomainObjectIDs.Order1, orderContainers[0].ID);
    Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orderContainers[1].ID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "The value of the provided ObjectID is of type 'System.String', but only 'System.Guid' is supported.\r\nParameter name: relatedID")]
  public void LoadDataContainersByRelatedIDWithObjectIDWithValueOfInvalidType ()
  {
    ObjectID invalidCustomerID = new ObjectID (
        DomainObjectIDs.Customer1.StorageProviderID, DomainObjectIDs.Customer1.ClassID, DomainObjectIDs.Customer1.Value.ToString ());

    ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions.GetByClassID ("Order");

    Provider.LoadDataContainersByRelatedID (orderDefinition, "Customer", invalidCustomerID);
  }
}
}
