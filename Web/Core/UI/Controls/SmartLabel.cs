using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web.Controls
{

[ToolboxItemFilter("System.Web.UI")]
public class BocPropertyLabel: WebControl
{
  private string _forControl = null;

	public BocPropertyLabel()
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

    IBusinessObjectBoundControl objectBoundControl = NamingContainer.FindControl (ForControl) as IBusinessObjectBoundControl;
    if (objectBoundControl != null && objectBoundControl.Property != null)
      writer.Write (objectBoundControl.Property.DisplayName);
    else
      writer.Write ("[Label for " + ForControl + "]");

    this.RenderEndTag (writer);
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);

    Control target = NamingContainer.FindControl (ForControl);
    IGetTargetControl getTargetControl = target as IGetTargetControl;
    if (getTargetControl != null)
      target = getTargetControl.GetTargetControl();
    if (target != null)
      writer.AddAttribute (HtmlTextWriterAttribute.For, target.ClientID);
  }
}

}
