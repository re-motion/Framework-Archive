using System;
using System.Web.UI;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

namespace OBRTest
{
public class PersonCustomCell: IBocCustomColumnDefinitionCell
{
  public PersonCustomCell (string argument)
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
      int listIndex)
  {
    writer.AddAttribute (
        HtmlTextWriterAttribute.Href, 
        list.GetCustomCellPostBackClientHyperlink (columnIndex, listIndex, "1"));
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("1 <br>");
    writer.RenderEndTag();

    writer.AddAttribute (
        HtmlTextWriterAttribute.Href, 
        list.GetCustomCellPostBackClientHyperlink (columnIndex, listIndex, "2"));
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    writer.Write ("2 <br>");
    writer.RenderEndTag();
  }

}
}
