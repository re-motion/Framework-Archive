using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   This interfaces declares advanced properties and methods for data-aware controls.
///   <seealso cref="FormGridManager"/>
/// </summary>
public interface ISmartControl: IControl
{
  /// <summary>
  ///   Specifies whether the control must be filled out by the user before submitting the form.
  /// </summary>
  bool IsRequired { get; }

  /// <summary>
  ///   Specifies the relative URL to the row's help text.
  /// </summary>
  string HelpUrl { get; }

  /// <summary>
  ///   Creates an appropriate validator for this control.
  /// </summary>
  BaseValidator[] CreateValidators(); 

  /// <summary>
  ///   Gets the ID of the (sub-)control that can be referenced by HTML &lt;label for=...&gt;.
  /// </summary>
  /// <remarks>
  ///   For simple controls, this is the ID of the control itself. For compound controls, this is the ID of the form element that
  ///   should receive the focus when the label is selected.
  /// </remarks>
  string TargetControlClientID { get; }

  /// <summary>
  ///   If UseLabel is true, it is valid to generate HTML &lt;label&gt; tags referencing <see cref="TargetControlClientID"/>.
  /// </summary>
  /// <remarks>
  ///   This flag is usually true, except for controls that render combo boxes or other HTML tags that do not function properly
  ///   with labels. This flag has been introduced due to a bug in Microsoft Internet Explorer.
  /// </remarks>
  bool UseLabel { get; }

  /// <summary>
  ///   Gets the label name of the control that should be presented to the user.
  /// </summary>
  string DisplayName { get; }
}

}
