using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

[ToolboxData("<{0}:WebButton runat=server></{0}:WebButton>")]
public class WebButton : Button
{
  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string accessKey;
    string text = StringUtility.NullToEmpty (Text);
    text = SmartLabel.FormatLabelText (text, false, out accessKey);

    if (StringUtility.IsNullOrEmpty (AccessKey))
      writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, accessKey);
    writer.AddAttribute (HtmlTextWriterAttribute.Value, text);

    base.AddAttributesToRender (writer);
  }
}

}
