using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

[DesignTimeVisible(false)]
public class WxeForm: HtmlForm
{
  public static WxeForm Replace (HtmlForm htmlForm)
  {
    WxeForm newForm = new WxeForm();

    if (! StringUtility.IsNullOrEmpty (htmlForm.Method))
      newForm.Method = htmlForm.Method;
    if (! StringUtility.IsNullOrEmpty (htmlForm.Enctype))
      newForm.Enctype = htmlForm.Enctype;
    if (! StringUtility.IsNullOrEmpty (htmlForm.Target))
      newForm.Target = htmlForm.Target;

    while (htmlForm.Controls.Count > 0)
      newForm.Controls.Add (htmlForm.Controls[0]);

    Control parent = htmlForm.Parent;
    if (parent != null)
    {
      parent.Controls.Remove (htmlForm);
      parent.Controls.Add (newForm);
    }

    newForm.ID = htmlForm.ID;
    newForm.Name = htmlForm.Name;
    return newForm;
  }

  protected override void RenderAttributes (HtmlTextWriter writer)
  {
    writer.WriteAttribute ("action", "WxeHandler.ashx" + Context.Request.Url.Query + Context.Request.Url.Fragment);
    Attributes.Remove ("action");

    // from HtmlForm
    writer.WriteAttribute("name", this.Name);
    base.Attributes.Remove("name");
    writer.WriteAttribute("method", this.Method);
    base.Attributes.Remove("method");
    //  writer.WriteAttribute("action", this.GetActionAttribute(), true);
    //  base.Attributes.Remove("action");
//    string text1 = this.Page.ClientOnSubmitEvent;
//    if ((text1 != null) && (text1.Length > 0))
//    {
//      if (base.Attributes["onsubmit"] != null)
//      {
//        text1 = text1 + base.Attributes["onsubmit"];
//        base.Attributes.Remove("onsubmit");
//      }
//      writer.WriteAttribute("language", "javascript");
//      writer.WriteAttribute("onsubmit", text1);
//    }
    if (this.ID == null)
      writer.WriteAttribute("id", this.ClientID);

    // from HtmlContainerControl
    this.ViewState.Remove("innerhtml");

    // from HtmlControl
    if (this.ID != null)
    {
          writer.WriteAttribute("id", this.ClientID);
    }
    this.Attributes.Render(writer);
  }

}

}
