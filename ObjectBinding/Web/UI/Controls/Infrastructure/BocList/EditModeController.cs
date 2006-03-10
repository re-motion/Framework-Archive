using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

[ToolboxItem (false)]
public class EditModeController : PlaceHolder
{
  // types

  // static members and constants

  private const string c_whiteSpace = "&nbsp;";

  // member fields

  private Controls.BocList _owner;

  private bool _isListEditModeActive;
  private NaInt32 _modifiableRowIndex = NaInt32.Null;
  private bool _isEditNewRow = false;
  
  private bool _isListEditModeRestored;
  private bool _isRowEditModeRestored = false;
  
  internal ModifiableRow _modifiableRow;
  internal ModifiableRow[] _modifiableRows;
  private PlaceHolder _modifiableRowsPlaceHolders;

  private bool _enableEditModeValidator = true;
  private bool _showEditModeRequiredMarkers = true;
  private bool _showEditModeValidationMarkers = false;
  private bool _disableEditModeValidationMessages = false;

  // construction and disposing

  public EditModeController (Controls.BocList owner)
  {
    ArgumentUtility.CheckNotNull ("owner", owner);

    _owner = owner;
  }

  // methods and properties

  public Controls.BocList Owner
  {
    get { return _owner; }
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    EnsureChildControls();
  }

  protected override void CreateChildControls()
  {
    Controls.Clear();

    _modifiableRowsPlaceHolders = new PlaceHolder();
    Controls.Add (_modifiableRowsPlaceHolders);
    
    _modifiableRow = new ModifiableRow (_owner);
    _modifiableRow.ID = ID + "_Row";
    Controls.Add (_modifiableRow);
  }

  public void RenderTitleCellMarkers (HtmlTextWriter writer, BocColumnDefinition column, int columnIndex)
  {
    bool isRequired = false;
    if (   IsEditDetailsModeActive
        && _showEditModeRequiredMarkers 
        && _modifiableRow.HasEditControl (columnIndex)
        && _modifiableRow.GetEditControl (columnIndex).IsRequired)
    {
      isRequired = true;
    }
    else if (IsListEditModeActive)
    {
      for (int i = 0; i < _modifiableRows.Length; i++)
      {
        if (   _modifiableRows[i].HasEditControl (columnIndex)
            && _modifiableRows[i].GetEditControl (columnIndex).IsRequired)
        {
          isRequired = true;
          break;
        }
      }
    }
    if (isRequired)
    {
      Image requriedFieldMarker = _owner.GetRequiredMarker();
      requriedFieldMarker.RenderControl (writer);
      writer.Write (c_whiteSpace);
    }
  }

  /// <summary>
  ///   Saves changes to previous edited row and starts editing for the new row.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     Once the list is in edit mode, it is important not to change to index of the edited 
  ///     <see cref="IBusinessObject"/> in <see cref="Value"/>. Otherwise the wrong object would be edited.
  ///     Use <see cref="IsEditDetailsModeActive"/> to programatically check whether it is save to insert a row.
  ///   </para><para>
  ///     While the list is in edit mode, all commands and menus for this list are disabled with the exception of
  ///     those rendered in the <see cref="BocEditDetailsColumnDefinition"/> column.
  ///   </para>
  /// </remarks>
  /// <param name="index"></param>
  public void SwitchRowIntoEditMode (int index, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    if (index < 0) throw new ArgumentOutOfRangeException ("index");
    if (_owner.IsEmptyList) throw new ArgumentOutOfRangeException ("index");
    if (index >= _owner.Value.Count) throw new ArgumentOutOfRangeException ("index");

    EnsureListEditModeRestored (oldColumns);
    if (IsListEditModeActive)
      _owner.EndListEditMode (true);

    EnsureRowEditModeRestored (oldColumns);
    if (IsEditDetailsModeActive)
      _owner.EndEditDetailsMode (true);
    
    if (_owner.IsReadOnly || IsListEditModeActive || IsEditDetailsModeActive)
      return;

    _modifiableRowIndex = index;
    CreateRowEditModeControls (columns);
    _modifiableRow.GetDataSource().LoadValues (false);
  }

  public void EndEditDetailsMode (bool saveChanges, BocColumnDefinition[] oldColumns)
  {
    EnsureRowEditModeRestored (oldColumns);

    if (! IsEditDetailsModeActive)
      return;

    if (! _owner.IsReadOnly)
    {
      if (saveChanges)
      {
        _owner.OnModifiedRowSaving (
            ModifiableRowIndex.Value, 
            (IBusinessObject) _owner.Value[ModifiableRowIndex.Value],
            _modifiableRow.GetDataSource(),
            _modifiableRow.GetEditControlsAsArray());
        
        bool isValid = _owner.ValidateModifiableRows();
        if (! isValid)
          return;

        _owner.IsDirty = IsDirty();

        _modifiableRow.GetDataSource().SaveValues (false);

        _owner.OnModifiedRowSaved (ModifiableRowIndex.Value, (IBusinessObject) _owner.Value[ModifiableRowIndex.Value]);
      }
      else
      {
        _owner.OnModifiedRowCanceling (
            ModifiableRowIndex.Value, 
            (IBusinessObject) _owner.Value[ModifiableRowIndex.Value], 
            _modifiableRow.GetDataSource(), 
            _modifiableRow.GetEditControlsAsArray());
        
        if (_isEditNewRow)
        {
          IBusinessObject editedBusinessObject = (IBusinessObject) _owner.Value[ModifiableRowIndex.Value];
          _owner.RemoveRow (ModifiableRowIndex.Value);
          _owner.OnModifiedRowCanceled (-1, editedBusinessObject);
        }
        else
        {
          _owner.OnModifiedRowCanceled (ModifiableRowIndex.Value, (IBusinessObject) _owner.Value[ModifiableRowIndex.Value]);
        }
      }

      _owner.EndEditDetailsModeCleanUp();
    }

    RemoveEditModeControls();
    _modifiableRowIndex = NaInt32.Null;
    _isEditNewRow = false;
  }


  /// <remarks>
  ///   If already in row edit mode and the previous row cannot be saved, the new row will not be added to the list.
  /// </remarks>
  /// <param name="businessObject"></param>
  public bool AddAndEditRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    EnsureListEditModeRestored (oldColumns);
    if (IsListEditModeActive)
      _owner.EndListEditMode (true);

    EnsureRowEditModeRestored (oldColumns);
    if (IsEditDetailsModeActive)
      _owner.EndEditDetailsMode (true);

    if (_owner.IsReadOnly || IsListEditModeActive || IsEditDetailsModeActive)
      return false;

    int index = _owner.AddRow (businessObject);
    if (index < 0)
      return false;

    SwitchRowIntoEditMode (index, oldColumns, columns);
    if (IsEditDetailsModeActive)
    {
      _isEditNewRow = true;
      return true;
    }
    else
    {
      RemoveRow (businessObject);
      return false;
    }
  }

  private void CreateRowEditModeControls (BocColumnDefinition[] columns)
  {
    EnsureChildControls();

    if (! IsEditDetailsModeActive)
      return;

    if (_owner.Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "Row edit mode cannot be restored: The BocList '{0}' does not have a Value.", ID));
    }

    if (_modifiableRowIndex.Value >= _owner.Value.Count)
    {
      _modifiableRowIndex = NaInt32.Null;
      return;
    }

    _modifiableRow.CreateControls (columns, (IBusinessObject) _owner.Value[_modifiableRowIndex.Value]);
  }

  /// <remarks>
  ///   Validators must be added to the controls collection after LoadPostData is complete.
  ///   If not, invalid validators will know that they are invalid without first calling validate.
  ///   the <see cref="FormGridManager"/> also generates the validators after the <c>OnLoad</c> or when
  ///   <see cref="FormGridManager.Validate"/> is called. Therefor the behaviors of the <c>BocList</c>
  ///   and the <c>FormGridManager</c> match.
  /// </remarks>
  private void EnsureRowEditModeValidatorsRestored()
  {
    if (! IsEditDetailsModeActive)
      return;

    _modifiableRow.EnsureValidatorsRestored();
  }

  private void EnsureRowEditModeRestored (BocColumnDefinition[] oldColumns)
  {
    if (_isRowEditModeRestored)
      return;
    _isRowEditModeRestored = true;
    if (! IsEditDetailsModeActive)
      return;

    CreateRowEditModeControls (oldColumns);
  }

  public bool ValidateModifiableRows()
  {
    bool isValid = true;

    EnsureValidatorsRestored();

    if (IsEditDetailsModeActive)
      isValid = _modifiableRow.Validate();
    else if (IsListEditModeActive)
      isValid &= ValidateListEditModeRows();

    return isValid;
  }

  private void RemoveEditModeControls()
  {
    _modifiableRow.RemoveControls();
  }

  public bool IsDirty()
  {
    if (IsEditDetailsModeActive)
    {
      return _modifiableRow.IsDirty();
    }
    else if (IsListEditModeActive)
    {
      for (int i = 0; i < _modifiableRows.Length; i++)
      {
        if (_modifiableRows[i].IsDirty())
          return true;
      }
      return false;
    }
    else
    {
      return false;
    }
  }

  /// <summary> 
  ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
  ///   interface.
  /// </summary>
  /// <returns> 
  ///   Returns the <see cref="Control.ClientID"/> values of the edit mode controls for the row currently being edited.
  /// </returns>
  /// <seealso cref="BusinessObjectBoundModifiableWebControl.GetTrackedClientIDs">BusinessObjectBoundModifiableWebControl.GetTrackedClientIDs</seealso>
  public string[] GetTrackedClientIDs()
  {
    if (IsEditDetailsModeActive)
    {
      return _modifiableRow.GetTrackedClientIDs();
    }
    else if (IsListEditModeActive)
    {
      StringCollection trackedIDs = new StringCollection();
      for (int i = 0; i < _modifiableRows.Length; i++)
        trackedIDs.AddRange (_modifiableRows[i].GetTrackedClientIDs());

      string[] trackedIDsArray = new string[trackedIDs.Count];
      trackedIDs.CopyTo (trackedIDsArray, 0);
      return trackedIDsArray;
    }
    else
    {
      return new string[0];
    }
  }

  internal void EnsureRestored (BocColumnDefinition[] oldColumns)
  {
    EnsureRowEditModeRestored (oldColumns);
    EnsureListEditModeRestored (oldColumns);
  }

  internal void EnsureValidatorsRestored()
  {
    EnsureRowEditModeValidatorsRestored();
    EnsureListEditModeValidatorsRestored();
  }

  /// <remarks>
  ///   Queried where the rendering depends on whether the list is in edit mode. 
  ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
  /// </remarks>
  public bool IsListEditModeActive
  {
    get { return _isListEditModeActive; } 
  }

  public void SwitchListIntoEditMode (BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    EnsureRowEditModeRestored (oldColumns);
    if (IsEditDetailsModeActive)
      _owner.EndEditDetailsMode (true);

    EnsureListEditModeRestored (oldColumns);
    if (IsListEditModeActive)
      _owner.EndListEditMode (true);

    if (_owner.IsReadOnly || IsEditDetailsModeActive || IsListEditModeActive)
      return;

    _isListEditModeActive = true;
    CreateListEditModeControls (columns);
    for (int i = 0; i < _modifiableRows.Length; i++)
      _modifiableRows[i].GetDataSource().LoadValues (false);
  }

  public void EndListEditMode (bool saveChanges, BocColumnDefinition[] oldColumns)
  {
    EnsureListEditModeRestored (oldColumns);

    if (! IsListEditModeActive)
      return;

    if (! _owner.IsReadOnly)
    {
      if (saveChanges)
      {
        for (int i = 0; i < _modifiableRows.Length; i++)
        {
          _owner.OnModifiedRowSaving (
              i, 
              (IBusinessObject) _owner.Value[i], 
              _modifiableRows[i].GetDataSource(), 
              _modifiableRows[i].GetEditControlsAsArray());
        }

        bool isValid = _owner.ValidateModifiableRows();
        if (! isValid)
          return;

        _owner.IsDirty = IsDirty();

        for (int i = 0; i < _modifiableRows.Length; i++)
        {
          _modifiableRows[i].GetDataSource().SaveValues (false);
          _owner.OnModifiedRowSaved (i, (IBusinessObject) _owner.Value[i]);
        }
      }
      else
      {
        for (int i = 0; i < _modifiableRows.Length; i++)
        {
          _owner.OnModifiedRowCanceling (
              i, 
              (IBusinessObject) _owner.Value[i], 
              _modifiableRows[i].GetDataSource(), 
              _modifiableRows[i].GetEditControlsAsArray());
        }

        //if (_isEditNewRow)
        //{
        //  IBusinessObject editedBusinessObject = (IBusinessObject) Value[ModifiableRowIndex.Value];
        //  RemoveRow (ModifiableRowIndex.Value);
        //  OnRowEditModeCanceled (-1, editedBusinessObject);
        //}
        //else
        //{
        for (int i = 0; i < _modifiableRows.Length; i++)
          _owner.OnModifiedRowCanceled (i, (IBusinessObject) _owner.Value[i]);
        //}
      }

      _owner.EndListEditModeCleanUp();
    }

    RemoveListEditModeControls();
    _isListEditModeActive = false;
  }
  
  private void CreateListEditModeControls (BocColumnDefinition[] columns)
  {
    EnsureChildControls();

    if (! IsListEditModeActive)
      return;

    if (_owner.Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "List edit mode cannot be restored: The BocList '{0}' does not have a Value.", ID));
    }

    _modifiableRows = new ModifiableRow[_owner.Value.Count];
    _modifiableRowsPlaceHolders.Controls.Clear();

    for (int i = 0; i < _owner.Value.Count; i++)
    {
      ModifiableRow controller = CreateModifiableRow (i, columns);

      _modifiableRows[i] = controller;
      _modifiableRowsPlaceHolders.Controls.Add (controller);
    }
  }

  private ModifiableRow CreateModifiableRow (int rowIndex, BocColumnDefinition[] columns)
  {
    ModifiableRow controller = new ModifiableRow (_owner);
    controller.ID = ID + "_ModifiableRow_" + rowIndex.ToString();
    controller.CreateControls (columns, (IBusinessObject) _owner.Value[rowIndex]);

    return controller;
  }

  internal void EnsureListEditModeRestored (BocColumnDefinition[] oldColumns)
  {
    if (_isListEditModeRestored)
      return;
    _isListEditModeRestored = true;
    if (! IsListEditModeActive)
      return;

    CreateListEditModeControls (oldColumns);
  }

  private void EnsureListEditModeValidatorsRestored ()
  {
    if (! IsListEditModeActive)
      return;

    for (int i = 0; i < _modifiableRows.Length; i++)
      _modifiableRows[i].EnsureValidatorsRestored();
  }

  private bool ValidateListEditModeRows ()
  {
    EnsureListEditModeValidatorsRestored();

    bool isValid = true;
    for (int i = 0; i < _modifiableRows.Length; i++)
      isValid &= _modifiableRows[i].Validate();
    return isValid;
 }

  private void RemoveListEditModeControls()
  {
    for (int i = 0; i < _modifiableRows.Length; i++)
      _modifiableRows[i].RemoveControls();
  }

  /// <summary> Calls the parent's <c>LoadViewState</c> method and restores this control's specific data. </summary>
  /// <param name="savedState"> An <see cref="Object"/> that represents the control state to be restored. </param>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    
    base.LoadViewState (values[0]);
    _isListEditModeActive = (bool) values[1];
    _modifiableRowIndex = (NaInt32) values[2];
    _isEditNewRow = (bool) values[3];
  }

  /// <summary> Calls the parent's <c>SaveViewState</c> method and saves this control's specific data. </summary>
  /// <returns> Returns the server control's current view state. </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[4];

    values[0] = base.SaveViewState();
    values[1] = _isListEditModeActive;
    values[2] = _modifiableRowIndex;
    values[3] = _isEditNewRow;

    return values;
  }

  /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void AddRows (IBusinessObject[] businessObjects, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

    if (_owner.Value == null)
      return;

    EnsureListEditModeRestored (oldColumns);
    if (IsListEditModeActive)
    {
      int startIndex = _owner.Value.Count - businessObjects.Length;
      ArrayList controllers = new ArrayList (businessObjects.Length);
      for (int i = startIndex; i < _owner.Value.Count; i++)
      {
        ModifiableRow controller = CreateModifiableRow (i, columns);
        controller.GetDataSource().LoadValues (false);
        _modifiableRowsPlaceHolders.Controls.Add (controller);
        controllers.Add (controller);
      }
      _modifiableRows = (ModifiableRow[]) ListUtility.AddRange (
          _modifiableRows, controllers, (CreateListMethod) null, true, true);
    }
  }

  /// <summary> Adds the <paramref name="businessObject"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  internal void AddRow (int index, IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    
    EnsureListEditModeRestored (oldColumns);
    if (IsListEditModeActive)
    {
      ModifiableRow controller = CreateModifiableRow (index, columns);
      controller.GetDataSource().LoadValues (false);
      _modifiableRowsPlaceHolders.Controls.Add (controller);
      _modifiableRows = (ModifiableRow[]) ListUtility.AddRange (
          _modifiableRows, controller, (CreateListMethod) null, true, true);
    }
  }

  /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void RemoveRows (IBusinessObject[] businessObjects)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);
    if (_owner.Value == null)
      return;

    if (_isEditNewRow && IsEditDetailsModeActive)
    {
      foreach (IBusinessObject businessObject in businessObjects)
      {
        if (businessObject == _owner.Value[ModifiableRowIndex.Value])
        {
          _isEditNewRow = false;
          break;
        }
      }
    }
    
    if (IsListEditModeActive)
    {
      int[] indices = ListUtility.IndicesOf (_owner.Value, businessObjects, false);
      ArrayList controllers = new ArrayList (indices.Length);
      foreach (int index in indices)
      {
        ModifiableRow controller = _modifiableRows[index];
        _modifiableRowsPlaceHolders.Controls.Remove (controller);
        controller.RemoveControls();
        controllers.Add (controller);
      }
      _modifiableRows = (ModifiableRow[]) ListUtility.Remove (
          _modifiableRows, controllers, (CreateListMethod) null, true);
    }

    _owner.RemoveRowsInternal (businessObjects);

    if (IsListEditModeActive)
      RefreshIDs();
  }

  /// <summary> Removes the <paramref name="businessObject"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void RemoveRow (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    if (_owner.Value == null)
      return;

    if (   _isEditNewRow 
        && IsEditDetailsModeActive
        && businessObject == _owner.Value[ModifiableRowIndex.Value])
    {
      _isEditNewRow = false;
    }
    
    if (IsListEditModeActive)
    {
      int index = ListUtility.IndexOf (_owner.Value, businessObject);
      if (index != -1)
      {
        ModifiableRow controller = _modifiableRows[index];
        _modifiableRowsPlaceHolders.Controls.Remove (controller);
        controller.RemoveControls();
        _modifiableRows = (ModifiableRow[]) ListUtility.Remove (
            _modifiableRows, controller, (CreateListMethod) null, true);
      }
    }

    _owner.RemoveRowInternal (businessObject);

    if (IsListEditModeActive)
      RefreshIDs();
  }

  private void RefreshIDs()
  {
    for (int i = 0; i < _modifiableRowsPlaceHolders.Controls.Count; i++)
    {
      ModifiableRow row = (ModifiableRow) _modifiableRowsPlaceHolders.Controls[i];
      string newID = ID + "_ModifiableRow_" + i.ToString();
      if (row.ID != newID)
        row.ID = newID;
    }
  }

  public NaInt32 ModifiableRowIndex
  {
    get { return _modifiableRowIndex; }
  }

  /// <remarks>
  ///   Queried where the rendering depends on whether the list is in edit mode. 
  ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
  /// </remarks>
  public bool IsEditDetailsModeActive
  {
    get { return ! _modifiableRowIndex.IsNull; } 
  }

  public bool ShowEditModeRequiredMarkers
  {
    get { return _showEditModeRequiredMarkers; }
    set { _showEditModeRequiredMarkers = value; }
  }

  public bool ShowEditModeValidationMarkers
  {
    get { return _showEditModeValidationMarkers; }
    set { _showEditModeValidationMarkers = value; }
  }

  public bool DisableEditModeValidationMessages
  {
    get { return _disableEditModeValidationMessages; }
    set { _disableEditModeValidationMessages = value; }
  }

  public bool EnableEditModeValidator
  {
    get { return _enableEditModeValidator; }
    set { _enableEditModeValidator = value; }
  }

  /// <summary>
  ///   Generates a <see cref="BocList.EditDetailsValidator"/>.
  /// </summary>
  /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
  public BaseValidator[] CreateValidators (IResourceManager resourceManager)
  {
    if (! (IsListEditModeActive || IsEditDetailsModeActive) || ! _enableEditModeValidator)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];

    EditDetailsValidator editDetailsValidator = new EditDetailsValidator (_owner);
    editDetailsValidator.ID = ID + "_ValidatorEditDetails";
    editDetailsValidator.ControlToValidate = ID;
    if (StringUtility.IsNullOrEmpty (_owner.ErrorMessage))
    {
      editDetailsValidator.ErrorMessage = 
          resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.EditDetailsErrorMessage);
    }
    else
    {
      editDetailsValidator.ErrorMessage = _owner.ErrorMessage;
    }
    validators[0] = editDetailsValidator;

    return validators;
  }
}

}
