using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI;

namespace OBRTest
{
public class PersonCustomCell: BocCustomColumnDefinitionCell
{
  protected override Control CreateControl(BocCustomCellArguments arguments)
  {
    HtmlInputFile inputFile = new HtmlInputFile();
    return inputFile;
  }

  protected override void OnLoad(BocCustomCellLoadArguments arguments)
  {
    if (arguments.List.Page.IsPostBack)
    {
      HtmlInputFile inputFile = (HtmlInputFile) arguments.Control;
      if (inputFile != null && inputFile.PostedFile != null)
      {
      }
    }
  }

  protected override void OnClick(BocCustomCellClickArguments arguments, string eventArgument)
  {
  }

  protected override void OnValidate(BocCustomCellValidationArguments arguments)
  {
    arguments.IsValid = false;
  }

  protected override void Render (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
  {
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "5em");
    writer.AddStyleAttribute ("overflow", "hidden");
    writer.AddStyleAttribute ("white-space", "nowrap");
    writer.AddAttribute (HtmlTextWriterAttribute.Title, "This is a very, very, very, very, very, very, very, very, very long text.");
    writer.RenderBeginTag (HtmlTextWriterTag.Span);

    writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
    string onClickEvent = GetPostBackClientEvent ("1");
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("1");
    writer.RenderEndTag();
    writer.Write ("<br>");

    writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
    onClickEvent = GetPostBackClientEvent ("2");
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("2");
    writer.RenderEndTag();
    writer.Write ("<br>");
    writer.Write ("This is a very, very, very, very, very, very, very, very, very long text.");

    writer.RenderEndTag();
  }
}

}
