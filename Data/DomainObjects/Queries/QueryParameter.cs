using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
/// <summary>
/// Represents a parameter that is used in a query.
/// </summary>
public class QueryParameter
{
  // types

  // static members and constants

  // member fields

  private string _name;
  private object _value;
  private QueryParameterType _parameterType;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <see cref="QueryParameter"/> class with a <see cref="ParameterType"/> of <see cref="QueryParameterType.Value"/>.
  /// </summary>
  /// <param name="name">The name of the parameter.</param>
  /// <param name="value">The value of the parameter.</param>
  public QueryParameter (string name, object value) : this (name, value, QueryParameterType.Value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="QueryParameter"/> class.
  /// </summary>
  /// <param name="name">The name of the parameter.</param>
  /// <param name="value">The value of the parameter.</param>
  /// <param name="parameterType">The <see cref="QueryParameterType"/> of the parameter.</param>
  /// <exception cref="System.ArgumentNullException"><i>name</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>name</i> is an empty string.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><i>parameterType</i> is not a valid enum value.</exception>
  public QueryParameter (string name, object value, QueryParameterType parameterType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);
    ArgumentUtility.CheckValidEnumValue (parameterType, "parameterType");

    _name = name;
    _value = value;
    _parameterType = parameterType;
  }

  // methods and properties

  /// <summary>
  /// Gets the name of the <see cref="QueryParameter"/>.
  /// </summary>
  public string Name
  {
    get { return _name; }
  }

  /// <summary>
  /// Gets the value of the <see cref="QueryParameter"/>.
  /// </summary>
  public object Value
  {
    get { return _value; }
  }

  /// <summary>
  /// Gets the <see cref="QueryParameterType"/> of the <see cref="QueryParameter"/>.
  /// </summary>
  public QueryParameterType ParameterType
  {
    get { return _parameterType; }
  }
}
}
