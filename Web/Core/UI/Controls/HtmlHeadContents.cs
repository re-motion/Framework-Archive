using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   When added to the webform (inside the head element), the <see cref="HtmlHeadContents"/> 
///   control renderes the controls registered with <see cref="HtmlHeadAppender"/>.
/// </summary>
[ToolboxData ("<{0}:HtmlHeadContents runat=\"server\" id=\"HtmlHeadContents\"></{0}:HtmlHeadContents>")]
public class HtmlHeadContents : Control
{
  protected override void Render(HtmlTextWriter writer)
  {
    HtmlHeadAppender.Current.EnsureAppended (Controls);
    //  Don't render tags for this control.
    RenderChildren (writer);
  }

  protected override void RenderChildren(HtmlTextWriter writer)
  {
    foreach (Control control in Controls)
    {
      HtmlGenericControl genericControl = control as HtmlGenericControl;
      if (genericControl != null)
      {
        //  <link ...> has no closing tags.
        if (string.Compare (genericControl.TagName, "link", true) == 0)
        {
          writer.WriteBeginTag ("link");
          foreach (string attributeKey in genericControl.Attributes.Keys)
            writer.WriteAttribute (attributeKey, genericControl.Attributes[attributeKey]);
          writer.WriteLineNoTabs (">");
        }
        else
        {
          control.RenderControl(writer);
          writer.WriteLine();
        }
      }
      else
      {
        control.RenderControl(writer);
        writer.WriteLine();
     }

    }
  }

}

}
