using System;

namespace Rubicon.ObjectBinding.Web.Controls
{

public delegate void BocListItemEventHandler (object sender, BocListItemEventArgs e);

public class BocListItemEventArgs: EventArgs
{
  private int _listIndex;
  private IBusinessObject _businessObject;

  /// <summary> Initializes a new instance. </summary>
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

public delegate void BocListRowEditModeEventHandler (object sender, BocListRowEditModeEventArgs e);

public class BocListRowEditModeEventArgs: BocListItemEventArgs
{
  private IBusinessObjectBoundModifiableControl[] _controls;
  private IBusinessObjectDataSource _dataSource;

  /// <summary> Initializes a new instance. </summary>
  public BocListRowEditModeEventArgs (
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
  private bool _isModifiableRow = true;

  /// <summary> Initializes a new instance. </summary>
  public BocListDataRowRenderEventArgs (int listIndex, IBusinessObject businessObject)
    : base (listIndex, businessObject)
  {
  }

  public bool IsModifiableRow
  {
    get { return _isModifiableRow; }
    set { _isModifiableRow = value; }
  }
}

}
