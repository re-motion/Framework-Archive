using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UI.Controls
{

[ToolboxItemFilter("System.Web.UI")]
public class SmartLabel: WebControl
{
  private string _forControl = null;

	public SmartLabel()
    : base (HtmlTextWriterTag.Label)
	{
	}

  /// <summary>
  ///   The ID of the control to display a label for.
  /// </summary>
  [TypeConverter (typeof (SmartControlToStringConverter))]
  public string ForControl
  {
    get { return _forControl; }
    set { _forControl = value; }
  }

  protected override void Render(HtmlTextWriter writer)
  {
    this.RenderBeginTag (writer);

    ISmartControl smartControl = NamingContainer.FindControl (ForControl) as ISmartControl;
    if (smartControl != null && smartControl.DisplayName != null)
      writer.Write (smartControl.DisplayName);
    else
      writer.Write ("[Label for " + ForControl + "]");

    this.RenderEndTag (writer);
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);

    Control target = NamingContainer.FindControl (ForControl);
    ISmartControl smartControl = target as ISmartControl;
    bool useLabel;
    if (smartControl != null)
    {
      target = smartControl.TargetControl;
      useLabel = smartControl.UseLabel;
    }
    else
    {
      useLabel = ! (target is DropDownList || target is HtmlSelect);
    }

    if (useLabel && target != null)
      writer.AddAttribute (HtmlTextWriterAttribute.For, target.ClientID);

    // TODO: add <a href="ToName(target.ClientID)"> ...
    // ToName: '.' -> '_'

    //  Accesskey support
  }
}

}
