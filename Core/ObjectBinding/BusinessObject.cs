using System;
using System.Xml.Serialization;

namespace Rubicon.ObjectBinding
{

public abstract class BusinessObject: IBusinessObject
{
  public static string GetPropertyString (IBusinessObject obj, IBusinessObjectProperty property, string format)
  {
    object value = obj.GetProperty (property);

    if (value == null)
      return string.Empty;

    IBusinessObjectWithIdentity businessObject = value as IBusinessObjectWithIdentity;
    if (businessObject != null)
      return businessObject.DisplayName;

    if (format != null)
    {
      IFormattable formattable = value as IFormattable;
      if (formattable != null)
        return formattable.ToString (format, null);
    }

    return value.ToString();
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
