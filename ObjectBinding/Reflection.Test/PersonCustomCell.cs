using System;
using System.Web.UI;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace OBRTest
{
public class PersonCustomCell: IBocCustomColumnDefinitionCell
{
  public PersonCustomCell ()
  {
  }

  public void OnClick(
      BocList list, 
      IBusinessObject businessObject, 
      BocCustomColumnDefinition columnDefiniton, 
      string argument)
  {
  }

  public void Render(
      HtmlTextWriter writer, 
      BocList list, 
      IBusinessObject businessObject, 
      BocCustomColumnDefinition columnDefiniton, 
      int columnIndex, 
      int listIndex,
      string onClick)
  {
    writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
    string onClickEvent = list.GetCustomCellPostBackClientEvent (columnIndex, listIndex, "1");
    onClickEvent += onClick;
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("1 <br>");
    writer.RenderEndTag();

    writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
    onClickEvent = list.GetCustomCellPostBackClientEvent (columnIndex, listIndex, "2");
    onClickEvent += onClick;
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClickEvent);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("2 <br>");
    writer.RenderEndTag();
  }

}
}
