using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

[ToolboxItemFilter("System.Web.UI")]
public class BocList: BusinessObjectBoundModifiableWebControl
{
  private ColumnDefinition[] _fixedColumns; // can only be set at design time. 
  private ColumnDefinitionCollection _additionalColumns; // may be set at run time. these columns do usually not contain commands.
  private ItemCommand _firstColumnCommand; // the command to be used for the first ValueColumnDefinition column
  private bool _showSelection; // show check boxes for each object
  private bool _showAdditionalColumnsSelection; // show drop down list for selecting additional columns
  private ColumnDefinitionCollection[] _availableColumnDefinitionCollections; // user may choose one ColumnDefinitionCollection
  private int _pageSize = 0; // <= 0: show all objects, > 0: show n objects per page
  private bool _alwaysShowPageInfo = false; // show page info ("page 1 of n") and links always (true), or only if there is more than 1 page (false)

  protected override void Render (HtmlTextWriter writer)
  {
    writer.WriteBeginTag ("table");
    writer.Write (HtmlTextWriter.TagRightChar);

    writer.WriteBeginTag ("tr");
    writer.Write (HtmlTextWriter.TagRightChar);

    ColumnDefinition[] columns;
    if (_additionalColumns == null)
      columns = _fixedColumns;
    else
      columns = (ColumnDefinition[]) ArrayUtility.Combine (_fixedColumns, _additionalColumns.Columns);

    foreach (IColumnDefinition column in columns)
    {
      writer.WriteBeginTag ("td");
      if (! column.Width.IsEmpty)
        writer.WriteStyleAttribute ("width", column.Width.ToString());
      writer.Write (HtmlTextWriter.TagRightChar);

      HttpUtility.HtmlEncode (column.Title, writer);

      writer.WriteEndTag ("td");
    }
    writer.WriteEndTag ("tr");
    writer.WriteEndTag ("table");
  }

}

}
