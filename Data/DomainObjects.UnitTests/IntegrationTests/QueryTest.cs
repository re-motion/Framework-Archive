using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class QueryTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void GetCollectionWithNullValue ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      Query query = new Query ("QueryWithNullValues");
      queryManager.GetCollection (query);
    }

  }
}