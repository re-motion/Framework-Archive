using System;
using System.ComponentModel;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectBoundControl: IComponent
{
  IBusinessObjectDataSource DataSource { get; set; }
  string PropertyIdentifier { get; set; }
  IBusinessObjectProperty Property { get; set; }
  object Value { get; set; }

  void LoadValue();

  bool SupportsProperty (IBusinessObjectProperty property);
}

public interface IBusinessObjectBoundModifiableControl: IBusinessObjectBoundControl
{
  void SaveValue();
}

}
