using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class ProviderLoadDataContainersByRelatedID : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ProviderLoadDataContainersByRelatedID ()
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
}
}
