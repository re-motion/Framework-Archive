using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
public class QueryDefinition
{
  // types

  // static members and constants

  // member fields

  private string _queryID;
  private string _storageProviderID;
  private string _statement;
  private QueryType _queryType;
  private Type _collectionType;

  // construction and disposing

  public QueryDefinition (string queryID, string storageProviderID, string statement, QueryType queryType)
      : this (queryID, storageProviderID, statement, queryType, null)
  {
  }

  public QueryDefinition (
      string queryID, 
      string storageProviderID,
      string statement, 
      QueryType queryType, 
      Type collectionType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    ArgumentUtility.CheckNotNullOrEmpty ("statement", statement);
    ArgumentUtility.CheckValidEnumValue (queryType, "queryType");

    _queryID = queryID;
    _storageProviderID = storageProviderID;
    _statement = statement;
    _queryType = queryType;
    _collectionType = collectionType;
  }

  // methods and properties

  public string QueryID
  {
    get { return _queryID; }
  }

  public string StorageProviderID
  {
    get { return _storageProviderID; }
  }

  public string Statement
  {
    get { return _statement; }
  }

  public QueryType QueryType
  {
    get { return _queryType; }
  }

  public Type CollectionType
  {
    get { return _collectionType; }
  }
}
}
