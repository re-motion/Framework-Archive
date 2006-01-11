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
/// <see cref="QueryConfiguration.Current"/> with the same <see cref="ID"/> again. Otherwise, a new object will be instantiated.
/// </remarks>
[Serializable]
public class QueryDefinition : ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private string _id;
  private string _storageProviderID;
  private string _statement;
  private QueryType _queryType;
  private Type _collectionType;
  
  // Note: _ispartOfQueryConfiguration is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private bool _ispartOfQueryConfiguration;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinition</b> class.
  /// </summary>
  /// <param name="queryID">The <paramref name="queryID"/> to be associated with this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="storageProviderID">The ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="statement">The <paramref name="statement"/> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <paramref name="storageProviderID"/> must understand the syntax of the <paramref name="statement"/>. Must not be <see langword="null"/>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="queryID"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="storageProviderID"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="statement"/> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <paramref name="queryID"/> is an empty string.<br /> -or- <br />
  ///   <paramref name="storageProviderID"/> is an empty string.<br /> -or- <br />
  ///   <paramref name="statement"/> is an empty string.
  /// </exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="queryType"/> is not a valid enum value.</exception>
  public QueryDefinition (string queryID, string storageProviderID, string statement, QueryType queryType)
      : this (queryID, storageProviderID, statement, queryType, null)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinition</b> class.
  /// </summary>
  /// <param name="queryID">The <paramref name="queryID"/> to be associated with this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="storageProviderID">The ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="statement">The <paramref name="statement"/> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <paramref name="storageProviderID"/> must understand the syntax of the <paramref name="statement"/>. Must not be <see langword="null"/>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <param name="collectionType">If <paramref name="queryType"/> specifies a collection to be returned, <paramref name="collectionType"/> specifies the type of the collection. If <paramref name="queryType"/> is <see langword="null"/>, <see cref="DomainObjectCollection"/> is used.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="queryID"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="storageProviderID"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="statement"/> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <paramref name="queryID"/> is an empty string.<br /> -or- <br />
  ///   <paramref name="storageProviderID"/> is an empty string.<br /> -or- <br />
  ///   <paramref name="statement"/> is an empty string.
  /// </exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="queryType"/> is not a valid enum value.</exception>
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

    _id = queryID;
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
    _id = info.GetString ("ID");
    _ispartOfQueryConfiguration = info.GetBoolean ("IsPartOfQueryConfiguration");

    if (!_ispartOfQueryConfiguration)
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
  [Obsolete ("Use property ID instead.", true)]
  // TODO: Remove this property after 1.3.2006.
  public string QueryID
  {
    get { return _id; }
  }

  /// <summary>
  /// Gets the unique ID for this <b>QueryDefinition</b>.
  /// </summary>
  public string ID
  {
    get { return _id; }
  }

  /// <summary>
  /// Gets the ID of the <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> responsible for executing instances of this <b>QueryDefinition</b>.
  /// </summary>
  public string StorageProviderID
  {
    get { return _storageProviderID; }
  }

  /// <summary>
  /// Gets the <paramref name="statement"/> of the <b>QueryDefinition</b>. The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> specified through <see cref="StorageProviderID"/> must understand the syntax of the <b>Statement</b>.
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
    GetObjectData (info, context);
  }

  /// <summary>
  /// Populates a specified <see cref="System.Runtime.Serialization.SerializationInfo"/> with the 
  /// data needed to serialize the current <see cref="QueryDefinition"/> instance. See remarks 
  /// on <see cref="QueryDefinition"/> for further details.
  /// </summary>
  /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
  /// <param name="context">The contextual information about the source or destination of the serialization.</param>
  /// <note type="inheritinfo">Overwrite this method to support serialization of derived classes.</note>
  protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("ID", _id);

    bool isPartOfQueryConfiguration = QueryConfiguration.Current.Contains (this);
    info.AddValue ("IsPartOfQueryConfiguration", isPartOfQueryConfiguration);

    if (!isPartOfQueryConfiguration)
    {
      info.AddValue ("StorageProviderID", _storageProviderID);
      info.AddValue ("Statement", _statement);
      info.AddValue ("QueryType", _queryType);
      info.AddValue ("CollectionType", _collectionType);
    }
  }

  #endregion

  #region IObjectReference Members

  /// <summary>
  /// Returns a reference to the real object that should be deserialized. See remarks 
  /// on <see cref="QueryDefinition"/> for further details.
  /// </summary>
  /// <param name="context">The source and destination of a given serialized stream.</param>
  /// <returns>Returns the actual <see cref="QueryDefinition"/>.</returns>
  object IObjectReference.GetRealObject (StreamingContext context)
  {
    if (_ispartOfQueryConfiguration)
      return QueryConfiguration.Current.QueryDefinitions.GetMandatory (_id);
    else
      return this;
  }

  #endregion
}
}
