using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
public class QueryParameter
{
  // types

  // static members and constants

  // member fields

  private string _name;
  private object _value;
  private QueryParameterType _parameterType;

  // construction and disposing

  public QueryParameter (string name, object value) : this (name, value, QueryParameterType.Value)
  {
  }

  public QueryParameter (string name, object value, QueryParameterType parameterType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);
    ArgumentUtility.CheckValidEnumValue (parameterType, "parameterType");

    _name = name;
    _value = value;
    _parameterType = parameterType;
  }

  // methods and properties

  public string Name
  {
    get { return _name; }
  }

  public object Value
  {
    get { return _value; }
  }

  public QueryParameterType ParameterType
  {
    get { return _parameterType; }
  }
}
}
