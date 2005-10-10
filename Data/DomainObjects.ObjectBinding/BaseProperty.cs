using System;
using System.Reflection;
using System.Collections;

using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// The default implementation of <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> for <see cref="BindableDomainObject"/>s.
/// </summary>
/// <remarks>See the documentation of <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> for additional documentation for each member.</remarks>
public class BaseProperty : IBusinessObjectProperty
{
  private PropertyInfo _propertyInfo;
  private bool _isRequired;
  private Type _itemType;
  private bool _isList;

  //TODO: should it be internal protected?
  //TODO: missing ArgumentUtility?!
  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="propertyInfo">The <see cref="System.Reflection.PropertyInfo"/> object of the property.</param>
  /// <param name="isRequired">A value indicating if the property is required by the business model, or not.</param>
  /// <param name="itemType">The type of the property value.</param>
  /// <param name="isList">A value indicating if the property contains multiple objects.</param>
  internal BaseProperty (PropertyInfo propertyInfo, bool isRequired, Type itemType, bool isList)
  {
    _propertyInfo = propertyInfo;
    _isRequired = isRequired;
    _itemType = itemType;
    _isList = isList;
  }

  /// <summary> Gets a flag indicating whether this property contains multiple values. </summary>
  /// <value><see langword="true"/> if this property contains multiple values; otherwise <see langword="false"/>. </value>
  public bool IsList
  {
    get { return _isList; }
  }

  /// <summary> Creates a list. </summary>
  /// <returns> A new array with the specified number of empty elements. </returns>
  public IList CreateList (int count)
  {
    if (!IsList)
      throw new InvalidOperationException ("Cannot create lists for non-list properties.");

    return Array.CreateInstance (_itemType, count);
  }

  /// <summary> Gets the type of a single value item. </summary>
  /// <remarks>If <see cref="IsList"/> is <see langword="false"/>, the item type is the same as <see cref="PropertyType"/>. 
  ///   Otherwise, the item type is the type of a list item.
  /// </remarks>
  public Type ItemType
  {
    get { return _itemType; }
  }

  /// <summary> Gets the type of the property. </summary>
  /// <remarks> 
  ///   This is the type of elements returned by the <see cref="BindableDomainObject.GetProperty"/> method
  ///   and set via the <see cref="BindableDomainObject.SetProperty"/> method.
  /// </remarks>
  public virtual Type PropertyType
  {
    get { return _propertyInfo.PropertyType;  }
  }

  /// <summary> Gets an identifier that uniquely defines this property within its class. </summary>
  /// <value>The name of the property.</value>
  public string Identifier
  {
    get { return _propertyInfo.Name; }
  }

  /// <summary> Gets the property name as presented to the user. </summary>
  /// <value>The human readable identifier of this property in the current culture.</value>
  public string DisplayName
  {
    get 
    { 
      string displayName = string.Empty;

      if (MultiLingualResourcesAttribute.ExistsResource (_propertyInfo.DeclaringType))
        displayName = MultiLingualResourcesAttribute.GetResourceText (_propertyInfo.DeclaringType, "property:" + _propertyInfo.Name);

      if (displayName == string.Empty || displayName == null)
        displayName = _propertyInfo.Name;

      return displayName; 
    }
  }

  /// <summary> Gets a flag indicating whether this property is required. </summary>
  /// <value><see langword="true"/> if this property is required; otherwise <see langword="false"/>. </value>
  public virtual bool IsRequired
  {
    get { return _isRequired;  }
  }
  
  /// <summary> Indicates whether this property can be accessed by the user. </summary>
  /// <param name="objectClass"> 
  ///   The <see cref="IBusinessObjectClass"/> of the <paramref name="obj"/>. 
  ///   This parameter is not used in the current implementation.
  /// </param>
  /// <param name="obj">
  ///   The object to evaluate this property for, or <see langword="null"/>.
  ///   This parameter is not used in the current implementation.
  /// </param>
  /// <returns><see langword="true"/></returns>
  public bool IsAccessible (IBusinessObjectClass objectClass, IBusinessObject obj)
  {
    return true;
  }

  /// <summary> Indicates whether this property can be accessed by the user. </summary>
  /// <param name="obj">
  ///   The object to evaluate this property for, or <see langword="null"/>.
  ///   This parameter is not used in the current implementation.
  /// </param>
  /// <returns><see langword="true"/></returns>
  public bool IsAccessible (IBusinessObject obj)
  {
    return true;
  }

  /// <summary>Indicates whether this property can be modified by the user.</summary>
  /// <param name="obj">This parameter is not used in the current implementation.</param>
  /// <returns><see langword="true"/> if the user can set this property; otherwise <see langword="false"/>.</returns>
  public bool IsReadOnly (IBusinessObject obj)
  {
    return !_propertyInfo.CanWrite;
  }

  /// <summary>
  /// Gets the <see cref="System.Reflection.PropertyInfo"/> object representing the property.
  /// </summary>
  public PropertyInfo PropertyInfo
  {
    get { return _propertyInfo; }
  }

  /// <summary>
  /// Converts the value from internal representation (business layer) to external representation (representation layer).
  /// </summary>
  /// <param name="internalValue">The internal value.</param>
  /// <returns>The external value. If the data type of the property has a field named "MinValue" and <paramref name="internalValue"/> is equal to it, then the return value is null.</returns>
  /// <note type="inheritinfo">
  ///   Override this method to convert data types, take care of default values or other changes.
  /// </note>
  internal protected virtual object FromInternalType (object internalValue)
  {
    if (!IsList)
    {
      FieldInfo minValueFieldInfo = _itemType.GetField ("MinValue");

      if (minValueFieldInfo != null)
      {
        if (minValueFieldInfo.GetValue (_itemType).ToString () == internalValue.ToString ())
          return null;
      }
    }

    return internalValue;
  }

  /// <summary>
  /// Converts the value from external representation (representation layer) to internal representation.
  /// </summary>
  /// <param name="publicValue">The external value.</param>
  /// <returns>The internal value.</returns>
  /// <exception cref="InvalidNullAssignmentException"><paramref name="publicValue"/> is <see langword="null"/> and this value is not supported by this property.</exception>
  internal protected virtual object ToInternalType (object publicValue)
  {
    if (_itemType.IsValueType && publicValue == null)
      throw new InvalidNullAssignmentException (_itemType);

    return publicValue;
  }

  /// <summary>
  /// Gets an IBusinessObjectProvider for the domain model.
  /// </summary>
  public IBusinessObjectProvider BusinessObjectProvider
  {
    get { return DomainObjectProvider.Instance; }
  }
}
}
