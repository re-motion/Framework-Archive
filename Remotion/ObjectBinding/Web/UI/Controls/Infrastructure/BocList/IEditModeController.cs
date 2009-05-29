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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{
  public interface IEditModeController : IControl
  {
    Controls.BocList OwnerControl { get; }
    bool IsRowEditModeActive { get; }
    bool IsListEditModeActive { get; }
    int? EditableRowIndex { get; }
    bool ShowEditModeRequiredMarkers { get; set; }
    bool ShowEditModeValidationMarkers { get; set; }
    bool DisableEditModeValidationMessages { get; set; }
    bool EnableEditModeValidator { get; set; }
    void SwitchRowIntoEditMode (int index, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns);
    void SwitchListIntoEditMode (BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns);
    bool AddAndEditRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns);
    void EndRowEditMode (bool saveChanges, BocColumnDefinition[] oldColumns);
    void EndListEditMode (bool saveChanges, BocColumnDefinition[] oldColumns);
    void EnsureEditModeRestored (BocColumnDefinition[] oldColumns);
    void AddRows (IBusinessObject[] businessObjects, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns);
    int AddRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns);
    void RemoveRows (IBusinessObject[] businessObjects);
    void RemoveRow (IBusinessObject businessObject);
    BaseValidator[] CreateValidators (IResourceManager resourceManager);

    /// <remarks>
    ///   Validators must be added to the controls collection after LoadPostData is complete.
    ///   If not, invalid validators will know that they are invalid without first calling validate.
    /// </remarks>
    void EnsureValidatorsRestored ();

    void PrepareValidation ();
    bool Validate ();
    void RenderTitleCellMarkers (HtmlTextWriter writer, BocColumnDefinition column, int columnIndex);
    bool IsRequired (int columnIndex);
    bool IsDirty ();
    string[] GetTrackedClientIDs ();
    IEditableRow GetEditableRow (int originalRowIndex);
  }
}