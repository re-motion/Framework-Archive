using System;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
[Serializable]
public class InvalidTypeException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private Type _expectedType;
  private Type _actualType;

  // construction and disposing

  public InvalidTypeException () : this ("Value does not match expected type.") 
  {
  }

  public InvalidTypeException (string message) : base (message) 
  {
  }
  
  public InvalidTypeException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected InvalidTypeException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _propertyName = info.GetString ("PropertyName");
    _expectedType = Type.GetType (info.GetString ("ExpectedType"));
    _actualType = Type.GetType (info.GetString ("ActualType"));
  }

  public InvalidTypeException (string propertyName, Type expectedType, Type actualType) : this (
      string.Format (
          "Actual type '{0}' of property '{1}' does not match expected type '{2}'.", 
          actualType, propertyName, expectedType), 
      propertyName,
      expectedType,
      actualType)
  {
  }

  public InvalidTypeException (string message, string propertyName, Type exptectedType, Type actualType) 
      : base (message) 
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("expectedType", exptectedType);
    ArgumentUtility.CheckNotNull ("actualType", actualType);

    _propertyName = propertyName;
    _expectedType = exptectedType;
    _actualType = actualType;
  }

  // methods and properties

  public string PropertyName
  {
    get { return _propertyName; }
  }

  public Type ExpectedType
  {
    get { return _expectedType; }
  }

  public Type ActualType
  {
    get { return _actualType; }
  }

  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("PropertyName", _propertyName);
    info.AddValue ("ExpectedType", _expectedType.FullName);
    info.AddValue ("ActualType", _actualType.FullName);
  }
}
}