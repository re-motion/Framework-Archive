using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A <c>Button</c> using <c>&</c> as access key prefix in <see cref="Button.Text"/>. </summary>
//TODO: .net2.0 complier switch. .net2.0 has more elegant way of doing buttons, making .net1.1 a workaround
[ToolboxData("<{0}:WebButton runat=server></{0}:WebButton>")]
public class WebButton : 
  Button, 
  // Required because Page.ProcessPostData always registers the last IPostBackEventHandler in the controls collection
  // for controls (buttons) having PostData but no IPostBackDataHandler. 
  // .net 2.0 resolves this issue for controls using a javascript induced postback event.
  IPostBackDataHandler
{
  IconInfo _icon;

  public WebButton()
  {
    _icon = new IconInfo();
    CssClass = "Button";
  }

  public void RaisePostDataChangedEvent()
  {
  }

  public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
  {
    return false;
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string accessKey;
    string text = StringUtility.NullToEmpty (Text);
    text = SmartLabel.FormatLabelText (text, false, out accessKey);

    if (StringUtility.IsNullOrEmpty (AccessKey))
      writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, accessKey);
    string tempText = Text;
    Text = text;
    //TODO: .net 2.0 compiler switch
    AddAttributesToRender_net11 (writer);
    //AddAttributesToRender_net20 (writer);
    Text = tempText;
  }

  /// <summary> Method to be executed when compiled for .net 1.1. Compiler switch not yet implented. </summary>
  private void AddAttributesToRender_net11 (HtmlTextWriter writer)
  {
    if (Page != null)
      Page.VerifyRenderingInServerForm(this);

    if (Page != null)
    {
      string onClick = string.Empty;
      if (CausesValidation && Page.Validators.Count > 0)
      {
        const string clientValidation = "if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate(); ";
        onClick += clientValidation;
        if (Attributes.Count > 0)
        {
          string baseOnClick = Attributes["onclick"];
          if (baseOnClick != null)
          {
            onClick += baseOnClick;
            Attributes.Remove("onclick");
          }
        }
      }
      onClick += Page.GetPostBackEventReference (this) + "; ";
      onClick += "return false;";
      writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
      writer.AddAttribute("language", "javascript");
    }
    bool causesValidationTemp = CausesValidation;
    CausesValidation = false;
    base.AddAttributesToRender (writer);
    CausesValidation = causesValidationTemp;
  }

  /// <summary> Method to be executed when compiled for .net 2.0. Compiler switch not yet implented. </summary>
  private void AddAttributesToRender_net20 (HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Button; }
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    string text = StringUtility.NullToEmpty (Text);
    text = SmartLabel.FormatLabelText (text, true);

    if (HasControls())
    {
      base.RenderContents (writer);
    }
    else
    {
      bool hasIcon = _icon != null && ! StringUtility.IsNullOrEmpty (_icon.Url);
      bool hasText = ! StringUtility.IsNullOrEmpty (text);
      if (hasIcon)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Src, _icon.Url);
        if (! _icon.Height.IsEmpty)
          writer.AddAttribute (HtmlTextWriterAttribute.Height, _icon.Height.ToString());
        if (! _icon.Width.IsEmpty)
          writer.AddAttribute (HtmlTextWriterAttribute.Width, _icon.Width.ToString());
        writer.AddStyleAttribute ("vertical-align", "middle");
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag();
      }
      if (hasIcon && hasText)
        writer.Write ("&nbsp;");
      if (hasText)
        writer.Write (text);
    }
  }

  /// <summary> Gets or sets the icon displayed in this menu item. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The icon displayed.")]
  [NotifyParentProperty (true)]
  public IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }
}

}
