using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
public class QueryParameterCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueryParameterCollection ()
  {
  }

  // standard constructor for collections
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

  public void Add (string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);

    Add (new QueryParameter (parameterName, parameterValue));
  }

  public void Add (string parameterName, object parameterValue, QueryParameterType parameterType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
    ArgumentUtility.CheckValidEnumValue (parameterType, "parameterType");

    Add (new QueryParameter (parameterName, parameterValue, parameterType));
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (QueryParameter queryParameter)
  {
    ArgumentUtility.CheckNotNull ("queryParameter", queryParameter);

    return Contains (queryParameter.Name);
  }

  public bool Contains (string name)
  {
    return base.ContainsKey (name);
  }

  public QueryParameter this [int index]  
  {
    get { return (QueryParameter) GetObject (index); }
  }

  public QueryParameter this [string name]  
  {
    get { return (QueryParameter) GetObject (name); }
  }

  public void Add (QueryParameter value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);
    base.Add (value.Name, value);
  }

  #endregion
}
}
