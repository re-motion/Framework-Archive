using System;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rubicon.Globalization;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

[ToolboxItem (false)]
public class ModifiableRow : PlaceHolder, INamingContainer
{
  // types

  // static members and constants

  private const string c_whiteSpace = "&nbsp;";

  // member fields

  private Controls.BocList _owner;

  private ModifiableRowDataSourceFactory _dataSourceFactory;
  private ModifiableRowControlFactory _editControlFactory;
  
  private IBusinessObjectReferenceDataSource _dataSource;

  private PlaceHolder _editControls = null;
  private PlaceHolder _validatorControls = null;

  private bool _isRowEditModeValidatorsRestored = false;
  private IBusinessObjectBoundModifiableWebControl[] _rowEditModeControls;

  // construction and disposing

  public ModifiableRow (Controls.BocList owner)
  {
    ArgumentUtility.CheckNotNull ("owner", owner);

    _owner = owner;
  }

  // methods and properties

  public Controls.BocList Owner
  {
    get { return _owner; }
  }

  public ModifiableRowDataSourceFactory DataSourceFactory
  {
    get 
    {
      return _dataSourceFactory; 
    }
    set 
    {
      ArgumentUtility.CheckNotNull ("value", value);
      _dataSourceFactory = value; 
    }
  }

  public ModifiableRowControlFactory EditControlFactory
  {
    get 
    {
      return _editControlFactory; 
    }
    set 
    {
      ArgumentUtility.CheckNotNull ("value", value);
      _editControlFactory = value; 
    }
  }

  public virtual void CreateControls (BocColumnDefinition[] columns, IBusinessObject value)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);
    ArgumentUtility.CheckNotNull ("value", value);

    EnsureFactories();

    CreatePlaceHolders (columns);

    _dataSource = _dataSourceFactory.Create (value);
    _rowEditModeControls = new IBusinessObjectBoundModifiableWebControl[columns.Length];

    for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
    {
      BocSimpleColumnDefinition simpleColumn = columns[idxColumns] as BocSimpleColumnDefinition;

      if (IsColumnModifiable (simpleColumn))
      {
        IBusinessObjectBoundModifiableWebControl control = _editControlFactory.Create (simpleColumn, idxColumns);

        if (control != null)
        {
          control.ID = idxColumns.ToString();
          control.DataSource = _dataSource;
          control.Property = simpleColumn.PropertyPath.LastProperty;
          SetEditControl (idxColumns, control);

          _rowEditModeControls[idxColumns] = control;
        }
      }
    }    
  }

  protected void EnsureFactories ()
  {
    if (_dataSourceFactory == null)
      _dataSourceFactory = new ModifiableRowDataSourceFactory ();
    if (_editControlFactory == null)
      _editControlFactory = new ModifiableRowControlFactory ();
  }

  protected void CreatePlaceHolders (BocColumnDefinition[] columns)
  {
    RemoveControls();

    _editControls = new PlaceHolder();
    Controls.Add (_editControls);

    _validatorControls = new PlaceHolder();
    Controls.Add (_validatorControls);

    for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
    {
      _editControls.Controls.Add (new PlaceHolder());
      _validatorControls.Controls.Add (new PlaceHolder());
    }
  }

  protected bool IsColumnModifiable (BocSimpleColumnDefinition column)
  {
    if (column == null)
      return false;
    if (column.IsReadOnly)
      return false;
    if (column.PropertyPath.Properties.Length > 1)
      return false;
    return true;
  }

  public void RemoveControls()
  {
    Controls.Clear();
    _editControls = null;
    _validatorControls = null;
  }

  public IBusinessObjectReferenceDataSource GetDataSource()
  {
    return _dataSource;
  }

  protected void SetEditControl (int index, IBusinessObjectBoundModifiableWebControl control)
  {
    ArgumentUtility.CheckNotNullAndType ("control", control, typeof (Control));

    ControlCollection cellControls = GetEditControls (index);
    if (cellControls.Count > 0)
      cellControls.Clear();
    cellControls.Add ((Control) control);
  }

  private ControlCollection GetEditControls (int columnIndex)
  {
    if (columnIndex < 0 || columnIndex >= _editControls.Controls.Count) throw new ArgumentOutOfRangeException ("columnIndex");

    return _editControls.Controls[columnIndex].Controls;
  }

  public IBusinessObjectBoundModifiableWebControl GetEditControl (int columnIndex)
  {
    ControlCollection controls = GetEditControls (columnIndex);
    return (IBusinessObjectBoundModifiableWebControl) controls[0];
  }

  public bool HasEditControls ()
  {
    return _editControls != null;
  }

  public bool HasEditControl (int columnIndex)
  {
    if (HasEditControls())
      return GetEditControls (columnIndex).Count > 0;
    else
      return false;
  }

  protected void AddToValidators (int columnIndex, BaseValidator[] validators)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("validators", validators);

    ControlCollection cellValidators = GetValidators (columnIndex);
    for (int i = 0; i < validators.Length; i++)
      cellValidators.Add (validators[i]);
  }

  public ControlCollection GetValidators (int columnIndex)
  {
    if (columnIndex < 0 || columnIndex >= _validatorControls.Controls.Count) throw new ArgumentOutOfRangeException ("columnIndex");

    return _validatorControls.Controls[columnIndex].Controls;
  }

  public bool HasValidators ()
  {
    return _validatorControls != null;
  }

  public bool HasValidators (int columnIndex)
  {
    if (HasValidators())
      return GetValidators (columnIndex).Count > 0;
    else
      return false;
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
    if (_isRowEditModeValidatorsRestored)
      return;
    _isRowEditModeValidatorsRestored = true;
    if (! HasEditControls())
      return;

    for (int i = 0; i < _editControls.Controls.Count; i++)
      CreateValidators (i);
  }

  public void CreateValidators (int columnIndex)
  {
    if (HasEditControl (columnIndex))
    {
      IBusinessObjectBoundModifiableWebControl editControl = GetEditControl (columnIndex);
      BaseValidator[] validators = editControl.CreateValidators();
      if (validators != null)
        AddToValidators (columnIndex, validators);
    }
  }

  public bool Validate ()
  {
    bool isValid = true;

    if (HasValidators())
    {
      for (int i = 0; i < _editControls.Controls.Count; i++)
        isValid &= Validate (i);
    }

    return isValid;
  }

  public bool Validate (int columnIndex)
  {
    bool isValid = true;

    if (HasValidators (columnIndex))
    {
      ControlCollection cellValidators = GetValidators (columnIndex);
      for (int i = 0; i < cellValidators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) cellValidators[i];
        validator.Validate();
        isValid &= validator.IsValid;
      }
    }

    return isValid;
  }

  public string[] GetTrackedClientIDs()
  {
    StringCollection trackedIDs = new StringCollection();

    if (HasEditControls())
    {
      for (int i = 0; i < _editControls.Controls.Count; i++)
        trackedIDs.AddRange (GetTrackedClientIDs (i));
    }

    string[] trackedIDsArray = new string[trackedIDs.Count];
    trackedIDs.CopyTo (trackedIDsArray, 0);
    return trackedIDsArray;
  }

  public string[] GetTrackedClientIDs (int columnIndex)
  {
    if (HasEditControl (columnIndex))
      return GetEditControl (columnIndex).GetTrackedClientIDs();
    else
      return new string[0];
  }

  public bool IsDirty()
  {
    if (HasEditControls())
    {
      for (int i = 0; i < _editControls.Controls.Count; i++)
      {
        if (IsDirty (i))
          return true;
      }
    }
    return false;
  }
  
  public bool IsDirty (int columnIndex)
  {
    if (HasEditControl (columnIndex))
      return GetEditControl (columnIndex).IsDirty;
    else
      return false;
  }

  public IBusinessObjectBoundModifiableWebControl[] GetEditControlsAsArray()
  {
    return (IBusinessObjectBoundModifiableWebControl[]) _rowEditModeControls.Clone ();
  }

  public void RenderSimpleColumnCellEditModeControl (
      HtmlTextWriter writer, 
      BocSimpleColumnDefinition column,
      IBusinessObject businessObject,
      int columnIndex,
      EditDetailsValidator editDetailsValidator,
      bool showEditDetailsValidationMarkers,
      bool disableEditDetailsValidationMessages) 
  {
    if (! HasEditControl (columnIndex))
      return;
  
    ControlCollection validators = GetValidators (columnIndex);

    IBusinessObjectBoundModifiableWebControl editModeControl = _rowEditModeControls[columnIndex];
    
    CssStyleCollection editModeControlStyle = null;
    bool isEditModeControlWidthEmpty = true;
    if (editModeControl is WebControl)
    {
      editModeControlStyle = ((WebControl) editModeControl).Style;
      isEditModeControlWidthEmpty = ((WebControl) editModeControl).Width.IsEmpty;
    }
    else if (editModeControl is System.Web.UI.HtmlControls.HtmlControl)
    {
      editModeControlStyle = ((System.Web.UI.HtmlControls.HtmlControl) editModeControl).Style;
    }
    if (editModeControlStyle != null)
    {
      bool enforceWidth = 
             column.EnforceWidth 
          && ! column.Width.IsEmpty
          && column.Width.Type != UnitType.Percentage;

      if (enforceWidth)
        editModeControlStyle["width"] = column.Width.ToString();
      else if (StringUtility.IsNullOrEmpty (editModeControlStyle["width"]) && isEditModeControlWidthEmpty)
        editModeControlStyle["width"] = "100%";
      if (StringUtility.IsNullOrEmpty (editModeControlStyle["vertical-align"]))
        editModeControlStyle["vertical-align"] = "middle";
    }
    
    if (showEditDetailsValidationMarkers)
    {
      bool isCellValid = true;
      Image validationErrorMarker = _owner.GetValidationErrorMarker();
      
      for (int i = 0; i < validators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) validators[i];
        isCellValid &= validator.IsValid;
        if (! validator.IsValid)
        {
          if (StringUtility.IsNullOrEmpty (validationErrorMarker.ToolTip))
          {
            validationErrorMarker.ToolTip = validator.ErrorMessage;
          }
          else
          {
            validationErrorMarker.ToolTip += "\r\n";
            validationErrorMarker.ToolTip += validator.ErrorMessage;
          }
        }
      }
      if (! isCellValid)
      {
        validationErrorMarker.RenderControl (writer);
        writer.Write (c_whiteSpace);

        if (editModeControlStyle != null)
          editModeControlStyle["width"] = "80%";
      }
    }

    editModeControl.RenderControl (writer);

    for (int i = 0; i < validators.Count; i++)
    {
      BaseValidator validator = (BaseValidator) validators[i];
      if (   editDetailsValidator == null 
          || disableEditDetailsValidationMessages)
      {
        validator.Display = ValidatorDisplay.None;
        validator.EnableClientScript = false;
      }
      else
      {
        validator.Display = editDetailsValidator.Display;
        validator.EnableClientScript = editDetailsValidator.EnableClientScript;
      }

      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      validator.RenderControl (writer);
      writer.RenderEndTag();

      if (   ! validator.IsValid 
          && validator.Display == ValidatorDisplay.None
          && ! disableEditDetailsValidationMessages)
      {
        if (! StringUtility.IsNullOrEmpty (validator.CssClass))
          writer.AddAttribute (HtmlTextWriterAttribute.Class, validator.CssClass);
        else
          writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassEditDetailsValidationMessage);
        writer.RenderBeginTag (HtmlTextWriterTag.Div);
        writer.Write (validator.ErrorMessage); // Do not HTML encode.
        writer.RenderEndTag();
      }
    }
  }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s edit details validation messages. </summary>
  /// <remarks>
  ///   <para> Class: <c>bocListEditDetailsValidationMessage</c> </para>
  ///   <para> Only applied if the <see cref="EditDetailsValidator"/> has no CSS-class of its own. </para>
  ///   </remarks>
  protected virtual string CssClassEditDetailsValidationMessage
  { get { return "bocListEditDetailsValidationMessage"; } }
  
}

}
