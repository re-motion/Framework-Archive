using System;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
//TODO documentation: Write summary for class
[Serializable]
public class ValueTooLongException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private int _maxLength;

  // construction and disposing

  public ValueTooLongException () : this ("Property value too long.") 
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

  /// <summary>
  /// Sets the SerializationInfo object with the parameter name and additional exception information.
  /// </summary>
  /// <param name="info">The object that holds the serialized object data.</param>
  /// <param name="context">The contextual information about the source or destination.</param>
  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("PropertyName", _propertyName);
    info.AddValue ("MaxLength", _maxLength);
  }
}
}
