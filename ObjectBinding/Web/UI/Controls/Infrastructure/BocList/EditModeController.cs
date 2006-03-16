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

  private static readonly object s_modifiableRowChangesSavingEvent = new object();
  private static readonly object s_modifiableRowChangesSavedEvent = new object();
  private static readonly object s_modifiableRowChangesCancelingEvent = new object();
  private static readonly object s_modifiableRowChangesCanceledEvent = new object();

  private const string c_whiteSpace = "&nbsp;";

  // member fields

  private Controls.BocList _ownerControl;

  private bool _isListEditModeActive;
  private NaInt32 _modifiableRowIndex = NaInt32.Null;
  private bool _isEditNewRow = false;
  
  private bool _isEditModeRestored = false;
  
  internal ModifiableRow[] _rows;

  private bool _enableEditModeValidator = true;
  private bool _showEditModeRequiredMarkers = true;
  private bool _showEditModeValidationMarkers = false;
  private bool _disableEditModeValidationMessages = false;

  // construction and disposing

  public EditModeController (Controls.BocList ownerControl)
  {
    ArgumentUtility.CheckNotNull ("ownerControl", ownerControl);

    _ownerControl = ownerControl;
  }

  // methods and properties

  public Controls.BocList OwnerControl
  {
    get { return _ownerControl; }
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
    ArgumentUtility.CheckNotNull ("oldColumns", oldColumns);
    ArgumentUtility.CheckNotNull ("columns", columns);

    if (_ownerControl.Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "Cannot initialize edit details mode: The BocList '{0}' does not have a Value.", _ownerControl.ID));
    }

    if (index < 0) throw new ArgumentOutOfRangeException ("index");
    if (index >= _ownerControl.Value.Count) throw new ArgumentOutOfRangeException ("index");

    RestoreAndEndEditMode (oldColumns);
    
    if (_ownerControl.IsReadOnly || IsListEditModeActive || IsEditDetailsModeActive)
      return;

    _modifiableRowIndex = index;
    CreateEditModeControls (columns);
    LoadValues (false);
  }

  public void SwitchListIntoEditMode (BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNull ("oldColumns", oldColumns);
    ArgumentUtility.CheckNotNull ("columns", columns);

    if (_ownerControl.Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "Cannot initialize list edit mode: The BocList '{0}' does not have a Value.", _ownerControl.ID));
    }

    RestoreAndEndEditMode (oldColumns);

    if (_ownerControl.IsReadOnly || IsEditDetailsModeActive || IsListEditModeActive)
      return;

    _isListEditModeActive = true;
    CreateEditModeControls (columns);
    LoadValues (false);
  }
  
  /// <remarks>
  ///   If already in row edit mode and the previous row cannot be saved, the new row will not be added to the list.
  /// </remarks>
  /// <param name="businessObject"></param>
  public bool AddAndEditRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNull ("oldColumns", oldColumns);
    ArgumentUtility.CheckNotNull ("columns", columns);

    RestoreAndEndEditMode (oldColumns);

    if (_ownerControl.IsReadOnly || IsListEditModeActive || IsEditDetailsModeActive)
      return false;

    int index = AddRow (businessObject, oldColumns, columns);
    if (index < 0)
      return false;

    SwitchRowIntoEditMode (index, oldColumns, columns);
    
    if (! IsEditDetailsModeActive)
    {
      throw new InvalidOperationException (string.Format (
          "BocList '{0}': Could not switch newly added row into edit mode.", 
          OwnerControl.ID));
    }
    _isEditNewRow = true;
    return true;
  }

  private void RestoreAndEndEditMode (BocColumnDefinition[] oldColumns)
  {
    EnsureEditModeRestored (oldColumns);

    if (IsEditDetailsModeActive)
      EndEditDetailsMode (true, oldColumns);
    else if (IsListEditModeActive)
      EndListEditMode (true, oldColumns);
  }

  public void EndEditDetailsMode (bool saveChanges, BocColumnDefinition[] oldColumns)
  {
    if (! IsEditDetailsModeActive)
      return;

    EnsureEditModeRestored (oldColumns);

    if (! _ownerControl.IsReadOnly)
    {
      int index = _modifiableRowIndex.Value;
      IBusinessObject value = (IBusinessObject) _ownerControl.Value[index];

      if (saveChanges)
      {
        OnModifiableRowChangesSaving (index, value, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());
        
        bool isValid = Validate();
        if (! isValid)
          return;

        _ownerControl.IsDirty = IsDirty();

        _rows[0].GetDataSource().SaveValues (false);
        OnModifiableRowChangesSaved (index, value);
      }
      else
      {
        OnModifiableRowChangesCanceling (index, value, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());
        
        if (_isEditNewRow)
        {
          _ownerControl.RemoveRowInternal (value);
          OnModifiableRowChangesCanceled (-1, value);
        }
        else
        {
          OnModifiableRowChangesCanceled (index, value);
        }
      }

      _ownerControl.EndEditDetailsModeCleanUp (_modifiableRowIndex.Value);
    }

    RemoveEditModeControls();
    _modifiableRowIndex = NaInt32.Null;
    _isEditNewRow = false;
  }

  public void EndListEditMode (bool saveChanges, BocColumnDefinition[] oldColumns)
  {
    if (! IsListEditModeActive)
      return;

    EnsureEditModeRestored (oldColumns);

    if (! _ownerControl.IsReadOnly)
    {
      IBusinessObject[] values = 
          (IBusinessObject[]) ArrayUtility.Convert (_ownerControl.Value, typeof (IBusinessObject));

      if (saveChanges)
      {
        for (int i = 0; i < _rows.Length; i++)
          OnModifiableRowChangesSaving (i, values[i], _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

        bool isValid = Validate();
        if (! isValid)
          return;

        _ownerControl.IsDirty = IsDirty();

        for (int i = 0; i < _rows.Length; i++)
          _rows[i].GetDataSource().SaveValues (false);

        for (int i = 0; i < _rows.Length; i++)
          OnModifiableRowChangesSaved (i, values[i]);
      }
      else
      {
        for (int i = 0; i < _rows.Length; i++)
          OnModifiableRowChangesCanceling (i, values[i], _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

        //if (_isEditNewRow)
        //{
        //  IBusinessObject editedBusinessObject = values[_modifiableRowIndex.Value];
        //  RemoveRow (_modifiableRowIndex.Value);
        //  OnRowEditModeCanceled (-1, editedBusinessObject);
        //}
        //else
        //{
        for (int i = 0; i < _rows.Length; i++)
          OnModifiableRowChangesCanceled (i, values[i]);
        //}
      }

      _ownerControl.EndListEditModeCleanUp();
    }

    RemoveEditModeControls();
    _isListEditModeActive = false;
  }


  private void CreateEditModeControls (BocColumnDefinition[] columns)
  {
    if (IsEditDetailsModeActive)
    {
      IBusinessObject value = (IBusinessObject) _ownerControl.Value[_modifiableRowIndex.Value];
      PopulateModifiableRows (new IBusinessObject[] {value}, columns);
    }
    else if (IsListEditModeActive)
    {
      PopulateModifiableRows (_ownerControl.Value, columns);
    }
  }

  private void PopulateModifiableRows (IList values, BocColumnDefinition[] columns)
  {
    EnsureChildControls();

    _rows = new ModifiableRow[values.Count];
    Controls.Clear();

    for (int i = 0; i < values.Count; i++)
    {
      ModifiableRow row = CreateModifiableRow (i, (IBusinessObject) values[i], columns);

      _rows[i] = row;
      Controls.Add (row);
    }
  }

  private ModifiableRow CreateModifiableRow (int rowIndex, IBusinessObject value, BocColumnDefinition[] columns)
  {
    ModifiableRow row = new ModifiableRow (_ownerControl);
    row.ID = ID + "_Row" + rowIndex.ToString();

    ModifiableRowDataSourceFactory dataSourceFactory = _ownerControl.GetEditModeDataSourceFactory();
    if (dataSourceFactory != null)
      row.DataSourceFactory = dataSourceFactory;

    ModifiableRowControlFactory controlFactory = _ownerControl.GetEditModeControlFactory();
    if (controlFactory != null)
      row.ControlFactory = controlFactory;

    row.CreateControls (columns, value);

    return row;
  }

  private void LoadValues (bool interim)
  {
    for (int i = 0; i < _rows.Length; i++)
      _rows[i].GetDataSource().LoadValues (interim);
  }

  public void EnsureEditModeRestored (BocColumnDefinition[] oldColumns)
  {
    ArgumentUtility.CheckNotNull ("oldColumns", oldColumns);

    if (_isEditModeRestored)
      return;
    _isEditModeRestored = true;

    if (_ownerControl.Value == null)
    {
      throw new InvalidOperationException (string.Format (
          "Cannot restore edit mode: The BocList '{0}' does not have a Value.", _ownerControl.ID));
    }
    if (IsEditDetailsModeActive && _modifiableRowIndex.Value >= _ownerControl.Value.Count)
    {
      throw new InvalidOperationException (string.Format ("Cannot restore edit details mode: "
          + "The Value collection of the BocList '{0}' no longer contains the previously modified row.", 
          _ownerControl.ID));
    }

    CreateEditModeControls (oldColumns);
  }

  private void RemoveEditModeControls()
  {
    for (int i = 0; i < _rows.Length; i++)
      _rows[i].RemoveControls();
  }


  /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void AddRows (IBusinessObject[] businessObjects, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);
    ArgumentUtility.CheckNotNull ("columns", columns);

    _ownerControl.AddRowsInternal (businessObjects);

    if (_ownerControl.Value != null)
    {
      EnsureEditModeRestored (oldColumns);
      if (IsListEditModeActive)
      {
        int startIndex = _ownerControl.Value.Count - businessObjects.Length;
        ArrayList newRows = new ArrayList (businessObjects.Length);
        for (int i = startIndex; i < _ownerControl.Value.Count; i++)
        {
          ModifiableRow newRow = CreateModifiableRow (i, (IBusinessObject) _ownerControl.Value[i], columns);
          newRow.GetDataSource().LoadValues (false);
          Controls.Add (newRow);
          newRows.Add (newRow);
        }
        _rows = (ModifiableRow[]) ListUtility.AddRange (_rows, newRows, (CreateListMethod) null, true, true);
      }
    }
  }

  /// <summary> Adds the <paramref name="businessObject"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public int AddRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns, BocColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    ArgumentUtility.CheckNotNull ("columns", columns);
  
    int index = _ownerControl.AddRowInternal (businessObject);

    if (index != -1)
    {
      EnsureEditModeRestored (oldColumns);
      if (IsListEditModeActive)
      {
        ModifiableRow newRow = CreateModifiableRow (index, businessObject, columns);
        newRow.GetDataSource().LoadValues (false);
        Controls.Add (newRow);
        _rows = (ModifiableRow[]) ListUtility.AddRange (_rows, newRow, (CreateListMethod) null, true, true);
      }
    }

    return index;
  }

  /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void RemoveRows (IBusinessObject[] businessObjects)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

    if (_ownerControl.Value != null)
    {
      if (IsEditDetailsModeActive)
      {
        throw new InvalidOperationException (string.Format (
            "Cannot remove rows while the BocList '{0}' is in edit details mode. "
            + "Call EndEditDetailsMode() before removing the rows.",
            _ownerControl.ID));
      }
      else if (IsListEditModeActive)
      {
        int[] indices = ListUtility.IndicesOf (_ownerControl.Value, businessObjects, false);
        ArrayList rows = new ArrayList (indices.Length);
        foreach (int index in indices)
        {
          ModifiableRow row = _rows[index];
          Controls.Remove (row);
          row.RemoveControls();
          rows.Add (row);
        }
        _rows = (ModifiableRow[]) ListUtility.Remove (_rows, rows, (CreateListMethod) null, true);
        RefreshIDs();
      }
    }

    _ownerControl.RemoveRowsInternal (businessObjects);
  }

  /// <summary> Removes the <paramref name="businessObject"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  public void RemoveRow (IBusinessObject businessObject, BocColumnDefinition[] oldColumns)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);
    ArgumentUtility.CheckNotNull ("oldColumns", oldColumns);
    
    if (_ownerControl.Value != null)
    {
      if (IsEditDetailsModeActive)
      {
        throw new InvalidOperationException (string.Format (
          "Cannot remove a row while the BocList '{0}' is in edit details mode. "
          + "Call EndEditDetailsMode() before removing the row.",
          _ownerControl.ID));
      }  
      else if (IsListEditModeActive)
      {
        int index = ListUtility.IndexOf (_ownerControl.Value, businessObject);
        if (index != -1)
        {
          ModifiableRow row = _rows[index];
          Controls.Remove (row);
          row.RemoveControls();
          _rows = (ModifiableRow[]) ListUtility.Remove (_rows, row, (CreateListMethod) null, true);
        }
        RefreshIDs();
      }
    }

    _ownerControl.RemoveRowInternal (businessObject);
  }

  private void RefreshIDs()
  {
    for (int i = 0; i < Controls.Count; i++)
    {
      ModifiableRow row = (ModifiableRow) Controls[i];
      string newID = ID + "_Row" + i.ToString();
      if (row.ID != newID)
        row.ID = newID;
    }
  }


  /// <remarks>
  ///   Queried where the rendering depends on whether the list is in edit mode. 
  ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
  /// </remarks>
  public bool IsEditDetailsModeActive
  {
    get { return ! _modifiableRowIndex.IsNull; } 
  }

  /// <remarks>
  ///   Queried where the rendering depends on whether the list is in edit mode. 
  ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
  /// </remarks>
  public bool IsListEditModeActive
  {
    get { return _isListEditModeActive; } 
  }

  public NaInt32 ModifiableRowIndex
  {
    get { return _modifiableRowIndex; }
  }


  /// <summary>
  ///   Generates a <see cref="BocList.EditDetailsValidator"/>.
  /// </summary>
  /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
  public BaseValidator[] CreateValidators (IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

    if (! (IsListEditModeActive || IsEditDetailsModeActive) || ! _enableEditModeValidator)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];

    EditDetailsValidator editDetailsValidator = new EditDetailsValidator (_ownerControl);
    editDetailsValidator.ID = ID + "_ValidatorEditDetails";
    editDetailsValidator.ControlToValidate = ID;
    if (StringUtility.IsNullOrEmpty (_ownerControl.ErrorMessage))
    {
      if (IsEditDetailsModeActive)
      {
        editDetailsValidator.ErrorMessage = 
            resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.EditDetailsErrorMessage);
      }
      else if (IsListEditModeActive)
      {
        editDetailsValidator.ErrorMessage = 
            resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.ListEditModeErrorMessage);
     }
    }
    else
    {
      editDetailsValidator.ErrorMessage = _ownerControl.ErrorMessage;
    }
    validators[0] = editDetailsValidator;

    return validators;
  }

  /// <remarks>
  ///   Validators must be added to the controls collection after LoadPostData is complete.
  ///   If not, invalid validators will know that they are invalid without first calling validate.
  ///   the <see cref="FormGridManager"/> also generates the validators after the <c>OnLoad</c> or when
  ///   <see cref="FormGridManager.Validate"/> is called. Therefor the behaviors of the <c>BocList</c>
  ///   and the <c>FormGridManager</c> match.
  /// </remarks>
  public void EnsureValidatorsRestored()
  {
    if (IsEditDetailsModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
        _rows[i].EnsureValidatorsRestored();
    }
  }

  public bool Validate()
  {
    EnsureValidatorsRestored();

    bool isValid = true;
    
    if (IsEditDetailsModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
        isValid &= _rows[i].Validate();
    
      isValid &= _ownerControl.ValidateModifiableRowsInternal();
    }

    return isValid;
  }

  
  public void RenderTitleCellMarkers (HtmlTextWriter writer, BocColumnDefinition column, int columnIndex)
  {
    if (_showEditModeRequiredMarkers && IsRequired (columnIndex))
    {
      Image requriedFieldMarker = _ownerControl.GetRequiredMarker();
      requriedFieldMarker.RenderControl (writer);
      writer.Write (c_whiteSpace);
    }
  }

  public bool IsRequired (int columnIndex)
  {
    if (IsEditDetailsModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
      {
        if (_rows[i].IsRequired (columnIndex))
          return true;
      }
    }

    return false;
  }

  
  public bool IsDirty()
  {
    if (IsEditDetailsModeActive || IsListEditModeActive)
    {
      for (int i = 0; i < _rows.Length; i++)
      {
        if (_rows[i].IsDirty())
          return true;
      }
    }

    return false;
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
    if (IsEditDetailsModeActive || IsListEditModeActive)
    {
      StringCollection trackedIDs = new StringCollection();
      for (int i = 0; i < _rows.Length; i++)
        trackedIDs.AddRange (_rows[i].GetTrackedClientIDs());

      string[] trackedIDsArray = new string[trackedIDs.Count];
      trackedIDs.CopyTo (trackedIDsArray, 0);
      return trackedIDsArray;
    }
    else
    {
      return new string[0];
    }
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


  protected virtual void OnModifiableRowChangesSaving (
      int index,
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundModifiableWebControl[] controls)
  {
    BocListModifiableRowChangesEventHandler handler = 
        (BocListModifiableRowChangesEventHandler) Events[s_modifiableRowChangesSavingEvent];
    if (handler != null)
    {
      BocListModifiableRowChangesEventArgs e = 
          new BocListModifiableRowChangesEventArgs (index, businessObject, dataSource, controls);
      handler (_ownerControl, e);
    }
  }

  protected virtual void OnModifiableRowChangesSaved (int index, IBusinessObject businessObject)
  {
    BocListItemEventHandler handler = (BocListItemEventHandler) Events[s_modifiableRowChangesSavedEvent];
    if (handler != null)
    {
      BocListItemEventArgs e = new BocListItemEventArgs (index, businessObject);
      handler (_ownerControl, e);
    }
  }

  protected virtual void OnModifiableRowChangesCanceling (
      int index,
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundModifiableWebControl[] controls)
  {
    BocListModifiableRowChangesEventHandler handler = 
        (BocListModifiableRowChangesEventHandler) Events[s_modifiableRowChangesCancelingEvent];
    if (handler != null)
    {
      BocListModifiableRowChangesEventArgs e = 
          new BocListModifiableRowChangesEventArgs (index, businessObject, dataSource, controls);
      handler (_ownerControl, e);
    }
  }

  protected virtual void OnModifiableRowChangesCanceled (int index, IBusinessObject businessObject)
  {
    BocListItemEventHandler handler = (BocListItemEventHandler) Events[s_modifiableRowChangesCanceledEvent];
    if (handler != null)
    {
      BocListItemEventArgs e = new BocListItemEventArgs (index, businessObject);
      handler (_ownerControl, e);
    }
  }

  
  /// <summary> Is raised before the modified row is saved. </summary>
  public event BocListModifiableRowChangesEventHandler ModifiableRowChangesSaving
  {
    add { Events.AddHandler (s_modifiableRowChangesSavingEvent, value); }
    remove { Events.RemoveHandler (s_modifiableRowChangesSavingEvent, value); }
  }

  /// <summary> Is raised after the modified row's changes have been saved. </summary>
  public event BocListItemEventHandler ModifiableRowChangesSaved
  {
    add { Events.AddHandler (s_modifiableRowChangesSavedEvent, value); }
    remove { Events.RemoveHandler (s_modifiableRowChangesSavedEvent, value); }
  }

  /// <summary> Is raised before the modified row's changes are canceled. </summary>
  public event BocListModifiableRowChangesEventHandler ModifiableRowChangesCanceling
  {
    add { Events.AddHandler (s_modifiableRowChangesCancelingEvent, value); }
    remove { Events.RemoveHandler (s_modifiableRowChangesCancelingEvent, value); }
  }

  /// <summary> Is raised after the modified row's changes have been canceled. </summary>
  public event BocListItemEventHandler ModifiableRowChangesCanceled
  {
    add { Events.AddHandler (s_modifiableRowChangesCanceledEvent, value); }
    remove { Events.RemoveHandler (s_modifiableRowChangesCanceledEvent, value); }
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
}

}
