using System;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UnitTests.UI;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocList"/> type. </summary>
public class BocListMock: BocList
{
  private WcagHelperMock _wcagHelper = new WcagHelperMock();

	public new void EvaluateWaiConformity (BocColumnDefinition[] columns)
  {
    base.EvaluateWaiConformity (columns);
  }

  protected override WcagHelper WcagHelper
  {
    get { return _wcagHelper; }
  }

  public WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelper; }
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
