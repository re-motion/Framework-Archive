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

  /// <summary>
  /// Gets the interfaces derived from IBusinessObjectProperty that this control supports, or <see langword="null"/> if no restrictions are made.
  /// </summary>
  Type[] SupportedPropertyInterfaces { get; }
}

public interface IBusinessObjectBoundModifiableControl: IBusinessObjectBoundControl
{
  void SaveValue();
}

}
