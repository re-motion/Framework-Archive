using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects
{
[Serializable]
public class InvalidEnumDefinitionException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private Type _enumType;

  // construction and disposing

  public InvalidEnumDefinitionException () : base ("Enumeration does not contain any values.") 
  {
  }

  public InvalidEnumDefinitionException (string message) : base (message) 
  {
  }
  
  public InvalidEnumDefinitionException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected InvalidEnumDefinitionException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _enumType = (Type) info.GetValue ("EnumType", typeof (Type));
  }

  public InvalidEnumDefinitionException (Type enumType) : this (
      string.Format ("Enumeration '{0}' does not contain any values.", enumType.FullName), enumType)
  {
  }

  public InvalidEnumDefinitionException (string message, Type enumType) : base (message) 
  {
    ArgumentUtility.CheckNotNull ("enumType", enumType);

    _enumType = enumType;
  }

  // methods and properties

  public Type EnumType
  {
    get { return _enumType; }
  }

  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("EnumType", _enumType);
  }
}
}
