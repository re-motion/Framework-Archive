using System;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Represents the definition of a query.
/// </summary>
/// <remarks>
/// During the serialization process the object determines if it is part of <see cref="QueryConfiguration.Current"/> 
/// and serializes this information. If it was then the deserialized object will be the reference from 
/// <see cref="QueryConfiguration.Current"/> with the same <see cref="QueryID"/> again. Otherwise, a new object will be instantiated.
/// </remarks>
[Serializable]
public class QueryDefinition : ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private string _queryID;
  private string _storageProviderID;
  private string _statement;
  private QueryType _queryType;
  private Type _collectionType;
  
  // Note: _isInQueryConfiguration is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private bool _isInQueryConfiguration;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinition</b> class.
  /// </summary>
  /// <param name="queryID">The <i>queryID</i> to be associated with this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="storageProviderID">The ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="statement">The <i>statement</i> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <i>storageProviderID</i> must understand the syntax of the <i>statement</i>. Must not be <see langword="null"/>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>queryID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>storageProviderID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>statement</i> is <see langword="null"/>.
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
  /// <param name="queryID">The <i>queryID</i> to be associated with this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="storageProviderID">The ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="statement">The <i>statement</i> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <i>storageProviderID</i> must understand the syntax of the <i>statement</i>. Must not be <see langword="null"/>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <param name="collectionType">If <i>queryType</i> specifies a collection to be returned, <i>collectionType</i> specifies the type of the collection. If <i>queryType</i> is <see langword="null"/>, <see cref="DomainObjectCollection"/> is used.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>queryID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>storageProviderID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>statement</i> is <see langword="null"/>.
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
      throw new ArgumentException (string.Format ("The scalar query '{0}' must not specify a collectionType.", queryID), "collectionType");

    if (queryType == QueryType.Collection && collectionType == null)
      collectionType = typeof (DomainObjectCollection);

    if (collectionType != null 
        && !collectionType.Equals (typeof (DomainObjectCollection)) 
        && !collectionType.IsSubclassOf (typeof (DomainObjectCollection)))
    {
      throw new ArgumentException (string.Format (
          "The collectionType of query '{0}' must be 'Rubicon.Data.DomainObjects.DomainObjectCollection' or derived from it.", queryID), "collectionType");
    }

    _queryID = queryID;
    _storageProviderID = storageProviderID;
    _statement = statement;
    _queryType = queryType;
    _collectionType = collectionType;
  }

  /// <summary>
  /// This constructor is used for deserializing the object and is not intended to be used directly from code.
  /// </summary>
  /// <param name="info">The data needed to serialize or deserialize an object. </param>
  /// <param name="context">The source and destination of a given serialized stream.</param>
  protected QueryDefinition (SerializationInfo info, StreamingContext context)
  {
    _queryID = info.GetString ("QueryID");
    _isInQueryConfiguration = info.GetBoolean ("IsInQueryConfiguration");

    if (!_isInQueryConfiguration)
    {
      _storageProviderID = info.GetString ("StorageProviderID");
      _statement = info.GetString ("Statement");
      _queryType = (QueryType) info.GetValue ("QueryType", typeof (QueryType));
      _collectionType = (Type) info.GetValue ("CollectionType", typeof (Type));
    }
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

  #region IObjectReference Members

  /// <summary>
  /// Returns a reference to the real object that should be deserialized. See remarks 
  /// on <see cref="QueryDefinition"/> for further details.
  /// </summary>
  /// <param name="context">The source and destination of a given serialized stream.</param>
  /// <returns></returns>
  object IObjectReference.GetRealObject (StreamingContext context)
  {
    if (!_isInQueryConfiguration)
      return this;
    else
      return QueryConfiguration.Current.QueryDefinitions.GetMandatory (_queryID);
  }

  #endregion

  #region ISerializable Members

  /// <summary>
  /// Populates a specified <see cref="System.Runtime.Serialization.SerializationInfo"/> with the 
  /// data needed to serialize the current <see cref="QueryDefinition"/> instance. See remarks 
  /// on <see cref="QueryDefinition"/> for further details.
  /// </summary>
  /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
  /// <param name="context">The contextual information about the source or destination of the serialization.</param>
  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("QueryID", _queryID);

    bool isInQueryConfiguration = QueryConfiguration.Current[_queryID] != null;
    info.AddValue ("IsInQueryConfiguration", isInQueryConfiguration);

    if (!isInQueryConfiguration)
    {
      info.AddValue ("StorageProviderID", _storageProviderID);
      info.AddValue ("Statement", _statement);
      info.AddValue ("QueryType", _queryType);
      info.AddValue ("CollectionType", _collectionType);
    }
  }

  #endregion
}
}
