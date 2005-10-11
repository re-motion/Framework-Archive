using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
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
    ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

    DataContainerCollection orderContainers = Provider.LoadDataContainersByRelatedID (
        orderDefinition, "Customer", DomainObjectIDs.Customer1);

    Assert.AreEqual (2, orderContainers.Count);
    Assert.AreEqual (DomainObjectIDs.Order1, orderContainers[0].ID);
    Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orderContainers[1].ID);
  }

  [Test]
  public void LoadDataContainersByRelatedIDOfDifferentStorageProvider ()
  {
    ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

    DataContainerCollection orderContainers = Provider.LoadDataContainersByRelatedID (orderDefinition, "Official", DomainObjectIDs.Official1);
    Assert.IsNotNull (orderContainers);
    Assert.AreEqual (5, orderContainers.Count);
  }
}
}
