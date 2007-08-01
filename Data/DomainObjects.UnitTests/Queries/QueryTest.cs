using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class QueryTest : StandardMappingTest
  {
    [Test]
    public void InitializeWithQueryID ()
    {
      QueryParameterCollection parameters = new QueryParameterCollection ();
      Query query = new Query ("OrderQuery", parameters);

      QueryDefinition definition = QueryConfiguration.Current["OrderQuery"];
      Assert.AreSame (definition, query.Definition);
      Assert.AreEqual (definition.ID, query.ID);
      Assert.AreEqual (definition.CollectionType, query.CollectionType);
      Assert.AreEqual (definition.QueryType, query.QueryType);
      Assert.AreEqual (definition.Statement, query.Statement);
      Assert.AreEqual (definition.StorageProviderID, query.StorageProviderID);
      Assert.AreSame (parameters, query.Parameters);
    }

    [Test]
    public void InitializeWithQueryDefinition ()
    {
      QueryParameterCollection parameters = new QueryParameterCollection ();

      QueryDefinition definition = QueryFactory.CreateOrderQueryWithCustomCollectionType ();
      Query query = new Query (definition, parameters);

      Assert.AreSame (definition, query.Definition);
      Assert.AreEqual (definition.ID, query.ID);
      Assert.AreSame (parameters, query.Parameters);
    }
  }
}
