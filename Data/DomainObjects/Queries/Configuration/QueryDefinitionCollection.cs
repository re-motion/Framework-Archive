using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Represents a collection of <see cref="QueryDefinition"/>s.
/// </summary>
public class QueryDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinitionCollection</b> class.
  /// </summary>
  public QueryDefinitionCollection ()
  {
  }

  // standard constructor for collections

  /// <summary>
  /// Initializes a new <b>QueryDefinitionCollection</b> as a shallow copy of a given <see cref="QueryDefinitionCollection"/>.
  /// </summary>
  /// <remarks>The new <b>QueryDefinitionCollection</b> has the same items as the given <i>collection</i>.</remarks>
  /// <param name="collection">The <see cref="QueryDefinitionCollection"/> to copy.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><i>collection</i> is a null reference.</exception>
  public QueryDefinitionCollection (
      QueryDefinitionCollection collection,
      bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (QueryDefinition queryDefinition in collection)  
    {
      Add (queryDefinition);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  /// <summary>
  /// Returns the <see cref="QueryDefinition"/> identified through <i>queryID</i>. If no <see cref="QueryDefinition"/> can be found an exception is thrown.
  /// </summary>
  /// <param name="queryID">The <see cref="QueryDefinition.QueryID"/> of the <see cref="QueryDefinition"/> to be found.</param>
  /// <returns>The <see cref="QueryDefinition"/> identified through <i>queryID</i>.</returns>
  /// <exception cref="QueryConfigurationException">The <see cref="QueryDefinition"/> identified through <i>queryID</i> could not be found.</exception>
  public QueryDefinition GetMandatory (string queryID)
  {
    if (!Contains (queryID))
      throw CreateQueryConfigurationException ("QueryDefinition '{0}' does not exist.", queryID);

    return this[queryID];
  }

  private ArgumentException CreateArgumentException (string message, string parameterName, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), parameterName);
  }

  private QueryConfigurationException CreateQueryConfigurationException (
      string message, 
      params object[] args)
  {
    return new QueryConfigurationException (string.Format (message, args));
  }

  #region Standard implementation for "add-only" collections

  /// <summary>
  /// Determines whether an item is in the <see cref="QueryDefinitionCollection"/>.
  /// </summary>
  /// <param name="queryDefinition">The <see cref="QueryDefinition"/> to locate in the <see cref="QueryDefinitionCollection"/>.</param>
  /// <returns><b>true</b> if <i>queryDefinition</i> is found in the <see cref="QueryDefinitionCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><i>queryDefinition</i> is a null reference</exception>
  public bool Contains (QueryDefinition queryDefinition)
  {
    ArgumentUtility.CheckNotNull ("queryDefinition", queryDefinition);

    return Contains (queryDefinition.QueryID);
  }

  /// <summary>
  /// Determines whether an item is in the <see cref="QueryDefinitionCollection"/>.
  /// </summary>
  /// <param name="queryID">The <see cref="QueryDefinition.QueryID"/> of the <see cref="QueryDefinition"/> to locate in the <see cref="QueryDefinitionCollection"/>.</param>
  /// <returns><b>true</b> if the <see cref="QueryDefinition"/> with the <i>queryID</i> is found in the <see cref="QueryDefinitionCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><i>queryID</i> is a null reference</exception>
  public bool Contains (string queryID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
    return BaseContainsKey (queryID);
  }

  /// <summary>
  /// Gets or sets the <see cref="QueryDefinition"/> with a given <i>index</i> in the <see cref="QueryDefinitionCollection"/>.
  /// </summary>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <i>index</i> is less than zero.<br /> -or- <br />
  ///   <i>index</i> is equal to or greater than the number of items in the collection.
  /// </exception>
  public QueryDefinition this [int index]  
  {
    get { return (QueryDefinition) BaseGetObject (index); }
  }

  /// <summary>
  /// Gets the <see cref="QueryDefinition"/> with a given <see cref="QueryDefinition.QueryID"/> from the <see cref="QueryDefinitionCollection"/>.
  /// </summary>
  /// <remarks>The indexer returns null if the given <i>queryID</i> was not found.</remarks>
  public QueryDefinition this [string queryID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
      return (QueryDefinition) BaseGetObject (queryID); 
    }
  }

  /// <summary>
  /// Adds a <see cref="QueryDefinition"/> to the collection.
  /// </summary>
  /// <param name="queryDefinition">The <see cref="QueryDefinition"/> to add.</param>
  /// <returns>The zero-based index where the <i>queryDefinition</i> has been added.</returns>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentNullException"><i>queryDefinition</i> is a null reference.</exception>
  public int Add (QueryDefinition queryDefinition)  
  {
    ArgumentUtility.CheckNotNull ("queryDefinition", queryDefinition);

    if (Contains (queryDefinition.QueryID))
    {
      throw CreateArgumentException (
          "QueryDefinition '{0}' already exists in collection.", "queryDefinition", queryDefinition.QueryID);
    }

    return BaseAdd (queryDefinition.QueryID, queryDefinition);
  }

  #endregion
}
}
