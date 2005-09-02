using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI;

namespace Rubicon.Web.ExecutionEngine
{

[DesignTimeVisible(false)]
public class WxeForm: HtmlForm, IPostBackDataHandler
{
  private static readonly object s_loadPostDataEvent = new object();

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

  /// <summary> Calls the <see cref="OnLoadPostData"/> method. </summary>
  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    return OnLoadPostData (postDataKey, postCollection);
  }

  /// <summary> Calls the <see cref="RaisePostDataChangedEvent"/> method. </summary>
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    RaisePostDataChangedEvent();
  }

  /// <summary> Fires the <see cref="LoadPostData"/> event. </summary>
  protected virtual bool OnLoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    EventHandler eventHandler = (EventHandler) Events[s_loadPostDataEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
    return false;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
  }

  /// <summary> Occurs during the load post data phase. </summary>
  [Browsable (false)]
  public event EventHandler LoadPostData
  {
    add { Events.AddHandler (s_loadPostDataEvent, value); }
    remove { Events.RemoveHandler (s_loadPostDataEvent, value); }
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    Page.RegisterRequiresPostBack (this);
  }

  protected override void RenderAttributes (HtmlTextWriter writer)
  {
    WxeContext wxeContext = WxeContext.Current;
    if (wxeContext != null)
    {
      string action = wxeContext.GetPath (false);
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
    //  string text1 = this.Page.ClientOnSubmitEvent;
    //  if ((text1 != null) && (text1.Length > 0))
    //  {
    //    if (base.Attributes["onsubmit"] != null)
    //    {
    //      text1 = text1 + base.Attributes["onsubmit"];
    //      base.Attributes.Remove("onsubmit");
    //    }
    //    writer.WriteAttribute("language", "javascript");
    //    writer.WriteAttribute("onsubmit", text1);
    //  }
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
    if (! Rubicon.Web.UI.HtmlHeadAppender.Current.HasAppended)
    {
      ISmartNavigablePage smartNavigablePage = Page as ISmartNavigablePage;
      if (   WxeHandler.IsSessionManagementEnabled
          || (   smartNavigablePage != null
              && (   smartNavigablePage.IsSmartScrollingEnabled 
                  || smartNavigablePage.IsSmartFocusingEnabled)))
      {
        throw new ApplicationException ("The Rubicon.Web.UI.Controls.HtmlHeadContents element is missing on the page.");
      }
    }
    base.Render (writer);
  }
}

}
