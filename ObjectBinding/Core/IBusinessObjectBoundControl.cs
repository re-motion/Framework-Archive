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

  void LoadValue (bool interim);

  bool SupportsProperty (IBusinessObjectProperty property);

  /// <summary>
  ///   Determines whether the control has a valid configuration.
  /// </summary>
  /// <remarks>
  ///   The configuration is considered invalid if data binding is configured for a property 
  ///   that is not available for the bound class or object.
  /// </remarks>
  bool IsValid { get; }
}

public interface IBusinessObjectBoundModifiableControl: IBusinessObjectBoundControl
{
  void SaveValue (bool interim);
}

}
