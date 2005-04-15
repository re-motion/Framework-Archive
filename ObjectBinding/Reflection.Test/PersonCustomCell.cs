using System;
using System.Web.UI;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace OBRTest
{
public class PersonCustomCell: BocCustomColumnDefinitionCell
{
  protected override void DoRender (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
  {
    writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
    string onClickEvent = GetPostBackClientEvent ("1");
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent + arguments.OnClick);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("1");
    writer.RenderEndTag();
    writer.Write ("<br>");

    writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
    onClickEvent = GetPostBackClientEvent ("2");
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent + arguments.OnClick);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("2");
    writer.RenderEndTag();
    writer.Write ("<br>");
  }
}

}
