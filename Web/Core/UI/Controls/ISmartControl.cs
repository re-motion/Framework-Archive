using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

namespace Rubicon.Web.UI.Controls
{

public interface ISmartControl: IComponent
{
  bool IsRequired { get; }
  string HelpUrl { get; }

  /// <summary>
  ///   Creates an appropriate validator for this control.
  /// </summary>
  BaseValidator[] CreateValidators(); 

  string TargetControlID { get; }
  string DisplayName { get; }
}

}
