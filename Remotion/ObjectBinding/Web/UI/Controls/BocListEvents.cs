// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.ComponentModel;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
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
  private IBusinessObjectBoundEditableControl[] _controls;
  private IBusinessObjectDataSource _dataSource;

  public BocListEditableRowChangesEventArgs (
      int listIndex, 
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundEditableWebControl[] controls)
    : base (listIndex, businessObject)
  {
    _dataSource = dataSource;
    _controls = controls;
  }

  public IBusinessObjectDataSource DataSource
  {
    get { return _dataSource; }
  }

  public IBusinessObjectBoundEditableControl[] Controls
  {
    get { return _controls; }
  }
}

public delegate void BocListDataRowRenderEventHandler (object sender, BocListDataRowRenderEventArgs e);

public class BocListDataRowRenderEventArgs: BocListItemEventArgs
{
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
