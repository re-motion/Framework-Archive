using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

#if ! net20 
/// <summary> A hidden value control that can raise a data changed event when its contents has been modified. </summary>
/// <remarks> .net 2.0 will provide such this control in its class library. </remarks>
public class HiddenField: WebControl, IPostBackDataHandler
{
  private static readonly object s_eventValueChanged = new object();
  string _value;

  protected override ControlCollection CreateControlCollection()
  {
    return new EmptyControlCollection (this);
  }

  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    return this.LoadPostData (postDataKey, postCollection);
  }

  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    this.RaisePostDataChangedEvent();
  }

  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    string oldValue = Value;
    string newValue = postCollection[postDataKey];
    if (oldValue != newValue)
    {
      Value = newValue;
      return true;
    }
    return false;
  }

  protected virtual void RaisePostDataChangedEvent()
  {
    OnValueChanged (EventArgs.Empty);
  }
 
  protected virtual void OnValueChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_eventValueChanged];
    if (eventHandler != null)
      eventHandler (this, e);
  }
 
  [Category ("Action")]
  [Description ("Fires when the value of the control changes.")]
  public event EventHandler ValueChanged
  {
    add { Events.AddHandler (s_eventValueChanged, value); }
    remove { Events.RemoveHandler (s_eventValueChanged, value); }
  }

  [DefaultValue("")]
  [Category("Behavior")]
  public virtual string Value
  {
    get { return StringUtility.NullToEmpty (_value); }
    set { _value = value; }
  } 

  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    _value = (string) values[1];
  }

  protected override object SaveViewState()
  {
    object[] values = new object[2];

    values[0] = base.SaveViewState();
    values[1] = _value;

    return values;
  }

  protected override void Render (HtmlTextWriter writer)
  {
    if (Page != null)
      Page.VerifyRenderingInServerForm (this);
    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
    string name = UniqueID;
    if (name != null)
      writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
    if (ID != null)
      writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
    string value = Value;
    if (value.Length > 0)
      writer.AddAttribute(HtmlTextWriterAttribute.Value, value);
    writer.RenderBeginTag(HtmlTextWriterTag.Input);
    writer.RenderEndTag();
  }
}
#endif

}
