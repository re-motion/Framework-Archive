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

}
