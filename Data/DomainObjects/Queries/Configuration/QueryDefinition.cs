using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Represents the definition of a query.
/// </summary>
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

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinition</b> class.
  /// </summary>
  /// <param name="queryID">The <i>queryID</i> to be associated with this <b>QueryDefinition</b>.</param>
  /// <param name="storageProviderID">The ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>.</param>
  /// <param name="statement">The <i>statement</i> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <i>storageProviderID</i> must understand the syntax of the <i>statement</i>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>queryID</i> is a null reference.<br /> -or- <br />
  ///   <i>storageProviderID</i> is a null reference.<br /> -or- <br />
  ///   <i>statement</i> is a null reference.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>queryID</i> is an empty string.<br /> -or- <br />
  ///   <i>storageProviderID</i> is an empty string.<br /> -or- <br />
  ///   <i>statement</i> is an empty string.
  /// </exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><i>queryType</i> is not a valid enum value.</exception>
  public QueryDefinition (string queryID, string storageProviderID, string statement, QueryType queryType)
      : this (queryID, storageProviderID, statement, queryType, null)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinition</b> class.
  /// </summary>
  /// <param name="queryID">The <i>queryID</i> to be associated with this <b>QueryDefinition</b>.</param>
  /// <param name="storageProviderID">The ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>.</param>
  /// <param name="statement">The <i>statement</i> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <i>storageProviderID</i> must understand the syntax of the <i>statement</i>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <param name="collectionType">If <i>queryType</i> specifies a collection to be returned, <i>collectionType</i> specifies the type of the collection. If null, <see cref="DomainObjectCollection"/> is used.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>queryID</i> is a null reference.<br /> -or- <br />
  ///   <i>storageProviderID</i> is a null reference.<br /> -or- <br />
  ///   <i>statement</i> is a null reference.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>queryID</i> is an empty string.<br /> -or- <br />
  ///   <i>storageProviderID</i> is an empty string.<br /> -or- <br />
  ///   <i>statement</i> is an empty string.
  /// </exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><i>queryType</i> is not a valid enum value.</exception>
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

    if (queryType == QueryType.Scalar && collectionType != null)
      throw new ArgumentException ("A scalar query must not specify a collectionType.", "collectionType");

    if (queryType == QueryType.Collection && collectionType == null)
      collectionType = typeof (DomainObjectCollection);

    if (collectionType != null 
        && !collectionType.Equals (typeof (DomainObjectCollection)) 
        && !collectionType.IsSubclassOf (typeof (DomainObjectCollection)))
    {
      throw new ArgumentException (string.Format (
          "The collectionType of query '{0}' must be 'Rubicon.Data.DomainObjects.DomainObjectCollection' or derived from it.", queryID));
    }

    _queryID = queryID;
    _storageProviderID = storageProviderID;
    _statement = statement;
    _queryType = queryType;
    _collectionType = collectionType;
  }

  // methods and properties

  /// <summary>
  /// Gets the unique ID for this <b>QueryDefinition</b>.
  /// </summary>
  public string QueryID
  {
    get { return _queryID; }
  }

  /// <summary>
  /// Gets the ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>.
  /// </summary>
  public string StorageProviderID
  {
    get { return _storageProviderID; }
  }

  /// <summary>
  /// Gets the <i>statement</i> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <see cref="StorageProviderID"/> must understand the syntax of the <b>Statement</b>.
  /// </summary>
  public string Statement
  {
    get { return _statement; }
  }

  /// <summary>
  /// Gets the <see cref="QueryType"/> of this <b>QueryDefinition</b>.
  /// </summary>
  public QueryType QueryType
  {
    get { return _queryType; }
  }

  /// <summary>
  /// If <see cref="QueryType"/> specifies a collection to be returned, <b>CollectionType</b> specifies the type of the collection. The default is <see cref="DomainObjectCollection"/>. 
  /// </summary>
  public Type CollectionType
  {
    get { return _collectionType; }
  }
}
}
