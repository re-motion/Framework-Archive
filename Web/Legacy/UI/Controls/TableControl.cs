using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{
public class TableControl : Control
{ 
  public enum OrientationType
  {
    Horizontal = 0,
    Vertical = 1
  }

  protected Table _table = new Table ();
  private OrientationType _orientation = OrientationType.Horizontal;
  
  public OrientationType Orientation
  {
    get { return _orientation; }
    set { _orientation = value; }
  }

  public Unit Width
  {
    get { return _table.Width; }
    set { _table.Width = value; }
  }
  
  public string CssClass
  {
    get { return _table.CssClass; }
    set { _table.CssClass = value; }
  }

  public void Clear ()
  {
    _table.Rows.Clear ();
  }

  public TableRow AddRow (string cssClass, params string[] headers)
  {
     return AddRowWithStyle (cssClass, null, headers);
  }

  public TableRow AddRowWithStyle (string cssClass, string cssStyle, params string[] headers)
  {
    if (Orientation == OrientationType.Horizontal)
      return AddHorizontalRow (cssClass, cssStyle, headers);
    else
      return AddVerticalRow (cssClass, cssStyle, headers);
  }
  
  private TableRow AddHorizontalRow (string cssClass, string cssStyle, params string[] values)
  {
    TableRow newRow = new TableRow ();

    foreach (string columnValue in values)
    {
      TableCell newCell = new TableCell ();
      newCell.Text = columnValue;
      newCell.CssClass = cssClass;
      
      if (cssStyle != null)
        newCell.Attributes["style"] = cssStyle;

      newRow.Cells.Add (newCell);
    }
    
    _table.Rows.Add (newRow);

    return newRow;
  }
  
  private TableRow AddVerticalRow (string cssClass, string cssStyle, params string[] values)
  {
    bool createRows = (_table.Rows.Count == 0);

    for (int i = 0; i < values.Length; ++i)
    { 
      string rowValue = values[i];

      TableRow tableRow;
      if (createRows)
      {
        tableRow = new TableRow ();
        _table.Rows.Add (tableRow);
      }
      else
      {
        tableRow = _table.Rows[i];
      }
      
      TableCell newCell = new TableCell ();

      newCell.Text = rowValue;
      newCell.CssClass = cssClass;

      if (cssStyle != null)
        newCell.Attributes["style"] = cssStyle;

      tableRow.Cells.Add (newCell);
    }

    return null;
  }

  protected override void Render (HtmlTextWriter writer)
  {
    _table.RenderControl (writer);
  }
}
}
