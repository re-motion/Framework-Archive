using System;
using System.Web.UI;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   When added to the webform (inside the head element), the <see cref="HtmlHeadContents"/> 
///   control renderes the controls registered with <see cref="HtmlHeadAppender"/>.
/// </summary>
[ToolboxData ("<{0}:HtmlHeadContents runat=\"server\" id=\"HtmlHeadContents\"></rwc:HtmlHeadContents>")]
public class HtmlHeadContents : Control
{
  protected override void Render(HtmlTextWriter writer)
  {
    HtmlHeadAppender.Current.EnsureAppended (Controls);
    //  Don't render tags for this control.
  }

  protected override void RenderChildren(HtmlTextWriter writer)
  {
    base.RenderChildren (writer);
  }
}

}
