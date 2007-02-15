using System;
using Rubicon.ObjectBinding.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

public class BocListSortingOrderEntryMock : BocListSortingOrderEntry
{
  public BocListSortingOrderEntryMock (BocColumnDefinition column, SortingDirection direction)
    : base (column, direction)
  {
  }

  public BocListSortingOrderEntryMock (int columnIndex, SortingDirection direction)
    : base (columnIndex, direction)
  {
  }

  public new int ColumnIndex
  {
    get { return base.ColumnIndex; }
  }

  public new void SetColumnIndex (int columnIndex)
  {
    base.SetColumnIndex (columnIndex);
  }

  public new void SetColumn (BocColumnDefinition column)
  {
    base.SetColumn (column); 
  }

  public new void SetDirection (SortingDirection direction)
  {
    base.SetDirection  (direction);
  }
}

}
