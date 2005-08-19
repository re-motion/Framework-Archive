using System;
using System.IO;
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
  public static readonly string ReturningTokenID = "wxeReturningToken";

  HtmlInputHidden _returningTokenField;

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
      int htmlFormIndex = parent.Controls.IndexOf (htmlForm);
      if (htmlFormIndex >= 0)
      {
        parent.Controls.RemoveAt (htmlFormIndex);
        parent.Controls.AddAt (htmlFormIndex, newForm);
      }
      else
      {
        parent.Controls.Add (newForm);
      }
    }

    newForm.ID = htmlForm.ID;
    newForm.Name = htmlForm.Name;
    return newForm;
  }

  protected override void RenderAttributes (HtmlTextWriter writer)
  {
    if (WxeContext.Current != null)
    {
      string action = WxeContext.Current.GetResumePath (false);
      writer.WriteAttribute ("action", action);
      Attributes.Remove ("action");
    }

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
      writer.WriteAttribute("id", this.ClientID);

    this.Attributes.Render(writer);
  }

  protected override void Render(HtmlTextWriter writer)
  {
    if (WxeHandler.IsSessionManagementEnabled && ! Rubicon.Web.UI.HtmlHeadAppender.Current.HasAppended)
      throw new ApplicationException ("The Rubicon.Web.UI.Controls.HtmlHeadContents element is missing on the page.");
    base.Render (writer);
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    _returningTokenField = new HtmlInputHidden ();
    _returningTokenField.ID = WxeForm.ReturningTokenID;
    this.Controls.Add (_returningTokenField);  
  }

  /// <summary> The token of the <see cref="WxeFunction"/> to return to. </summary>
  public String ReturningToken
  {
    get { return _returningTokenField.Value; }
    set { _returningTokenField.Value = value; }
  }
}

}
