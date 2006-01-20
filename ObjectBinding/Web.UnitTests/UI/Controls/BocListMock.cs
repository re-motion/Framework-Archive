using System;
using System.ComponentModel;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UnitTests.UI;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocList"/> type. </summary>
[ToolboxItem (false)]
public class BocListMock: BocList
{
	public new void EvaluateWaiConformity (BocColumnDefinition[] columns)
  {
    base.EvaluateWaiConformity (columns);
  }

  public new bool HasOptionsMenu
  {
    get { return base.HasOptionsMenu; }
  }

  public new bool HasListMenu
  {
    get { return base.HasListMenu; }
  }

  public new bool HasAvailableViewsList
  {
    get { return base.HasAvailableViewsList; }
  }

  public new bool IsSelectionEnabled
  {
    get { return base.IsSelectionEnabled; }
  }

  public new bool IsPagingEnabled
  {
    get { return base.IsPagingEnabled; }
  }

  public new bool IsColumnVisible (BocColumnDefinition column)
  {
    return base.IsColumnVisible (column);
  }


  public new bool IsClientSideSortingEnabled
  {
    get { return base.IsClientSideSortingEnabled; }
  }
  
}

}
