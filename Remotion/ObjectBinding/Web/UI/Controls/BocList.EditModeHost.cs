// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public partial class BocList
  {
    private class EditModeHost : IEditModeHost
    {
      private readonly BocList _bocList;

      public EditModeHost (BocList bocList)
      {
        _bocList = bocList;
      }

      public IList Value
      {
        get { return _bocList.Value; }
      }

      public string ID
      {
        get { return _bocList.ID; }
      }

      public bool IsReadOnly
      {
        get { return _bocList.IsReadOnly; }
      }

      public bool IsDirty
      {
        get { return _bocList.IsDirty; }
        set { _bocList.IsDirty = value; }
      }

      public EditableRowDataSourceFactory EditModeDataSourceFactory
      {
        get { return _bocList.EditModeDataSourceFactory; }
      }

      public EditableRowControlFactory EditModeControlFactory
      {
        get { return _bocList.EditModeControlFactory; }
      }

      public string ErrorMessage
      {
        get { return _bocList.ErrorMessage; }
      }

      public bool DisableEditModeValidationMessages
      {
        get { return _bocList.DisableEditModeValidationMessages; }
      }

      public bool ShowEditModeValidationMarkers
      {
        get { return _bocList.ShowEditModeValidationMarkers; }
      }

      public bool ShowEditModeRequiredMarkers
      {
        get { return _bocList.ShowEditModeRequiredMarkers; }
      }

      public bool EnableEditModeValidator
      {
        get { return _bocList.EnableEditModeValidator; }
      }

      public IRowIDProvider RowIDProvider
      {
        get { return _bocList.RowIDProvider; }
      }

      public Image GetRequiredMarker ()
      {
        return _bocList.GetRequiredMarker();
      }

      public Image GetValidationErrorMarker ()
      {
        return _bocList.GetValidationErrorMarker();
      }

      public EditModeValidator GetEditModeValidator ()
      {
        return _bocList._validators.OfType<EditModeValidator>().FirstOrDefault();
      }

      public void AddRows (IBusinessObject[] businessObjects)
      {
        ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);

        var oldValue = _bocList.Value;

        var newValue = ListUtility.AddRange (oldValue, businessObjects, _bocList.Property, false, true);

        if (oldValue == null || newValue == null)
          _bocList.Value = newValue;
        else
        {
          _bocList.SetValue (newValue);
          _bocList.IsDirty = true;

          var indices = Utilities.ListUtility.IndicesOf (newValue, businessObjects, true);
          var rows = businessObjects.Zip (indices, (o, i) => new BocListRow (i, o)).OrderBy (r => r.Index);
          foreach (var row in rows)
            _bocList.RowIDProvider.AddRow (row);
        }
      }

      public void RemoveRows (IBusinessObject[] businessObjects)
      {
        ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);

        var oldValue = _bocList.Value;
        int[] indices = null;
        if (oldValue != null)
          indices = Utilities.ListUtility.IndicesOf (oldValue, businessObjects, true);

        var newValue = ListUtility.Remove (_bocList.Value, businessObjects, _bocList.Property, false);

        if (oldValue == null || newValue == null)
          _bocList.Value = newValue;
        else
        {
          _bocList.SetValue (newValue);
          _bocList.IsDirty = true;

          var rows = businessObjects.Zip (indices, (o, i) => new BocListRow (i, o)).OrderByDescending (r => r.Index);
          foreach (var row in rows)
            _bocList.RowIDProvider.RemoveRow (row);
        }
      }

      public void EndRowEditModeCleanUp (int modifiedRowIndex)
      {
        if (! _bocList.IsReadOnly)
        {
          _bocList.OnStateOfDisplayedRowsChanged();
          BocListRow[] sortedRows = _bocList.EnsureSortedBocListRowsGot();
          for (int idxRows = 0; idxRows < sortedRows.Length; idxRows++)
          {
            int originalRowIndex = sortedRows[idxRows].Index;
            if (modifiedRowIndex == originalRowIndex)
            {
              _bocList._currentRow = idxRows;
              break;
            }
          }
        }
      }

      public void EndListEditModeCleanUp ()
      {
        if (! _bocList.IsReadOnly)
          _bocList.OnStateOfDisplayedRowsChanged();
      }

      public bool ValidateEditableRows ()
      {
        return _bocList.ValidateCustomColumns();
      }

      public void OnEditableRowChangesSaving (
          int index,
          IBusinessObject businessObject,
          IBusinessObjectDataSource dataSource,
          IBusinessObjectBoundEditableWebControl[] controls)
      {
        _bocList.OnEditableRowChangesSaving (index, businessObject, dataSource, controls);
      }

      public void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
      {
        _bocList.OnEditableRowChangesSaved (index, businessObject);
      }

      public void OnEditableRowChangesCanceling (
          int index,
          IBusinessObject businessObject,
          IBusinessObjectDataSource dataSource,
          IBusinessObjectBoundEditableWebControl[] controls)
      {
        _bocList.OnEditableRowChangesCanceling (index, businessObject, dataSource, controls);
      }

      public void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
      {
        _bocList.OnEditableRowChangesCanceled (index, businessObject);
      }
    }
  }
}