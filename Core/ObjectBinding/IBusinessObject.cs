using System;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObject
{
  object GetProperty (IBusinessObjectProperty property);
  void SetProperty (IBusinessObjectProperty property, object value);
  object this [IBusinessObjectProperty property] { get; set; }
  string GetPropertyString (IBusinessObjectProperty property);
  string GetPropertyString (IBusinessObjectProperty property, string format);

  object GetProperty (string property);
  void SetProperty (string property, object value);
  object this[string property] { get; set; }
  string GetPropertyString (string property);

  IBusinessObjectClass BusinessObjectClass { get; }
}

public interface IBusinessObjectWithIdentity : IBusinessObject
{
  string DisplayName { get; }
  string UniqueIdentifier { get; }
}

}
