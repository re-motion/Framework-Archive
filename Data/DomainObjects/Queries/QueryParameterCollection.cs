using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
/// <summary>
/// Represents a collection of <see cref="QueryParameter"/> objects.
/// </summary>
[Serializable]
public class QueryParameterCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>QueryParameterCollection</b>.
  /// </summary>
  public QueryParameterCollection ()
  {
  }

  // standard constructor for collections
  /// <summary>
  /// Initializes a new <b>QueryParameterCollection</b> as a shallow copy of a given <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <remarks>
  /// The new <b>QueryParameterCollection</b> has the same items as the given <i>collection</i>.
  /// </remarks>
  /// <param name="collection">The <see cref="QueryParameterCollection"/> to copy. Must not be <see langword="null"/>.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><i>collection</i> is <see langword="null"/>.</exception>
  public QueryParameterCollection (QueryParameterCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (QueryParameter parameter in collection)
    {
      Add (parameter);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  /// <summary>
  /// Adds a new <see cref="QueryParameter"/> to the collection with <see cref="QueryParameter.ParameterType"/> of Value.
  /// </summary>
  /// <param name="parameterName">The <see cref="QueryParameter.Name"/> of the new parameter. Must not be <see langword="null"/>.</param>
  /// <param name="parameterValue">The <see cref="QueryParameter.Value"/> of the new parameter.</param>
  /// <exception cref="System.ArgumentNullException"><i>parameterName</i> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>parameterName</i> is an empty string.</exception>
  public void Add (string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);

    Add (new QueryParameter (parameterName, parameterValue));
  }

  /// <summary>
  /// Adds a new <see cref="QueryParameter"/> to the collection.
  /// </summary>
  /// <param name="parameterName">The <see cref="QueryParameter.Name"/> of the new parameter. Must not be <see langword="null"/>.</param>
  /// <param name="parameterValue">The <see cref="QueryParameter.Value"/> of the new parameter.</param>
  /// <param name="parameterType">The <see cref="QueryParameterType"/> of the new parameter.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>parameterName</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>parameterName</i> is an empty string.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><i>parameterType</i> is not a valid enum value.</exception>
  public void Add (string parameterName, object parameterValue, QueryParameterType parameterType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
    ArgumentUtility.CheckValidEnumValue (parameterType, "parameterType");

    Add (new QueryParameter (parameterName, parameterValue, parameterType));
  }

  #region Standard implementation for "add-only" collections

  /// <summary>
  /// Determines whether an item is in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <param name="queryParameter">The <see cref="QueryParameter"/> to locate in the collection. Must not be <see langword="null"/>.</param>
  /// <returns><b>true</b> if <i>queryParameter</i> is found in the <see cref="QueryParameterCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><i>queryParameter</i> is <see langword="null"/></exception>
  public bool Contains (QueryParameter queryParameter)
  {
    ArgumentUtility.CheckNotNull ("queryParameter", queryParameter);

    return Contains (queryParameter.Name);
  }

  /// <summary>
  /// Determines whether an item is in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <param name="name">The <see cref="QueryParameter.Name"/> of the <see cref="QueryParameter"/> to locate in the collection. Must not be <see langword="null"/>.</param>
  /// <returns><b>true</b> if a <see cref="QueryParameter"/> with a <see cref="QueryParameter.Name"/> of <i>name</i> is found in the <see cref="QueryParameterCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><i>name</i> is <see langword="null"/></exception>
  public bool Contains (string name)
  {
    return BaseContainsKey (name);
  }

  /// <summary>
  /// Gets the <see cref="QueryParameter"/> with a given <i>index</i> in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  public QueryParameter this [int index]  
  {
    get { return (QueryParameter) BaseGetObject (index); }
  }

  /// <summary>
  /// Gets the <see cref="QueryParameter"/> with a given <i>name</i> in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <remarks>The indexer returns <see langword="null"/> if the given <i>name</i> was not found.</remarks>
  public QueryParameter this [string name]  
  {
    get { return (QueryParameter) BaseGetObject (name); }
  }

  /// <summary>
  /// Adds a <see cref="QueryParameter"/> to the collection.
  /// </summary>
  /// <param name="parameter">The <see cref="QueryParameter"/> to add.</param>
  /// <returns>The zero-based index where <i>parameter</i> has been added.</returns>
  public int Add (QueryParameter parameter)  
  {
    ArgumentUtility.CheckNotNull ("parameter", parameter);
    
    return BaseAdd (parameter.Name, parameter);
  }

  #endregion
}
}
