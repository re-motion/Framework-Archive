using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects
{
[Serializable]
public class ValueTooLongException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private int _maxLength;

  // construction and disposing

  public ValueTooLongException () : base ("Property value too long.") 
  {
  }

  public ValueTooLongException (string message) : base (message) 
  {
  }
  
  public ValueTooLongException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected ValueTooLongException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _propertyName = info.GetString ("PropertyName");
    _maxLength = info.GetInt32 ("MaxLength");
  }

  public ValueTooLongException (string propertyName, int maxLength) : this (
      string.Format (
          "Value for property '{0}' is too long. Maximum number of characters: {1}.", 
          propertyName, maxLength), 
      propertyName,
      maxLength)
  {
  }

  public ValueTooLongException (string message, string propertyName, int maxLength) : base (message) 
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _propertyName = propertyName;
    _maxLength = maxLength;
  }

  // methods and properties

  public string PropertyName
  {
    get { return _propertyName; }
  }

  public int MaxLength
  {
    get { return _maxLength; }
  }

  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("PropertyName", _propertyName);
    info.AddValue ("MaxLength", _maxLength);
  }
}
}
