using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Queries
{
[TestFixture]
public class QueryManagerTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private QueryManager _queryManager;

  // construction and disposing

  public QueryManagerTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _queryManager = new QueryManager (ClientTransactionMock);
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
    Assert.AreEqual (typeof (Customer), customers[0].GetType ());
  }
}
}
