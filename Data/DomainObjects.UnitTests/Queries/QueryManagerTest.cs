using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class QueryManagerTest : ClientTransactionBaseTest
  {
    private RootQueryManager _queryManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _queryManager = new RootQueryManager (ClientTransactionMock);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (ClientTransactionMock, _queryManager.ClientTransaction);
    }

    [Test]
    public void GetScalarWithoutParameter ()
    {
      Assert.AreEqual (42, _queryManager.GetScalar (new Query ("QueryWithoutParameter")));
    }

    [Test]
    public void GetCollection ()
    {
      Query query = new Query ("CustomerTypeQuery");
      query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      DomainObjectCollection customers = _queryManager.GetCollection (query);

      Assert.IsNotNull (customers);
      Assert.AreEqual (1, customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, customers[0].ID);
      Assert.AreEqual (typeof (Customer), customers[0].GetPublicDomainObjectType());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetCollectionWithScalarQuery ()
    {
      _queryManager.GetCollection (new Query ("OrderNoSumByCustomerNameQuery"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetScalarWithCollectionQuery ()
    {
      _queryManager.GetScalar (new Query ("OrderQuery"));
    }

    [Test]
    public void GetStoredProcedureResult ()
    {
      OrderCollection orders = (OrderCollection) _queryManager.GetCollection (new Query ("StoredProcedureQuery"));

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.Order2, orders[1].ID, "Order2");
    }

    [Test]
    public void GetStoredProcedureResultWithParameter ()
    {
      Query query = new Query ("StoredProcedureQueryWithParameter");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);
      OrderCollection orders = (OrderCollection) _queryManager.GetCollection (query);

      Assert.IsNotNull (orders, "OrderCollection is null");
      Assert.AreEqual (2, orders.Count, "Order count");
      Assert.AreEqual (DomainObjectIDs.Order1, orders[0].ID, "Order1");
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orders[1].ID, "OrderWithoutOrderItem");
    }
  }
}
