using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectProperty
{
  bool IsList { get; }
  Type PropertyType { get; }
  string Identifier { get; }
  string DisplayName { get; }
  NaBoolean IsRequired { get; }
 
  /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
  bool IsAccessible (IBusinessObject obj);

  /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
  bool IsReadOnly (IBusinessObject obj);
}

public interface IBusinessObjectStringProperty: IBusinessObjectProperty
{
  NaInt32 MaxLength { get; }
}

public interface IBusinessObjectNumericProperty: IBusinessObjectProperty
{
  bool AllowNegative { get; }
}

public interface IBusinessObjectDoubleProperty: IBusinessObjectNumericProperty
{
}

public interface IBusinessObjectInt32Property: IBusinessObjectNumericProperty
{
}

public interface IBusinessObjectDateProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectDateTimeProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectReferenceProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectBooleanProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectEnumerationProperty: IBusinessObjectProperty
{
  IEnumerationValueInfo[] GetAllValues ();
  IEnumerationValueInfo[] GetEnabledValues ();

  IEnumerationValueInfo GetValueInfo (object value);
  IEnumerationValueInfo GetValueInfo (string identifier);
}

public interface IEnumerationValueInfo
{
  /// <summary>
  ///   The object representing the original value, e.g. a System.Enum type.
  /// </summary>
  object Value { get; }

  /// <summary>
  ///   The string identifier representing the value.
  /// </summary>
  string Identifier { get; }

  /// <summary>
  ///   The string presented to the user.
  /// </summary>
  string DisplayName { get; }

  /// <summary>
  ///   Indicates whether this value should be presented as an option to the user. (If not, existing objects might still use this value.)
  /// </summary>
  bool IsEnabled { get; }
}

public class EnumerationValueInfo: IEnumerationValueInfo
{
  private object _value;
  private string _identifier;
  private string _displayName;
  private bool _isEnabled; 

  public EnumerationValueInfo (object value, string identifier, string displayName, bool isEnabled)
  {
    _value = value;
    _identifier = identifier;
    _displayName = displayName;
    _isEnabled = isEnabled;
  }

  public object Value
  {
    get { return _value; }
  }

  public string Identifier
  {
    get { return _identifier; }
  }

  public string DisplayName
  {
    get { return _displayName; }
  }

  public bool IsEnabled
  {
    get { return _isEnabled; }
  }
}

}
