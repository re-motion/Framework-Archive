using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   Can be used instead of <see cref="SmartLabel"/> controls 
///   (to label controls that do not implement ISmartControl).
/// </summary>
[ToolboxItemFilter("System.Web.UI")]
public class FormGridLabel: Label, ISmartControl
{
  private bool _required = false;
  private string _helpUrl = null;

  [Category("Behavior")]
  [DefaultValue (false)]
  [Description ("Specifies whether this row will be marked as 'required' in FormGrids.")]
  public bool Required
  {
    get { return _required; }
    set { _required = value; }
  }

  [Category("Behavior")]
  [DefaultValue (null)]
  [Description ("Specifies the relative URL to the row's help text.")]
  public string HelpUrl
  {
    get { return _helpUrl; }
    set { _helpUrl = value; }
  }

  [Browsable (false)]
  public bool IsRequired
  {
    get { return _required; }
  }

  BaseValidator[] ISmartControl.CreateValidators()
  {
    return null;
  }

  Control ISmartControl.TargetControl
  {
    get { return this; }
  }

  bool ISmartControl.UseLabel
  {
    get { return true; }
  }

  string ISmartControl.DisplayName
  {
    get { return base.Text; }
  }
}

}
