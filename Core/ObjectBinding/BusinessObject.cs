using System;
using System.Xml.Serialization;

namespace Rubicon.ObjectBinding
{

public abstract class BusinessObject: IBusinessObject
{
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

  [XmlIgnore]
  public abstract IBusinessObjectClass BusinessObjectClass { get; }
}

}
