using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.ObjectBinding;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{

[ToolboxItemFilter("System.Web.UI")]
public class SmartLabel: WebControl
{
  private string _forControl = null;

	public SmartLabel()
    : base (HtmlTextWriterTag.Label)
	{
	}

  [TypeConverter (typeof (BusinessObjectBoundControlToStringConverter))]
  public string ForControl
  {
    get { return _forControl; }
    set { _forControl = value; }
  }

  protected override void Render(HtmlTextWriter writer)
  {
    this.RenderBeginTag (writer);

    ISmartControl smartControl = NamingContainer.FindControl (ForControl) as ISmartControl;
    if (smartControl != null)
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
    if (smartControl != null)
      target = smartControl.TargetControl;

    if (target != null)
      writer.AddAttribute (HtmlTextWriterAttribute.For, target.ClientID);
  }
}

}
