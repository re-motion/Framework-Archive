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
  /// <remarks>
  ///   Use this method to create a new list in order to ensure that the correct list type is used
  ///   (Array, ArrayList, etc.)
  /// </remarks>
  /// <returns> 
  ///   A new list with the specified number of empty elements.
  /// </returns>
  IList CreateList (int count);

  /// <summary>
  ///   The type of a single value item.
  /// </summary>
  /// <remarks>
  ///   If <see cref="IsList"/> is <see langword="false"/>, the item type is the same as 
  ///   <see cref="PropertyType"/>. 
  ///   Otherwise, the item type is the type of a list item.
  /// </remarks>
  Type ItemType { get; }

  /// <summary>
  ///   The type of the property.
  /// </summary>
  /// <remarks> 
  ///   This is the type of elements returned by <see cref="IBusinessObject.GetProperty"/> 
  ///   and set via <see cref="IBusinessObject.SetProperty"/>.
  ///   If <see cref="IsList"/> is <see langword="true"/>, the property type must implement 
  ///   the <see cref="IList"/> interface, and the items contained in this list must have the 
  ///   type <see cref="ItemType"/>.
  /// </remarks>
  Type PropertyType { get; }

  /// <summary>
  ///   An identifier that uniquely defines this property within its class.
  /// </summary>
  string Identifier { get; }

  /// <summary>
  ///   The property name as presented to the user.
  /// </summary>
  /// <remarks>
  ///   The value of this property may depend on the current culture.
  /// </remarks>
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

  IBusinessObjectProvider BusinessObjectProvider { get; }
}

public interface IBusinessObjectStringProperty: IBusinessObjectProperty
{
  /// <summary>
  ///   The maximum length of a string assigned to the property, or <see cref="NaInt32.Null"/> 
  ///   if no maximum length is defined.
  /// </summary>
  NaInt32 MaxLength { get; }
}

public interface IBusinessObjectNumericProperty: IBusinessObjectProperty
{
  /// <summary>
  ///   Specifies whether negative numbers are valid for the property.
  /// </summary>
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
  /// <summary>
  ///   Gets the class information for elements of this property.
  /// </summary>
  IBusinessObjectClass ReferenceClass { get; }

  bool SupportsSearchAvailableObjects { get; }
  IBusinessObjectWithIdentity[] SearchAvailableObjects (IBusinessObject obj, string searchStatement); 
}

public interface IBusinessObjectBooleanProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectEnumerationProperty: IBusinessObjectProperty
{
  IEnumerationValueInfo[] GetAllValues ();
  IEnumerationValueInfo[] GetEnabledValues ();

  IEnumerationValueInfo GetValueInfoByValue (object value);
  IEnumerationValueInfo GetValueInfoByIdentifier (string identifier);
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
  ///   Indicates whether this value should be presented as an option to the user. 
  ///   (If not, existing objects might still use this value.)
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

  public virtual string DisplayName
  {
    get { return _displayName; }
  }

  public bool IsEnabled
  {
    get { return _isEnabled; }
  }
}

}
