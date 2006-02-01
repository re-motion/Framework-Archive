using System;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{
public class BocListSortingOrderProviderMock : IBocListSortingOrderProvider
{
  BocListSortingOrderEntry[] _sortingOrder = new BocListSortingOrderEntryMock[0];

  public void SetSortingOrder (BocListSortingOrderEntry[] sortingOrder)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("sortingOrder", sortingOrder);
    _sortingOrder = sortingOrder;
  }

  public BocListSortingOrderEntry[] GetSortingOrder()
  {
    return _sortingOrder;
  }
}

}
