using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A <c>Button</c> using <c>&</c> as access key prefix in <see cref="Button.Text"/>. </summary>
[ToolboxData("<{0}:WebButton runat=server></{0}:WebButton>")]
public class WebButton : Button
{
  IconInfo _icon;

  public WebButton()
  {
    _icon = new IconInfo();
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
    base.AddAttributesToRender (writer);
    Text = tempText;
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
  [Description ("The icon displayed in this tree node.")]
  [NotifyParentProperty (true)]
  public IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }
}

}
