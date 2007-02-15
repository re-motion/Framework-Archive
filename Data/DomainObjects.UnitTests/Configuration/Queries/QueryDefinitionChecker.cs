using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
  public class QueryDefinitionChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public QueryDefinitionChecker ()
    {
    }

    // methods and properties

    public void Check (QueryDefinitionCollection expectedQueries, QueryDefinitionCollection actualQueries)
    {
      Assert.AreEqual (expectedQueries.Count, actualQueries.Count,
          string.Format ("Number of queries does not match. Expected: {0}, actual: {1}",
          expectedQueries.Count, actualQueries.Count));

      foreach (QueryDefinition expectedQuery in expectedQueries)
      {
        QueryDefinition actualQuery = actualQueries[expectedQuery.ID];
        CheckQuery (expectedQuery, actualQuery);
      }
    }

    private void CheckQuery (QueryDefinition expectedQuery, QueryDefinition actualQuery)
    {
      Assert.AreEqual (expectedQuery.StorageProviderID, actualQuery.StorageProviderID,
          string.Format ("ProviderID of query definitions does not match. Expected: {0}, actual: {1}",
          expectedQuery.ID,
          expectedQuery.StorageProviderID,
          actualQuery.StorageProviderID));

      Assert.AreEqual (expectedQuery.Statement, actualQuery.Statement,
          string.Format ("Statement of query definitions does not match. Expected: {0}, actual: {1}",
          expectedQuery.ID,
          expectedQuery.Statement,
          actualQuery.Statement));

      Assert.AreEqual (expectedQuery.QueryType, actualQuery.QueryType,
          string.Format ("QueryType of query definitions does not match. Expected: {0}, actual: {1}",
          expectedQuery.ID,
          expectedQuery.QueryType,
          actualQuery.QueryType));

      Assert.AreEqual (expectedQuery.CollectionType, actualQuery.CollectionType,
          string.Format ("CollectionType of query definitions does not match. Expected: {0}, actual: {1}",
          expectedQuery.ID,
          expectedQuery.CollectionType,
          actualQuery.CollectionType));
    }
  }
}
