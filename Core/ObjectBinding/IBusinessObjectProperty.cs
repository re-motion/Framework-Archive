using System;
using System.Collections;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectProperty
{
  /// <summary>
  ///   Indicates whether this property contains multiple values.
  /// </summary>
  /// <remarks>
  ///   Multiple values are provided via any type implementing IList.
  /// </remarks>
  bool IsList { get; }

  /// <summary>
  ///   Creates a list.
  /// </summary>
  /// <returns></returns>
  IList CreateList (int count);

  Type PropertyType { get; }

  /// <summary>
  ///   An identifier that uniquely defines this property within its class.
  /// </summary>
  string Identifier { get; }

  /// <summary>
  ///   The property name as presented to the user (may depend on the current culture).
  /// </summary>
  string DisplayName { get; }

  /// <summary>
  ///   Indicates whether this property is required.
  /// </summary>
  /// <remarks>
  ///   Setting required properties to null may result in an error.
  /// </remarks>
  bool IsRequired { get; }
 
  /// <summary>
  ///   Indicates whether this property can be accessed by the user.
  /// </summary>
  /// <remarks>
  ///   The result may depend on the user's authorization and/or the object.
  /// </remarks>
  /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
  bool IsAccessible (IBusinessObject obj);

  /// <summary>
  ///   Indicates whether this property can be modified by the user.
  /// </summary>
  /// <remarks>
  ///   The result may depend on the user's authorization and/or the object.
  /// </remarks>
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
