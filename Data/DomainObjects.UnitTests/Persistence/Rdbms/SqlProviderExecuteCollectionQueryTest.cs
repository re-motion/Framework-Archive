using System;
using System.Data;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderExecuteCollectionQueryTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderExecuteCollectionQueryTest ()
  {
  }

  // methods and properties

  [Test]
  public void ExecuteCollectionQuery ()
  {
    Query query = new Query ("OrderQuery");
    query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);

    DataContainerCollection orderContainers = Provider.ExecuteCollectionQuery (query);

    Assert.IsNotNull (orderContainers);
    Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order1));
    Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.OrderWithoutOrderItem));
  }

  [Test]
  public void ExecuteCollectionQueryWithAllDataTypes ()
  {
    DataContainerCollection actualContainers = Provider.ExecuteCollectionQuery (new Query ("QueryWithAllDataTypes"));

    Assert.IsNotNull (actualContainers);
    Assert.AreEqual (1, actualContainers.Count);

    DataContainer expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
    DataContainerChecker checker = new DataContainerChecker ();
    checker.Check (expectedContainer, actualContainers[0]);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void ExecuteCollectionQueryWithScalarQuery ()
  {
    Provider.ExecuteCollectionQuery (new Query ("OrderNoSumByCustomerNameQuery"));
  }
}
}
