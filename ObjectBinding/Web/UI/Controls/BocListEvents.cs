using System;
using System.ComponentModel;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls
{

#region Obsolete

[Obsolete ("Use BocListEditableRowChangesEventHandler instead.", true)]
public delegate void BocListRowEditModeEventHandler (object sender, BocListRowEditModeEventArgs e);

[Obsolete ("Use BocListEditableRowChangesEventArgs instead.", true)]
public class BocListRowEditModeEventArgs: BocListEditableRowChangesEventArgs
{
  public BocListRowEditModeEventArgs (
      int listIndex, 
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundModifiableWebControl[] controls)
    : base (listIndex, businessObject, dataSource, controls)
  {
    throw new NotImplementedException ("Obsolete");
  }
}

#endregion

public delegate void BocListSortingOrderChangeEventHandler (object sender, BocListSortingOrderChangeEventArgs e);

public class BocListSortingOrderChangeEventArgs: EventArgs
{
  private BocListSortingOrderEntry[] _oldSortingOrder;
  private BocListSortingOrderEntry[] _newSortingOrder;

  public BocListSortingOrderChangeEventArgs (
      BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("oldSortingOrder", oldSortingOrder);
    ArgumentUtility.CheckNotNullOrItemsNull ("newSortingOrder", newSortingOrder);

    _oldSortingOrder = oldSortingOrder;
    _newSortingOrder = newSortingOrder;
  }

  /// <summary> Gets the old sorting order of the <see cref="BocList"/>. </summary>
  public BocListSortingOrderEntry[] OldSortingOrder
  {
    get { return _oldSortingOrder; }
  }

  /// <summary> Gets the new sorting order of the <see cref="BocList"/>. </summary>
  public BocListSortingOrderEntry[] NewSortingOrder
  {
    get { return _newSortingOrder; }
  }
}

public delegate void BocListItemEventHandler (object sender, BocListItemEventArgs e);

public class BocListItemEventArgs: EventArgs
{
  private int _listIndex;
  private IBusinessObject _businessObject;

  public BocListItemEventArgs (
      int listIndex, 
      IBusinessObject businessObject)
  {
    _listIndex = listIndex;
    _businessObject = businessObject;
  }

  /// <summary> An index that identifies the <see cref="IBusinessObject"/> that has been edited. </summary>
  public int ListIndex
  {
    get { return _listIndex; }
  }

  /// <summary>
  ///   The <see cref="IBusinessObject"/> that has been edited.
  /// </summary>
  public IBusinessObject BusinessObject
  {
    get { return _businessObject; }
  }
}

public delegate void BocListEditableRowChangesEventHandler (object sender, BocListEditableRowChangesEventArgs e);

public class BocListEditableRowChangesEventArgs : BocListItemEventArgs
{
  private IBusinessObjectBoundModifiableControl[] _controls;
  private IBusinessObjectDataSource _dataSource;

  public BocListEditableRowChangesEventArgs (
      int listIndex, 
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundModifiableWebControl[] controls)
    : base (listIndex, businessObject)
  {
    _dataSource = dataSource;
    _controls = controls;
  }

  public IBusinessObjectDataSource DataSource
  {
    get { return _dataSource; }
  }

  public IBusinessObjectBoundModifiableControl[] Controls
  {
    get { return _controls; }
  }
}

public delegate void BocListDataRowRenderEventHandler (object sender, BocListDataRowRenderEventArgs e);

public class BocListDataRowRenderEventArgs: BocListItemEventArgs
{
  #region Obsolete
  
  [Obsolete ("Use IsEditableRow instead.")]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public bool IsModifiableRow
  {
    get { return IsEditableRow; }
    set { IsEditableRow = value; }
  }

  #endregion

  private bool _isEditableRow = true;

  public BocListDataRowRenderEventArgs (int listIndex, IBusinessObject businessObject)
    : base (listIndex, businessObject)
  {
  }

  public bool IsEditableRow
  {
    get { return _isEditableRow; }
    set { _isEditableRow = value; }
  }
}

}
