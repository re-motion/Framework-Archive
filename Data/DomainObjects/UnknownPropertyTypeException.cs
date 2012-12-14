using System;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// The exception that is thrown when the <see cref="System.Type"/> of a <see cref="PropertyValue"/> is not known.
/// </summary>
[Serializable]
public class UnknownPropertyTypeException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private Type _propertyType;

  // construction and disposing

  public UnknownPropertyTypeException () : this ("Unknown property type.") 
  {
  }

  public UnknownPropertyTypeException (string message) : base (message) 
  {
  }
  
  public UnknownPropertyTypeException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected UnknownPropertyTypeException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _propertyType = (Type) info.GetValue ("PropertyType", typeof (Type));
  }

  public UnknownPropertyTypeException (Type propertyType) : this (
      string.Format ("Unknown property type '{0}'.", propertyType.FullName), propertyType)
  {
  }

  public UnknownPropertyTypeException (string message, Type propertyType) : base (message) 
  {
    ArgumentUtility.CheckNotNull ("propertyType", propertyType);

    _propertyType = propertyType;
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="System.Type"/> that was unknown.
  /// </summary>
  public Type PropertyType
  {
    get { return _propertyType; }
  }

  /// <summary>
  /// Sets the SerializationInfo object with the parameter name and additional exception information.
  /// </summary>
  /// <param name="info">The object that holds the serialized object data.</param>
  /// <param name="context">The contextual information about the source or destination.</param>
  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("PropertyType", _propertyType);
  }
}
}
