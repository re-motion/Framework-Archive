using System;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObject
{
  object GetProperty (IBusinessObjectProperty property);
  void SetProperty (IBusinessObjectProperty property, object value);
  object this [IBusinessObjectProperty property] { get; set; }

  object GetProperty (string property);
  void SetProperty (string property, object value);
  object this[string property] { get; set; }

  IBusinessObjectClass BusinessObjectClass { get; }
}

public abstract class BusinessObject: IBusinessObject
{
  public virtual IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetProperty (propertyIdentifier);
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

  public abstract IBusinessObjectClass BusinessObjectClass { get; }
}

}
