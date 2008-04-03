using System;
using System.Collections;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// A class providing reflection based access to property values of an object.
/// </summary>
public class BusinessObjectReflector
{
  private IBusinessObject _bindableObject;

  /// <summary>
  /// Instantiates a new object.
  /// </summary>
  /// <param name="bindableObject">The object to create the properties for. Must not be <see langword="null"/></param>
  /// <exception cref="System.ArgumentNullException"><paramref name="bindableObject"/> is <see langword="null"/>.</exception>
  public BusinessObjectReflector (IBusinessObject bindableObject)
	{
    ArgumentUtility.CheckNotNull ("bindableObject", bindableObject);

    _bindableObject = bindableObject;
	}

  /// <summary>
  /// Returns the string representation of the property value.
  /// </summary>
  /// <param name="property">The property to return.</param>
  /// <param name="format">A formatter for the string.</param>
  /// <returns>A string representing the property value.</returns>
  // TODO: missing ArgumentUtility?
  // TODO Doc: Code koennte intentionen besser ausdrücken.
  public string GetPropertyString (IBusinessObjectProperty property, string format)
  {
    object value;
    int count;

    if (property.IsList)
    {
      IList list = (IList) _bindableObject.GetProperty (property);
      count = list.Count;
      if (count > 0)
        value = list[0];
      else 
        value = null;
    }
    else if (property.GetType () == typeof (EnumerationProperty))
    {
      object internalValue = _bindableObject.GetProperty (property);

      value = ((EnumerationProperty)property).GetValueInfoByValue (internalValue, _bindableObject).DisplayName;
      count = 1;
    }
    else
    {
      value = _bindableObject.GetProperty (property);
      count = 1;
    }

    if (value == null)
      return string.Empty;

    string strValue = null;
    IBusinessObjectWithIdentity businessObject = value as IBusinessObjectWithIdentity;
    if (businessObject != null)
    {
      strValue = businessObject.DisplayNameSafe;
    }
    else if (property is IBusinessObjectBooleanProperty)
    {
      if (value is bool)
        strValue = ((IBusinessObjectBooleanProperty)property).GetDisplayName ((bool)value);
      else
        strValue = string.Empty;
    }
    else if (format != null)
    {
      IFormattable formattable = value as IFormattable;
      if (formattable != null)
        strValue = formattable.ToString (format, null);
    }

    if (strValue == null)
      strValue = value.ToString();

    if (count > 1)
      strValue += " ... [" + count.ToString() + "]";

    return strValue;
  }

  /// <summary>
  /// Returns the value of a given property.
  /// </summary>
  /// <param name="property">The property to return. Must be of type <see cref="BaseProperty"/> or derived from it and must not be <see langword="null"/>.</param>
  /// <returns>The value of the property.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is a null reference.</exception>
  /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="property"/> is not of type <see cref="BaseProperty"/> or derived from it.</exception>
  public object GetProperty (IBusinessObjectProperty property)
  {
    ArgumentUtility.CheckNotNullAndType<BaseProperty> ("property", property);
    BaseProperty reflectionProperty = (BaseProperty) property;
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = propertyInfo.GetValue (_bindableObject, new object[0]);
    return reflectionProperty.FromInternalType (_bindableObject, internalValue);
  }

  /// <summary>
  /// Sets the value of a given property.
  /// </summary>
  /// <param name="property">The property to set.  Must be of type <see cref="BaseProperty"/> or derived from it and must not be <see langword="null"/>.</param>
  /// <param name="value">The value that should be assigned to the property.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="property"/> is a null reference.</exception>
  /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="property"/> is not of type <see cref="BaseProperty"/> or derived from it.</exception>
  public void SetProperty (IBusinessObjectProperty property, object value)
  {
    ArgumentUtility.CheckNotNullAndType <BaseProperty> ("property", property);
    
    BaseProperty reflectionProperty = (BaseProperty) property;
    if (reflectionProperty is ReferenceProperty 
        && reflectionProperty.IsList 
        && reflectionProperty.BusinessObjectClass is DomainObjectClass
        && ((DomainObjectClass)reflectionProperty.BusinessObjectClass).ClassDefinition is ReflectionBasedClassDefinition)
    {
      return;
    }

    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = reflectionProperty.ToInternalType (value);
    propertyInfo.SetValue (_bindableObject, internalValue, new object[0]);
  }

  /// <summary>
  ///   Gets the value of <see cref="IBusinessObject.DisplayName"/> if it is accessible and otherwise falls back to the <see cref="string"/> returned by
  ///   <see cref="IBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder"/>.
  /// </summary>
  public string DisplayNameSafe
  {
    get
    {
      IBusinessObjectClass businessObjectClass = _bindableObject.BusinessObjectClass;
      IBusinessObjectProperty displayNameProperty = businessObjectClass.GetPropertyDefinition ("DisplayName");
      if (displayNameProperty.IsAccessible (businessObjectClass, _bindableObject))
        return _bindableObject.DisplayName;

      return businessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ();
    }
  }

}
}
