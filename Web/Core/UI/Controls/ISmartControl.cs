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
  ///   If UseInputControlCSS is true, the control requires special formatting.
  /// </summary>
  /// <remarks>
  ///   This flag should be true for controls rendering &lt;input&gt; or &lt;textarea&gt; elements.
  ///   The reason for this is in excentric application of CSS-classes to these elements via
  ///   the defintion of global styles (input {...} and textarea {...}). The most predictable result
  ///   is acchivied by directly assigning the class instead of using the global definitions.
  /// </remarks>
  //bool UseInputControlCSS { get; }

  /// <summary>
  ///   Gets the label name of the control that should be presented to the user.
  /// </summary>
  string DisplayName { get; }
}

}
