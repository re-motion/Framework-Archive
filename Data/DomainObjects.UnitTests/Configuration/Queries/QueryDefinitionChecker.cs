using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Queries;

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
        "Number of queries does not match. Expected: {0}, actual: {1}", 
        expectedQueries.Count, actualQueries.Count);

    foreach (QueryDefinition expectedQuery in expectedQueries)
    {
      QueryDefinition actualQuery = actualQueries[expectedQuery.QueryID];
      CheckQuery (expectedQuery, actualQuery);
    }
  }

  private void CheckQuery (QueryDefinition expectedQuery, QueryDefinition actualQuery)
  {
    Assert.AreEqual (expectedQuery.StorageProviderID, actualQuery.StorageProviderID, 
        "ProviderID of query definitions does not match. Expected: {0}, actual: {1}", 
        expectedQuery.QueryID,  
        expectedQuery.StorageProviderID, 
        actualQuery.StorageProviderID);

    Assert.AreEqual (expectedQuery.Statement, actualQuery.Statement, 
        "Statement of query definitions does not match. Expected: {0}, actual: {1}", 
        expectedQuery.QueryID,  
        expectedQuery.Statement, 
        actualQuery.Statement);
    
    Assert.AreEqual (expectedQuery.QueryType, actualQuery.QueryType, 
        "QueryType of query definitions does not match. Expected: {0}, actual: {1}", 
        expectedQuery.QueryID,  
        expectedQuery.QueryType, 
        actualQuery.QueryType);
    
    Assert.AreEqual (expectedQuery.CollectionType, actualQuery.CollectionType, 
        "CollectionType of query definitions does not match. Expected: {0}, actual: {1}", 
        expectedQuery.QueryID,  
        expectedQuery.CollectionType, 
        actualQuery.CollectionType);    
  }
}
}
