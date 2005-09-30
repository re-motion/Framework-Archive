using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A <c>LinkButton</c> using <c>&amp;</c> as access key prefix in <see cref="LinkButton.Text"/>. </summary>
/// <include file='doc\include\UI\Controls\WebLinkButton.xml' path='WebLinkButton/Class/*' />
[ToolboxData("<{0}:WebLinkButton runat=server></{0}:WebLinkButton>")]
public class WebLinkButton : LinkButton
{
  private string _text;

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string accessKey;
    _text = StringUtility.NullToEmpty (Text);
    _text = SmartLabel.FormatLabelText (_text, false, out accessKey);

    if (StringUtility.IsNullOrEmpty (AccessKey))
      writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, accessKey);

    base.AddAttributesToRender (writer);
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    if (WcagUtility.IsWcagDebuggingEnabled() && WcagUtility.IsWaiConformanceLevelARequired())
      WcagUtility.HandleError (1, this);

    if (HasControls())
      base.RenderContents (writer);
    else
      writer.Write (_text);
  }
}

}
