using System;
using System.Collections;
using System.Xml.Serialization;

namespace Rubicon.ObjectBinding
{

public abstract class BusinessObject: IBusinessObject
{
  public static string GetPropertyString (IBusinessObject obj, IBusinessObjectProperty property, string format)
  {
    object value;
    int count;

    if (property.IsList)
    {
      IList list = (IList) obj.GetProperty (property);
      count = list.Count;
      if (count > 0)
        value = list[0];
      else 
        value = null;
    }
    else
    {
      value = obj.GetProperty (property);
      count = 1;
    }

    if (value == null)
      return string.Empty;

    string strValue;
    IBusinessObjectWithIdentity businessObject = value as IBusinessObjectWithIdentity;
    if (businessObject != null)
    {
      strValue = businessObject.DisplayName;
    }
    else if (property is IBusinessObjectEnumerationProperty)
    {
      IBusinessObjectEnumerationProperty enumProperty = (IBusinessObjectEnumerationProperty) property;
      IEnumerationValueInfo enumValueInfo = enumProperty.GetValueInfoByValue (value);
      strValue = enumValueInfo.DisplayName;
    }
    else if (format != null && value is IFormattable)
    {
      strValue = ((IFormattable) value).ToString (format, null);
    }
    else
    {
      strValue = value.ToString();
    }

    if (count > 1)
      strValue += " ... [" + count.ToString() + "]";
    return strValue;
  }

  public virtual IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }

  public abstract object GetProperty (IBusinessObjectProperty property);
  public abstract void SetProperty (IBusinessObjectProperty property, object value);

  public object this [IBusinessObjectProperty property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  public string GetPropertyString (IBusinessObjectProperty property)
  {
    return GetPropertyString (property, null);
  }

  public virtual string GetPropertyString (IBusinessObjectProperty property, string format)
  {
    return GetPropertyString (this, property, format);
  }
  
  public object GetProperty (string property)
  {
    return GetProperty (GetBusinessObjectProperty (property));
  }

  public void SetProperty (string property, object value)
  {
    SetProperty (GetBusinessObjectProperty (property), value);
  }

  public object this [string property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  public string GetPropertyString (string property)
  {
    return GetPropertyString (GetBusinessObjectProperty (property));
  }

  [XmlIgnore]
  public abstract IBusinessObjectClass BusinessObjectClass { get; }
}

}
