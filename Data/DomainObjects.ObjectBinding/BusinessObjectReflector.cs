using System;
using System.Reflection;
using System.Collections;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class BusinessObjectReflector
{
  private IBusinessObject _bindableObject;

	public BusinessObjectReflector (IBusinessObject bindableObject)
	{
    ArgumentUtility.CheckNotNull ("bindableObject", bindableObject);
    _bindableObject = bindableObject;
	}

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
    else if (property.GetType () == typeof(EnumerationProperty))
    {
      object internalValue = _bindableObject.GetProperty (property);

      value = ((EnumerationProperty)property).GetValueInfoByValue (internalValue).DisplayName;
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
      strValue = businessObject.DisplayName;
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

  public object GetProperty (IBusinessObjectProperty property)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (BaseProperty));
    BaseProperty reflectionProperty = (BaseProperty) property;
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = propertyInfo.GetValue (_bindableObject, new object[0]);
    return reflectionProperty.FromInternalType (internalValue);
  }

  public void SetProperty (IBusinessObjectProperty property, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (BaseProperty));
    BaseProperty reflectionProperty = (BaseProperty) property;
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = reflectionProperty.ToInternalType (value);
    propertyInfo.SetValue (_bindableObject, internalValue, new object[0]);
  }
}
}
