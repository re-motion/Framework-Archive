using System;
using System.Collections;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   The <b>IBusinessObjectProperty</b> interface provides functionality common to all business object property 
///   spezializations for the individual data types.
/// </summary>
/// <remarks>
///   A business object property only implementing the generic <b>IBusinessObjectProperty</b> should only be used
///   if the data type of the value accessed through this property is unknown. This rule is espacially important 
///   when using business object binding, since the controls used to access the values require a strong typed 
///   business object property.
/// </remarks>
public interface IBusinessObjectProperty
{
  /// <summary> Gets a flag indicating whether this property contains multiple values. </summary>
  /// <value> <see langword="true"/> if this property contains multiple values. </value>
  /// <remarks> Multiple values are provided via any type implementing <see cref="IList"/>. </remarks>
  bool IsList { get; }

  /// <summary> Creates a list. </summary>
  /// <returns> A new list with the specified number of empty elements. </returns>
  /// <remarks>
  ///   Use this method to create a new list in order to ensure that the correct list type is used
  ///   (<see cref="Array"/>, <see cref="ArrayList"/>, etc.)
  /// </remarks>
  IList CreateList (int count);

  /// <summary> Gets the type of a single value item. </summary>
  /// <remarks> If <see cref="IsList"/> is <see langword="false"/>, the item type is the same as 
  ///   <see cref="PropertyType"/>. 
  ///   Otherwise, the item type is the type of a list item.
  /// </remarks>
  Type ItemType { get; }

  /// <summary> Gets the type of the property. </summary>
  /// <remarks> 
  ///   <para>
  ///     This is the type of elements returned by the <see cref="IBusinessObject.GetProperty"/> method
  ///     and set via the <see cref="IBusinessObject.SetProperty"/> method.
  ///   </para><para>
  ///     If <see cref="IsList"/> is <see langword="true"/>, the property type must implement the <see cref="IList"/> 
  ///     interface, and the items contained in this list must have a type of <see cref="ItemType"/>.
  ///   </para>
  /// </remarks>
  Type PropertyType { get; }

  /// <summary> Gets an identifier that uniquely defines this property within its class. </summary>
  /// <value> A <see cref="String"/> by which this property can be found within its <see cref="IBusinessObjectClass"/>. </value>
  string Identifier { get; }

  /// <summary> Gets the property name as presented to the user. </summary>
  /// <value> The human readable identifier of this property. </value>
  /// <remarks> The value of this property may depend on the current culture. </remarks>
  string DisplayName { get; }

  /// <summary> Gets a flag indicating whether this property is required. </summary>
  /// <value> <see langword="true"/> if this property is required. </value>
  /// <remarks> Setting required properties to <see langword="null"/> may result in an error. </remarks>
  bool IsRequired { get; }
 
  /// <summary> Indicates whether this property can be accessed by the user. </summary>
  /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> of the <paramref name="obj"/>. </param>
  /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
  /// <returns> <see langword="true"/> if the user can access this property. </returns>
  /// <remarks> The result may depend on the class, the user's authorization and/or the instance value. </remarks>
  bool IsAccessible (IBusinessObjectClass objectClass, IBusinessObject obj);

  /// <summary> Indicates whether this property can be modified by the user. </summary>
  /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
  /// <returns> <see langword="true"/> if the user can set this property. </returns>
  /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
  bool IsReadOnly (IBusinessObject obj);

  /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this property. </summary>
  /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type. </value>
  /// <remarks>
  ///   <note type="inheritinfo">
  ///     Must not return <see langword="null"/>.
  ///   </note>
  /// </remarks>
  IBusinessObjectProvider BusinessObjectProvider { get; }
}

/// <summary> 
///   The <b>IBusinessObjectStringProperty</b> provides additional meta data for <see cref="String"/> values.
/// </summary>
public interface IBusinessObjectStringProperty: IBusinessObjectProperty
{
  /// <summary>
  ///   Getsthe the maximum length of a string assigned to the property, or <see cref="NaInt32.Null"/> if no maximum 
  ///   length is defined.
  /// </summary>
  /// <value> An instance of the <see cref="NaInt32"/> data type.</value>
  /// <remarks>
  ///   <note type="inheritinfo">
  ///     Must not return <see langword="null"/>.
  ///   </note>
  /// </remarks>
  NaInt32 MaxLength { get; }
}

/// <summary> 
///   The <b>IBusinessObjectNumericProperty</b> interface provides additional meta data for numeric values.
/// </summary>
/// <remarks>
///   This interface is used as a base for the specific numeric data type interfaces 
///   (e.g. <see cref="IBusinessObjectInt32Property"/>, <see cref="IBusinessObjectDoubleProperty"/>).
/// </remarks>
public interface IBusinessObjectNumericProperty: IBusinessObjectProperty
{
  /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
  /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
  bool AllowNegative { get; }
}

/// <summary> The <b>IBusinessObjectDoubleProperty</b> interface is used for accessing <see cref="Double"/> values. </summary>
public interface IBusinessObjectDoubleProperty: IBusinessObjectNumericProperty
{
}

/// <summary> The <b>IBusinessObjectInt32Property</b> interface is used for accessing <see cref="Int32"/> values. </summary>
public interface IBusinessObjectInt32Property: IBusinessObjectNumericProperty
{
}

/// <summary> 
///   The <b>IBusinessObjectDateProperty</b> interface is used for accessing <see cref="DateTime"/> values whose time
///   component will be ignored and potentially not persisted. 
/// </summary>
public interface IBusinessObjectDateProperty: IBusinessObjectProperty
{
}

/// <summary> The <b>IBusinessObjectDateTimeProperty</b> interface is used for accessing <see cref="DateTime"/> values. </summary>
public interface IBusinessObjectDateTimeProperty: IBusinessObjectProperty
{
}

/// <summary> 
///   The <b>IBusinessObjectReferenceProperty</b> interface is used for accessing references to other 
///   <see cref="IBusinessObject"/> instances.
/// </summary>
public interface IBusinessObjectReferenceProperty: IBusinessObjectProperty
{
  /// <summary> Gets the class information for elements of this property. </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> accessed through this property.
  /// </value>
  IBusinessObjectClass ReferenceClass { get; }

  /// <summary> 
  ///   Gets a flag indicating whether it is possible to get a list of the objects that can be assigned to this property.
  /// </summary>
  /// <value> <see langword="true"/> if it is possible to get the available objects from the object model. </value>
  /// <remarks> 
  ///   Use the <see cref="SearchAvailableObjects"/> method (or an object model specific overload) to get the list of 
  ///   objects.
  /// </remarks>
  bool SupportsSearchAvailableObjects { get; }

  /// <summary> 
  ///   Searches the object model for the <see cref="IBusinessObject"/> instances that can be assigned to this property.
  /// </summary>
  /// <param name="obj"> The business object for which to search for the possible objects to be referenced. </param>
  /// <param name="searchStatement"> 
  ///   A <see cref="String"/> containing a search statement. Can be <see langword="null"/>.
  /// </param>
  /// <returns> 
  ///   A list of the <see cref="IBusinessObject"/> instances available. Must not return <see langword="null"/>.
  /// </returns>
  /// <exception cref="NotSupportedException">
  ///   Thrown if <see cref="SupportsSearchAvailableObjects"/> evaluated <see langword="false"/> but this method
  ///   has been called anyways.
  /// </exception>
  /// <remarks> 
  ///   This method is used if the seach statement is entered via the Visual Studio .NET designer, for instance in
  ///   the <see cref="T:Rubicon.ObjectBinding.Web.Controls.BocReferenceValue"/> control.
  ///   <note type="inheritinfo">
  ///     If your object model cannot evaluate a search string, but allows search through a less generic method,
  ///     provide an overload, and document that getting the list of available objects is only possible during runtime.
  ///   </note>
  /// </remarks>
  IBusinessObjectWithIdentity[] SearchAvailableObjects (IBusinessObject obj, string searchStatement); 

  /// <summary>
  ///   Gets a flag indicating if <see cref="Create"/> may be called to implicitly create a new business object 
  ///   for editing in case the object reference is null.
  /// </summary>
  bool CreateIfNull { get; }

  /// <summary>
  ///   If <see cref="CreateIfNull"/> is <see langword="true"/>, this method can be used to create a new business 
  ///   object.
  /// </summary>
  /// <param name="referencingObject"> 
  ///   The business object containing the reference property whose value will be assigned the newly created object. 
  /// </param>
  /// <exception cref="NotSupportedException"> 
  ///   Thrown if this method is called although <see cref="CreateIfNull"/> evaluated <see langword="false"/>. 
  /// </exception>
  /// <remarks>
  ///   A use case for the <b>Create</b> method is the instantiation of an business object without a unique identifier,
  ///   usually an <b>Aggregate</b>. The aggregate reference can be <see langword="null"/> until one of its values
  ///   is set in the user interface.
  /// </remarks>
  IBusinessObject Create (IBusinessObject referencingObject);
}

/// <summary> The <b>IBusinessObjectBooleanProperty</b> interface is used for accessing <see cref="Boolean"/> values. </summary>
public interface IBusinessObjectBooleanProperty: IBusinessObjectProperty
{
  /// <summary> Returns the human readable value of the boolean property. </summary>
  /// <param name="value"> The <see cref="Boolean"/> value to be formatted. </param>
  /// <returns> The human readable string value of the boolean property. </returns>
  /// <remarks> The value of this property may depend on the current culture. </remarks>
  string GetDisplayName (bool value);

  /// <summary> Returns the default value to be assumed if the boolean property returns <see langword="null"/>. </summary>
  /// <remarks> 
  ///   If <see cref="NaBoolean.Null"/> is returned, the object model does not define a default value. In case the 
  ///   caller requires a default value, the selection of the appropriate value is left to the caller.
  /// </remarks>
  NaBoolean GetDefaultValue(IBusinessObjectClass objectClass, IBusinessObject obj);
}

/// <summary> 
///   The <b>IBusinessObjectEnumerationProperty</b> interface is used for accessing the values of an enumeration. 
/// </summary>
/// <remarks> 
///   This property is not restrained to the enumerations derived from the <see cref="Enum"/> type. 
///   <note type="inheritinfo">
///     The native value must be serializable if this property is to be bound to the 
///     <see cref="T:Rubicon.ObjectBinding.Web.Controls.BocEnumValue"/> control.
///   </note>
/// </remarks>
public interface IBusinessObjectEnumerationProperty: IBusinessObjectProperty
{
  /// <summary> Returns a list of all the enumeration's values. </summary>
  /// <returns> 
  ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the values defined in the enumeration. 
  /// </returns>
  IEnumerationValueInfo[] GetAllValues();

  /// <summary> Returns a list of the enumeration's values that can be used in the current context. </summary>
  /// <returns> 
  ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the enabled values in the enumeration. 
  /// </returns>
  /// <remarks> CLS type enums do not inherently support the disabling of its values. </remarks>
  IEnumerationValueInfo[] GetEnabledValues();

  /// <overloads> Returns a specific enumeration value. </overloads>
  /// <summary> Returns a specific enumeration value. </summary>
  /// <param name="value"> The enumeration value to return the <see cref="IEnumerationValueInfo"/> for. </param>
  /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="value"/>. </returns>
  IEnumerationValueInfo GetValueInfoByValue (object value);

  /// <summary> Returns a specific enumeration value. </summary>
  /// <param name="identifier"> 
  ///   The string identifying the  enumeration value to return the <see cref="IEnumerationValueInfo"/> for. 
  /// </param>
  /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="identifier"/>. </returns>
  IEnumerationValueInfo GetValueInfoByIdentifier (string identifier);
}

/// <summary>
///   The <b>IEnumerationValueInfo"</b> interface provides fucntionality for encapsulating a native enumeration value 
///   for use with an <see cref="IBusinessObjectEnumerationProperty"/>.
/// </summary>
/// <remarks> 
///   For enumerations of the <see cref="Enum"/> type, the generic <see cref="EnumerationValueInfo"/> class can be 
///   used.
///  </remarks>
public interface IEnumerationValueInfo
{
  /// <summary> Gets the object representing the original value, e.g. a System.Enum type. </summary>
  /// <value> The encapsulated enumeration value. </value>
  object Value { get; }

  /// <summary> Gets the string identifier representing the value. </summary>
  /// <value> The encapsulated enumeration value's string representation. </value>
  string Identifier { get; }

  /// <summary> Gets the string presented to the user. </summary>
  /// <value> The human readable value of the encapsulated enumeration value. </value>
  /// <remarks> The value of this property may depend on the current culture. </remarks>
  string DisplayName { get; }

  /// <summary>
  ///   Gets a flag indicating whether this value should be presented as an option to the user. 
  ///   (If not, existing objects might still use this value.)
  /// </summary>
  /// <value> <see langword="true"/> if this enumeration value sould be presented as an option to the user. </value>
  bool IsEnabled { get; }
}

/// <summary> Default implementation of the <see cref="IEnumerationValueInfo"/> interface. </summary>
public class EnumerationValueInfo: IEnumerationValueInfo
{
  private object _value;
  private string _identifier;
  private string _displayName;
  private bool _isEnabled; 

  /// <summary> Initializes a new instance of the <b>EnumerationValueInfo</b> type. </summary>
  public EnumerationValueInfo (object value, string identifier, string displayName, bool isEnabled)
  {
    _value = value;
    _identifier = identifier;
    _displayName = displayName;
    _isEnabled = isEnabled;
  }

  /// <summary> Gets the object representing the original value, e.g. a System.Enum type. </summary>
  public object Value
  {
    get { return _value; }
  }

  /// <summary> Gets the string presented to the user. </summary>
  public string Identifier
  {
    get { return _identifier; }
  }

  /// <summary> Gets the string presented to the user. </summary>
  public virtual string DisplayName
  {
    get { return _displayName; }
  }

  /// <summary>
  ///   Gets a flag indicating whether this value should be presented as an option to the user. 
  ///   (If not, existing objects might still use this value.)
  /// </summary>
  public bool IsEnabled
  {
    get { return _isEnabled; }
  }
}

}
