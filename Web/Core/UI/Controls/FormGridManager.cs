using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;
using System.Reflection;

using log4net;

using Rubicon.Web.UI.Utilities;
using Rubicon;
using Rubicon.Utilities;
using Rubicon.Web.UI.Globalization;
using Rubicon.Globalization;

namespace Rubicon.Web.UI.Controls
{

/// <summary> Transforms one or more tables into form grids. </summary>
/// <include file='doc\include\FormGridManager.xml' path='FormGridManager/Class/*' />
[ToolboxData("<{0}:FormGridManager runat='server' visible='true'></{0}:FormGridManager>")]
[ToolboxItemFilter("System.Web.UI")]
public class FormGridManager : WebControl, IResourceDispatchTarget
{
  // types

  /// <summary> A list of possible images/icons displayed in the Form Grid. </summary>
  /// <remarks> The symbol names map directly to the image's file names. </remarks>
  protected enum FormGridImage
  {
    /// <summary> A blank image to be used as a spacer. </summary>
    Spacer,
    
    /// <summary> Used for field's with a mandatory input. </summary>
    RequiredField,
    
    /// <summary> Used to indicate a help link. </summary>
    Help,
    
    /// <summary> Used if an entered value does not validate. </summary>
    ValidationError
  }

  /// <summary> A list of form grid manager wide resources. </summary>
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Type, Enum). </remarks>
  protected enum ResourceIdentifiers
  {
    /// <summary>The alternate text for the required icon. Defaults to '*'.</summary>
    RequiredFieldAlternateText,
    /// <summary>The alternate text for the help icon. Defaults to '?'.</summary>
    HelpAlternateText,
    /// <summary>The alternate text for the validation error icon. Defaults to '!'.</summary>
    ValidationErrorInfoAlternateText,
    /// <summary>The tool tip text for the required icon.</summary>
    RequiredFieldTitle,
    /// <summary>The tool tip text for the help icon.</summary>
    HelpTitle,
    //  Not used, title always set to message
    //  ValidationErrorInfoTitle,
  }
  /// <summary>
  ///   Wrapper class for a single HtmlTable plus the additional information
  ///   added through the <see cref="FormGridManager"/>.
  /// </summary>
  protected class FormGrid
  {
    /// <summary> Delegate for creating an array of FormGridRow objects from an Html Table. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/CreateRows/*' />
    public delegate FormGridRow[] CreateRows (
      FormGrid formGrid, 
      int labelsColumn, 
      int controlsColumn);

    /// <summary> The <c>HtmlTable</c> used as a base for form grid. </summary>
    private HtmlTable _table;

    /// <summary> The <see cref="FormGridRow"/> collection for this <c>FormGrid</c>. </summary>
    private FormGridRowCollection _rows;

    /// <summary> The column normally containing the labels. </summary>
    private int _defaultLabelsColumn;

    /// <summary> The column normally containing the controls. </summary>
    private int _defaultControlsColumn;

    /// <summary> Simple contructor. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/Constructor/*' />
    public FormGrid (
      HtmlTable table, 
      CreateRows createRows,
      int defaultLabelsColumn, 
      int defaultControlsColumn)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("createRows", createRows);

      _table = table;
      _defaultLabelsColumn = defaultLabelsColumn;
      _defaultControlsColumn = defaultControlsColumn;
      _rows = new FormGridRowCollection (
        this,
        createRows (this, defaultLabelsColumn, defaultControlsColumn));
    }

    /// <summary> Uses the wrapped HtmlTable's ID's GetHashCode method. </summary>
    /// <returns> The hash code </returns>
    public override int GetHashCode()
    {
      return _table.ID.GetHashCode ();
    }

    /// <summary> Compares by reference. </summary>
    /// <param name="obj"> The object to compare with. </param>
    /// <returns> <see langname="true"/> if a match. </returns>
    public override bool Equals(object obj)
    {
      return object.ReferenceEquals (this, obj);
    }

    /// <summary>
    ///   Returns all <see cref="ValidationError"/> objects defined in the 
    ///   <see cref="FormGridRow"/> objects collection.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/GetValidationErrors/*' />
    public virtual ValidationError[] GetValidationErrors()
    {
      ArrayList validationErrorList = new ArrayList();

      foreach (FormGridRow row in _rows)
      {
        if (row.Type == FormGridRowType.DataRow)
          validationErrorList.AddRange(row.ValidationErrors);
      }
      
      return (ValidationError[])validationErrorList.ToArray (typeof (ValidationError));
    }

    /// <summary>
    ///   Searches through the <see cref="FormGridRow"/> objects collection for a validation error.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/HasValidationErrors/*' />
    public virtual bool HasValidationErrors()
    {
      foreach (FormGridRow row in _rows)
      {
        if (    row.Type == FormGridRowType.DataRow
            &&  row.ValidationErrors.Length > 0)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    ///   Searches through the <see cref="FormGridRow"/> objects collection for a validation markers.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/HasValidationMarkers/*' />
    public virtual bool HasValidationMarkers()
    {
      foreach (FormGridRow row in _rows)
      {
        if (    row.Type == FormGridRowType.DataRow 
            &&  row.ValidationMarker != null)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    ///   Searches through the <see cref="FormGridRow"/> objects collection for a required markers.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/HasRequiredMarkers/*' />
    public virtual bool HasRequiredMarkers()
    {
      foreach (FormGridRow row in _rows)
      {
        if (    row.Type == FormGridRowType.DataRow
            &&  row.RequiredMarker != null)
        {
            return true;
        }
      }

      return false;
    }

    /// <summary> Searches through the <see cref="FormGridRow"/> objects collection for a help providers. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/HasHelpProviders/*' />
    public virtual bool HasHelpProviders()
    {
      foreach (FormGridRow row in _rows)
      {
        if (    row.Type == FormGridRowType.DataRow
            &&  row.HelpProvider != null)
        {
            return true;
        }
      }

      return false;
    }

    /// <summary> Build the ID collection for this form grid. </summary>
    public virtual void BuildIDCollection()
    {
      foreach (FormGridRow row in _rows)
        row.BuildIDCollection();
    }

    /// <summary>
    ///   Searches for a <see cref="FormGridRow"/> containing the specified <paramref name="ID"/>.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/FormGridRow/*' />
    public FormGridRow GetRowForID (string id)
    {
      if (id == null || id == string.Empty)
        return null;

      foreach (FormGridRow row in _rows)
      {
        if (row.ContainsControlWithID (id))
          return row;
      }

      return null;
    }

    /// <summary>
    ///   Inserts a <see cref="FormGridRow"/> at the position specified by 
    ///   <paramref name="positionInFormGrid"/> and <paramref name="relatedRowID"/>.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/InsertNewFormGridRow/*' />
    public void InsertNewFormGridRow (
      FormGridRow newFormGridRow,
      string relatedRowID,
      FormGridRowPrototype.RowPosition positionInFormGrid)
    {
      ArgumentUtility.CheckNotNull ("newFormGridRow", newFormGridRow);

      FormGridRow relatedRow = GetRowForID (relatedRowID);

      //  Not found, append to form grid instead of inserting at position of related form grid row
      if (relatedRow == null)
      {
        s_log.Warn ("Could not find control '" + relatedRowID + "' inside FormGrid (HtmlTable) '" + _table.ID + "' in naming container '" + _table.NamingContainer.GetType().FullName + "' on page '" + _table.Page.ToString() + "'.");

        //  append html table rows
        foreach (HtmlTableRow newHtmlTableRow in newFormGridRow.HtmlTableRows)
          _table.Rows.Add (newHtmlTableRow);
        
        //  append form grid row
        ((IList)Rows).Add (newFormGridRow);
      }
        //  Insert after the related form grid row
      else if (positionInFormGrid == FormGridRowPrototype.RowPosition.AfterRowWithID)
      {
        //  Find insertion postion for the html table rows

        int idxHtmlTableRow = 0;

        HtmlTableRow lastReleatedTableRow = 
          relatedRow.HtmlTableRows[relatedRow.HtmlTableRows.Count - 1];

        //  Find position in html table
        for (; idxHtmlTableRow < _table.Rows.Count; idxHtmlTableRow++)
        {
          if (_table.Rows[idxHtmlTableRow] == lastReleatedTableRow)
          {    
            //  We want to isnert after the current position
            idxHtmlTableRow++;

            break;
          }
        }

        //  Insert the new html table rows
        foreach (HtmlTableRow newHtmlTableRow in newFormGridRow.HtmlTableRows)
        {
          _table.Rows.Insert (idxHtmlTableRow, newHtmlTableRow);
          idxHtmlTableRow++;
        }


        //  Insert row into Form Grid

        int idxFormGridRow = ((IList)Rows).IndexOf (relatedRow);
        //  After the index of the related row
        idxFormGridRow++;
        ((IList)Rows).Insert (idxFormGridRow, newFormGridRow);
      }
        //  Insert before the related form grid row
      else if (positionInFormGrid == FormGridRowPrototype.RowPosition.BeforeRowWithID)
      {
        //  Find insertion postion for the html table rows

        int idxHtmlTableRow = 0;

        HtmlTableRow firstReleatedTableRow = relatedRow.HtmlTableRows[0];

        //  Find position in html table
        for (; idxHtmlTableRow < _table.Rows.Count; idxHtmlTableRow++)
        {
          if (_table.Rows[idxHtmlTableRow] == firstReleatedTableRow)
            break;
        }

        //  Insert the new html table rows
        foreach (HtmlTableRow newHtmlTableRow in newFormGridRow.HtmlTableRows)
        {
          _table.Rows.Insert (idxHtmlTableRow, newHtmlTableRow);
          idxHtmlTableRow++;
        }


        //  Insert row into Form Grid
        
        int idxFormGridRow = ((IList)Rows).IndexOf (relatedRow);
        //  Before the related row
        ((IList)Rows).Insert (idxFormGridRow, newFormGridRow);
      }

      newFormGridRow.BuildIDCollection ();
    }

    /// <summary> The <see cref="HtmlTable"/> used as base for the form grid. </summary>
    public HtmlTable Table
    {
      get { return _table; }
    }

    /// <summary> The <see cref="FormGridRowCollection"/> for this <c>FormGrid</c>. </summary>
    public FormGridRowCollection Rows
    {
      get { return _rows; }
    }

    /// <summary> Can be used to store the initial value for the labels column. </summary>
    public int DefaultLabelsColumn
    {
      get { return _defaultLabelsColumn; }
      set { _defaultLabelsColumn = value; }
    }

    /// <summary> Can be used to store the initial value for the controls column. </summary>
    public int DefaultControlsColumn
    {
      get { return _defaultControlsColumn; }
      set { _defaultControlsColumn = value; }
    }
}

  /// <summary> A collection of <see cref="FormGridRow"/> objects. </summary>
  protected sealed class FormGridRowCollection : CollectionBase
  {
    /// <summary> The <see cref="FormGrid"/> to which this collection belongs to. </summary>
    private FormGrid _ownerFormGrid;

    /// <summary> Simple constructor. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRowCollection/Constructor/*' />
    public FormGridRowCollection (FormGrid ownerFormGrid, FormGridRow[] formGridRows)
    {
      ArgumentUtility.CheckNotNull ("formGridRows", formGridRows);
      ArgumentUtility.CheckNotNull ("ownerFormGrid", ownerFormGrid);

      _ownerFormGrid = ownerFormGrid;

      for (int index = 0; index < formGridRows.Length; index++)
      {
        if (formGridRows[index] == null)
          throw new ArgumentNullException ("formGridRows[" + index + "]");

        formGridRows[index]._formGrid = _ownerFormGrid;
      }

      InnerList.AddRange (formGridRows);
    }

    /// <summary> A read only indexer for the <see cref="FormGridRow"/> objects. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRowCollection/Indexer/*' />
    public FormGridRow this [int index]
    {
      get
      {
        if (index < 0 || index >= InnerList.Count) throw new ArgumentOutOfRangeException ("index");

        return (FormGridRow)InnerList[index];
      }
    }

    /// <summary> Allows only the insertion of objects of type of <see cref="FormGridRow"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRowCollection/OnInsert/*' />
    protected override void OnInsert(int index, object value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      
      FormGridRow formGridRow = value as FormGridRow;
      if (formGridRow == null) throw new ArgumentTypeException ("value", typeof (FormGridRow), value.GetType());
      
      formGridRow._formGrid = _ownerFormGrid;
      base.OnInsert (index, value);
    }

  }

  /// <summary>
  ///   Wrapper class for one or more <see cref="HtmlTableRows"/> forming a logical row in the 
  ///   <see cref="FormGrid"/> object's base <see cref="HtmlTable"/>.
  /// </summary>
  protected class FormGridRow
  {
    /// <summary> The <see cref="FormGrid"/> instance this <c>FormGridRow</c> is a part of. </summary>
    internal FormGrid _formGrid;

    /// <summary> The <see cref="HtmlTableRow"/> collection for this <c>FormGridRow</c>. </summary>
    private ReadOnlyHtmlTableRowCollection _htmlTableRows;

    /// <summary> The type of this <c>FormGridRow</c>. </summary>
    private FormGridRowType _type;

    /// <summary> <see langword="true"/> if the row sould be rendered. </summary>
    private bool _visible;

    /// <summary> The <c>ValidationError</c> objects for this <c>FormGridRow</c>. </summary>
    private ValidationError[] _validationErrors;

    /// <summary> The validation marker for this <c>FormGridRow</c>. </summary>
    private Control _validationMarker;

    /// <summary>The required marker for this <c>FormGridRow</c>. </summary>
    private Control _requiredMarker;

    /// <summary> The help provider for this <c>FormGridRow</c>. </summary>
    private Control _helpProvider;

    /// <summary> The index of the row containing the labels cell. </summary>
    private int _labelsRowIndex;

    /// <summary> The index of the row containing the controls cell. </summary>
    private int _controlsRowIndex;
 
    /// <summary> The index of the column normally containing the labels cell. </summary>
    private int _labelsColumn;

    /// <summary> The index of the column normally containing the controls cell. </summary>
    private int _controlsColumn;

    /// <summary> The cell containing the labels. </summary>
    private HtmlTableCell _labelsCell;

    /// <summary> The cell containing the controls. </summary>
    private HtmlTableCell _controlsCell;

    /// <summary>
    ///   The cell used as a place holder if the controls cell is not at the standard position.
    /// </summary>
    private HtmlTableCell _controlsCellDummy;

    /// <summary> The cell containing the markers. </summary>
    private HtmlTableCell _markersCell;

    /// <summary> The cell containing the validation messages. </summary>
    private HtmlTableCell _validationMessagesCell;

    /// <summary>
    ///   The cell used as a place holder if the validation message cell is not at the standard
    ///   position.
    /// </summary>
    private HtmlTableCell _validationMessagesCellDummy;

    /// <summary> The Web.UI.Controls in this <see cref="FormGridRow"/>, using the ID as key. </summary>
    private Hashtable _controls;

    /// <summary> Simple contructor. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/Constructor/*' />
    public FormGridRow (
      HtmlTableRow[] htmlTableRows,
      FormGridRowType type, 
      int labelsColumn, 
      int controlsColumn)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlTableRows", htmlTableRows);

      _htmlTableRows = new ReadOnlyHtmlTableRowCollection(htmlTableRows);
      _type = type;
      _validationErrors = new ValidationError[]{};
      _labelsColumn = labelsColumn;
      _controlsColumn = controlsColumn;
      _visible = true;
      _controls = new Hashtable(0);

      for (int index = 0; index < htmlTableRows.Length; index++)
      {
        if (htmlTableRows[index] == null)
          throw new ArgumentNullException ("htmlTableRows[" + index + "]");
      }
    }

    /// <summary> Set the labels cell for this <see cref="FormGridRow"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/SetLabelsCell/*' />
    public virtual HtmlTableCell SetLabelsCell (int rowIndex, int cellIndex)
    {
      CheckCellRange (rowIndex, cellIndex);
      _labelsRowIndex = rowIndex;
      _labelsCell = _htmlTableRows[rowIndex].Cells[cellIndex];
      return _labelsCell;
    }

    /// <summary> Set the controls cell for this <see cref="FormGridRow"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/SetControlsCell/*' />
    public virtual HtmlTableCell SetControlsCell (int rowIndex, int cellIndex)
    {
      CheckCellRange (rowIndex, cellIndex);
      _controlsRowIndex = rowIndex;
      _controlsCell = _htmlTableRows[rowIndex].Cells[cellIndex];
      return _controlsCell;
    }

    /// <summary> Set the controls cell dummy for this <see cref="FormGridRow"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/SetControlsCellDummy/*' />
    public virtual HtmlTableCell SetControlsCellDummy (int rowIndex, int cellIndex)
    {
      CheckCellRange (rowIndex, cellIndex);
      _controlsCellDummy = _htmlTableRows[rowIndex].Cells[cellIndex];
      return _controlsCellDummy;
    }

    /// <summary> Set the markers cell for this <see cref="FormGridRow"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/SetMarkersCell/*' />
    public virtual HtmlTableCell SetMarkersCell (int rowIndex, int cellIndex)
    {
      CheckCellRange (rowIndex, cellIndex);
      _markersCell = _htmlTableRows[rowIndex].Cells[cellIndex];
      return _markersCell;
    }

    /// <summary> Set the validation messages cell for this <see cref="FormGridRow"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/SetValidationMessagesCell/*' />
    public virtual HtmlTableCell SetValidationMessagesCell (int rowIndex, int cellIndex)
    {
      CheckCellRange (rowIndex, cellIndex);
      _validationMessagesCell = _htmlTableRows[rowIndex].Cells[cellIndex];
      return _validationMessagesCell;
    }

    /// <summary>Set the labels validation messages cell dummy for this <see cref="FormGridRow"/>.</summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/SetValidationMessagesCellDummy/*' />
    public virtual HtmlTableCell SetValidationMessagesCellDummy (int rowIndex, int cellIndex)
    {
      CheckCellRange (rowIndex, cellIndex);
      _validationMessagesCellDummy = _htmlTableRows[rowIndex].Cells[cellIndex];
      return _validationMessagesCellDummy;
    }

    /// <summary> Checks if the indices are inside the bounds. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/CheckCellRange/*' />
    private void CheckCellRange (int rowIndex, int cellIndex)
    {
      if (rowIndex >= _htmlTableRows.Count) throw new ArgumentOutOfRangeException ("rowIndex", "Specified argument was out of the range of valid values.");
      if (rowIndex < 0) throw new ArgumentOutOfRangeException ("rowIndex", "Specified argument was out of the range of valid values.");

      if (cellIndex >= _htmlTableRows[rowIndex].Cells.Count) throw new ArgumentOutOfRangeException ("cellIndex", "Specified argument was out of the range of valid values.");
      if (cellIndex < 0) throw new ArgumentOutOfRangeException ("cellIndex", "Specified argument was out of the range of valid values.");
    }

    /// <summary>
    ///   Fills a <see cref="Hashtable"/> with the controls contained in this 
    ///   <see cref="FormGridRow"/>, using their ID as a key.
    /// </summary>
    /// <remarks> Considers only controls where <see cref="Control.ID"/> is set.</remarks>
    public virtual void BuildIDCollection()
    {
      //  Assume an average of 2 controls per cell
      _controls = new Hashtable (2 * _htmlTableRows.Count * _htmlTableRows[0].Cells.Count);

      foreach (HtmlTableRow row in _htmlTableRows)
      {
        foreach (HtmlTableCell cell in row.Cells)
        {
          foreach (Control control in cell.Controls)
          {
            if (control.ID != null && control.ID != string.Empty)
              _controls[control.ID] = control;
          }
        }
      }
    }

    /// <summary> Returns the control with the specified ID or <see langword="null"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/GetControlForID/*' />
    public virtual Control GetControlForID (string id)
    {
      StringUtility.IsNullOrEmpty (id);
      return (Control)_controls[id];
    }

    /// <summary> 
    ///   Returns <see langword="true"/> if the control with the specified ID is contained 
    ///   in the <see cref="FormGridRow"/>.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ContainsControlWithID/*' />
    public virtual bool ContainsControlWithID (string id)
    {
      return GetControlForID (id) != null;
    }

    /// <summary>
    ///   Test's whether this <see cref="FormGridRow"/> contains visible controls or if 
    ///   it's own <see cref="Visible"/> property is set to false.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/CheckVisibility/*' />
    public virtual bool CheckVisibility()
    {
      if (!_visible)
        return false;

      foreach (HtmlTableRow row in _htmlTableRows)
      {
        foreach (HtmlTableCell cell in row.Cells)
        {
          foreach (Control control in cell.Controls)
          {
            if (control.Visible)
              return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    ///   Sets the <see cref="FormGridRow"/> and it's contained <see cref="HtmlTableRow"/> objects
    ///   invisible.
    /// </summary>
    public virtual void Hide()
    {
      _visible = false;

      foreach (HtmlTableRow row in _htmlTableRows)
        row.Visible = false;
    }

    /// <summary>
    ///   Sets the <see cref="FormGridRow"/> and it's contained <see cref="HtmlTableRow"/> 
    ///   visible.
    /// </summary>
   public virtual void Show()
    {
      _visible = true;

      foreach (HtmlTableRow row in _htmlTableRows)
        row.Visible = true;
    }

    /// <summary>
    ///   The <see cref="FormGrid"/> instance of which this <c>FormGridRow</c> is a part of.
    /// </summary>
    public FormGrid FormGrid
    {
      get { return _formGrid; }
    }

    /// <summary> The <see cref="HtmlTableRow"/> collection for this <c>FormGridRow</c>. </summary>
    public ReadOnlyHtmlTableRowCollection HtmlTableRows
    {
      get { return _htmlTableRows; }
    }

    /// <summary> The type of this <c>FormGridRow</c>. </summary>
    public FormGridRowType Type
    {
      get { return _type; }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the contained <see cref="HtmlTableRows"/>
    ///   should be rendered on as Ui on the page.
    /// </summary>
    public bool Visible
    {
      get { return _visible; }
      set { _visible = value; }
    }

    /// <summary> The <c>ValidationError</c> objects for this <c>FormGridRow</c>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ValidationErrors/remarks' />
    public ValidationError[] ValidationErrors
    {
      get
      {
        return _validationErrors; 
      }
      set
      { 
        ArgumentUtility.CheckNotNull ("value", value);
        _validationErrors = value; 
      }
    }

    /// <summary> The validation marker for this <c>FormGridRow</c>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ValidationMarker/*' />
    public Control ValidationMarker
    {
      get { return _validationMarker; }
      set { _validationMarker = value; }
    }

    /// <summary> The required marker for this <c>FormGridRow</c>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/RequiredMarker/*' />
    public Control RequiredMarker
    {
      get { return _requiredMarker; }
      set { _requiredMarker = value; }
    }

    /// <summary> The help provider for this <c>FormGridRow</c>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/HelpProvider/*' />
    public Control HelpProvider
    {
      get { return _helpProvider; }
      set { _helpProvider = value; }
   }

    /// <summary> The index of the row containing the labels cell. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsRowIndex/*' />
    public int LabelsRowIndex
    {
      get { return _labelsRowIndex; }
    }

    /// <summary> The index of the row containing the controls cell. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsRowIndex/*' />
    public int ControlsRowIndex
    {
      get { return _controlsRowIndex; }
    }

    /// <summary> The index of the column normally containing the labels cell. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsColumn/*' />
    public int LabelsColumn
    {
      get { return _labelsColumn; }
      set { _labelsColumn = value; }
    }

    /// <summary> The index of the column normally containing the controls cell. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsColumn/*' />
    public int ControlsColumn
    {
      get { return _controlsColumn; }
      set { _controlsColumn = value; }
    }

    /// <summary> The <see cref="HtmlTableRow"/> containing the labels cell. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsRow/*' />
    public HtmlTableRow LabelsRow
    {
      get { return _htmlTableRows[_labelsRowIndex]; }
    }

    /// <summary> The <see cref="HtmlTableRow"/> containing the controls cell. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsRow/*' />
    public HtmlTableRow ControlsRow
    {
      get { return _htmlTableRows[_controlsRowIndex]; }
    }

    /// <summary> The cell containing the labels. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsCell/*' />
    public HtmlTableCell LabelsCell
    {
      get { return _labelsCell; }
    }

    /// <summary> The cell containing the controls. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsCell/*' />
    public HtmlTableCell ControlsCell
    {
      get { return _controlsCell; }
    }

    /// <summary>
    ///   The cell used as a place holder if the controls cell is not at the standard position.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsCellDummy/*' />
    public HtmlTableCell ControlsCellDummy
    {
      get { return _controlsCellDummy; }
    }

    /// <summary> The cell containing the markers. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/MarkersCell/*' />
    public HtmlTableCell MarkersCell
    {
      get { return _markersCell; }
    }

    /// <summary> The cell containing the validation messages. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ValidationMessagesCell/*' />
    public HtmlTableCell ValidationMessagesCell
    {
      get { return _validationMessagesCell; }
    }

    /// <summary>
    ///   The cell used as a place holder if the validation message cell is not at the standard
    ///   position.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/ValidationMessagesCellDummy/*' />
    public HtmlTableCell ValidationMessagesCellDummy
    {
      get { return _validationMessagesCellDummy; }
    }
  }

  /// <summary> A read only collection of <see cref="HtmlTableRow"/> objects. </summary>
  protected sealed class ReadOnlyHtmlTableRowCollection : ReadOnlyCollectionBase
  {
    /// <summary> Simple constructor. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ReadOnlyHtmlTableRowCollection/Constructor/*' />
    public ReadOnlyHtmlTableRowCollection (HtmlTableRow[] htmlTableRows)
    {
      ArgumentUtility.CheckNotNull ("htmlTableRows", htmlTableRows);

      for (int index = 0; index < htmlTableRows.Length; index++)
      {
        if (htmlTableRows[index] == null)
          throw new ArgumentNullException ("htmlTableRows[" + index + "]");
      }

       InnerList.AddRange (htmlTableRows);
    }

    /// <summary> A read only indexer for the <see cref="HtmlTableRow"/> onbjects. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ReadOnlyHtmlTableRowCollection/Indexer/*' />
    public HtmlTableRow this [int index]
    {
      get
      {
        if (index < 0 || index >= InnerList.Count) throw new ArgumentOutOfRangeException ("index");

        return (HtmlTableRow)InnerList[index];
      }
    }
  }

  /// <summary> The row types possible for a <see cref="FormGridRow"/>. </summary>
  protected enum FormGridRowType
  {
    /// <summary> The row containing the form grid's title. </summary>
    TitleRow,

    /// <summary> A row containing labels, controls and validators. </summary>
    DataRow,

    /// <summary> A row that can not be identified as one of the other types. </summary>
    UnknownRow
  }

  // constants

  /// <summary> Sufffix for identifying all tables to be used as form grids. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridSuffix/*' />
  private const string c_formGridSuffix = "FormGrid";

  #region private const string c_viewStateID...

  /// <summary> View State ID for the form grid view states. </summary>
  private const string c_viewStateIDFormGrids = "FormGrids";
  
  /// <summary> View State ID for _labelsColumn. </summary>
  private const string c_viewStateIDLabelsColumn = "_labelsColumn";
  
  /// <summary> View State ID for _controlsColumn. </summary>
  private const string c_viewStateIDControlsColumn = "_controlsColumn";
  
  /// <summary> View State ID for _showValidationMarkers. </summary>
  private const string c_viewStateIDShowValidationMarkers = "_showValidationMarkers";
  
  /// <summary> View State ID for _showRequiredMarkers. </summary>
  private const string c_viewStateIDShowRequiredMarkers = "_showRequiredMarkers";
  
  /// <summary> View State ID for _showHelpProviders. </summary>
  private const string c_viewStateIDHelpProviders = "_showHelpProviders";
  
  /// <summary> View State ID for _validatorVisibility. </summary>
  private const string c_viewStateIDValidatorVisibility = "_validatorVisibility";

  /// <summary> View State ID for _formGridSuffix. </summary>
  private const string c_viewStateIDFormGridSuffix = "_formGridSuffix";

  #endregion

  // static members

  /// <summary> The log4net logger. </summary>
  private static readonly log4net.ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  // member fields

  /// <summary>
  ///   Collection of <see cref="FormGrid" /> objects to be managed by this <c>FormGridManager</c>.
  /// </summary>
  private Hashtable _formGrids;

  /// <summary> Index of the column normally containing the labels. </summary>
  private int _labelsColumn;

  /// <summary>
  ///   Index of the column normally containing the controls. </summary>
  private int _controlsColumn;

  /// <summary>
  ///   Suffix identifying the <c>HtmlTables</c> to be managed by this <c>FormGridManager</c>.
  /// </summary>
  private string _formGridSuffix;

  /// <summary> The cell where the validation message should be written. </summary>
  private ValidatorVisibility _validatorVisibility;

  /// <summary> Enable/Disable the validation markers. </summary>
  private bool _showValidationMarkers;

  /// <summary> Enable/Disable the required markers. </summary>
  private bool _showRequiredMarkers;

  /// <summary> Enable/Disable the help providers. </summary>
  private bool _showHelpProviders;

  /// <summary> State variable for the two part transformation process. </summary>
  private bool _hasCompletedTransformationStepPreLoadViewState;

  /// <summary>
  ///   Caches the <see cref="IFormGridRowProvider"/> for this <see cref="FormGridManager"/>.
  /// </summary>
  private IFormGridRowProvider _cachedFormGridRowProvider;

  /// <summary>
  ///   <see langword="true"/> if the control hierarchy doesn't implement 
  ///   <see cref="IFormGridRowProvider"/>.
  /// </summary>
  private bool isFormGridRowProviderUndefined;

  /// <summary>
  ///   Caches the <see cref="IResourceManager"/> for this <see cref="FormGridManager"/>.
  /// </summary>
  private IResourceManager _cachedResourceManager;

  /// <summary>
  /// 
  /// </summary>
  private bool isResourceManagerUndefined;

  // construction and disposing

  /// <summary> Simple constructor. </summary>
  public FormGridManager()
  {
    _formGrids = new Hashtable();

    _labelsColumn = 0;
    _controlsColumn = 1;

    _formGridSuffix = c_formGridSuffix;

    _validatorVisibility = ValidatorVisibility.ValidationMessageInControlsColumn;
    _showValidationMarkers = true;
    _showRequiredMarkers = true;
    _showHelpProviders = true;
  }
  
  // methods and properties

  /// <summary> Validates all <c>FormGrid</c> objects managed by this <c>FormGridManager</c>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/Validate/*' />
  public bool Validate()
  {
    PopulateFormGridList (this.Parent);

    bool isValid = true;

    foreach (FormGrid formGrid in _formGrids.Values)
    {
      ValidateFormGrid (formGrid);

      isValid &= GetValidationErrors().Length == 0;
    }

    return isValid;
  }

  /// <summary>
  ///   Assembles all <see cref="ValidationError"/> objects in the managed <c>FormGrids</c>.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetValidationErrors/*' />
  public ValidationError[] GetValidationErrors()
  {
    ArrayList validationErrorList = new ArrayList();

    foreach (FormGrid formGrid in _formGrids.Values)
        validationErrorList.AddRange(formGrid.GetValidationErrors());

    return (ValidationError[])validationErrorList.ToArray (typeof (ValidationError));
  }

  /// <summary> Implementation of <see cref="IResourceDispatchTarget"/>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/Dispatch/*' />
  public virtual void Dispatch (IDictionary values)
  {
    PopulateFormGridList (Parent);

    if (_formGrids == null)
      return;

    Hashtable formGridControls = new Hashtable();

    //  Parse the values

    foreach (DictionaryEntry entry in values)
    {
      //  Compound key: "formGridID:controlID:property"
      string key = (string)entry.Key;

      int posColon = key.IndexOf (':');

      //  Split after formGridID
      string formGridID = key.Substring (0, key.IndexOf (":"));
      string elementIDProperty = key.Substring (posColon + 1);

      FormGrid formGrid = (FormGrid)_formGrids[formGridID.GetHashCode()];

      if (formGrid != null)
      {
        //  Get the controls for the current FormGrid
        Hashtable controls = (Hashtable) formGridControls[formGridID];

        //  If no hashtable exists, create it and insert it into the formGridControls hashtable.
        if (controls == null)
        {
          controls = new Hashtable 
            ();
          formGridControls[formGridID] = controls;
        }

        //  Test for a second colon in the key
        posColon = elementIDProperty.IndexOf (':');

        if (posColon >= 0)
        {
          //  If one is found, this is an elementID/property pair
    
          string controlID = elementIDProperty.Substring (0, posColon);
          string property = elementIDProperty.Substring (posColon + 1);

          //  Get the dictonary for the current element
          IDictionary controlValues = (IDictionary) controls[controlID];

          //  If no dictonary exists, create it and insert it into the elements hashtable.
          if (controlValues == null)
          {
            controlValues = new HybridDictionary();
            controls[controlID] = controlValues;
          }

          //  Insert the argument and resource's value into the dictonary for the specified element.
          controlValues.Add (property, entry.Value);
        }
        else
        {
          //  Not supported format
          s_log.Warn ("FormGridManager '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' received a resource with an invalid key '" + key + "'. Required format: 'formGridID:controlID:property'.");
        }
      }
      else
      {
        //  Invalid form grid
        s_log.Warn ("FormGrid '" + formGridID + "' is not managed by FormGridManager '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "'.");
      }
    }

    //  Assign the values

    foreach (DictionaryEntry formGridEntry in formGridControls)
    {
      string formGridID = (string)formGridEntry.Key;
      FormGrid formGrid = (FormGrid)_formGrids[formGridID.GetHashCode()];
      
      Hashtable controls = (Hashtable)formGridEntry.Value;

      foreach (DictionaryEntry controlEntry in controls)
      {
        string controlID = (string)controlEntry.Key;
        Control control = formGrid.Table.FindControl (controlID);

        if (control != null)
        {
          IDictionary controlValues = (IDictionary) controlEntry.Value;

          //  Pass the values to the control
          IResourceDispatchTarget resourceDispatchTarget = control as IResourceDispatchTarget;

          if (resourceDispatchTarget != null) //  Control knows how to dispatch
            resourceDispatchTarget.Dispatch (controlValues);       
          else
            ResourceDispatcher.DispatchGeneric (control, controlValues);

          //  Access key support for Labels      
          Label label = control as Label;
          if (label != null)
          {
            //  Label has associated control
            if (label.AssociatedControlID.Length > 0)
            {
              Control associatedControl = this.FindControl (label.AssociatedControlID);

              ISmartControl smartControl = control as ISmartControl;
              if (smartControl != null && smartControl.UseLabel)
              {
                string accessKey;
                label.Text = AccessKeyUtility.FormatLabelText (label.Text, false, out accessKey);
                label.AccessKey = accessKey;
              }
              else if (associatedControl is TextBox)
              {
                string accessKey;
                label.Text = AccessKeyUtility.FormatLabelText (label.Text, false, out accessKey);
                label.AccessKey = accessKey;
              }
              else
              {
                label.Text = AccessKeyUtility.RemoveAccessKey (label.Text);
                label.AccessKey = "";
              }
            }
            else
            {
              label.Text = AccessKeyUtility.RemoveAccessKey (label.Text);
            }
          }
        }
        else
        {
          //  Invalid control
          s_log.Warn ("FormGrid '" + formGridID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a control with ID '" + controlID + "'.");
        }
      }
    }
  }

  /// <summary>
  ///   Calls <see cref="TransformIntoFormGridPreLoadViewState"/> and <see cref="TransformIntoFormGridPostValidation"/>.
  /// </summary>
  /// <param name="e"> The <see cref="EventArgs"/>. </param>
  protected override void OnPreRender (EventArgs e)
  {
    TransformIntoFormGridPreLoadViewState();
    TransformIntoFormGridPostValidation();

    base.OnPreRender (e);
  }

  /// <summary> This member overrides <see cref="Control.LoadViewState"/>. </summary>
  protected override void LoadViewState (object savedState)
  {
    //  Get view state for the form grid manager

    if (savedState != null)
    {
      base.LoadViewState (savedState);
 
      object labelsColumn = ViewState[c_viewStateIDLabelsColumn];
      if (labelsColumn != null)
        _labelsColumn = (int)labelsColumn;

      object controlsColumn = ViewState[c_viewStateIDControlsColumn];
      if (controlsColumn != null)
        _controlsColumn = (int)controlsColumn;

      object formGridSuffix = ViewState[c_viewStateIDFormGridSuffix];
      if (formGridSuffix != null)
        _formGridSuffix = (string)formGridSuffix;

      object showValidationMarkers = ViewState[c_viewStateIDShowValidationMarkers];
      if (showValidationMarkers != null)
        _showValidationMarkers = (bool)showValidationMarkers;

      object showRequiredMarkers = ViewState[c_viewStateIDShowRequiredMarkers];
      if (showRequiredMarkers != null)
        _showRequiredMarkers = (bool)showRequiredMarkers;

      object showHelpProviders = ViewState[c_viewStateIDHelpProviders];
      if (showHelpProviders != null)
        _showHelpProviders = (bool)showHelpProviders;

      object validatorVisibility = ViewState[c_viewStateIDValidatorVisibility];
      if (validatorVisibility != null)
        _validatorVisibility = (ValidatorVisibility)validatorVisibility;
    }


    //  Rebuild the HTML tables used as form grids
    TransformIntoFormGridPreLoadViewState();


    //  Restore the view state to the form grids

    Hashtable formGridViewStates = ViewState[c_viewStateIDFormGrids] as Hashtable;

    if (formGridViewStates != null)
    {
      foreach (FormGrid formGrid in _formGrids.Values)
      {
        object viewState = formGridViewStates[formGrid.Table.ID];

        LoadFormGridViewState (formGrid, viewState);
      }
    }
  }

  /// <summary> This member overrides <see cref="Control.SaveViewState"/>. </summary>
  protected override object SaveViewState()
  {
    Hashtable formGridViewStates = new Hashtable (_formGrids.Count);

    foreach (FormGrid formGrid in _formGrids.Values)
    {
      object formGridViewState = SaveFormGridViewState (formGrid);

      formGridViewStates.Add (formGrid.Table.ID, formGridViewState);
    }
    
    ViewState[c_viewStateIDFormGrids] = formGridViewStates;
    
    ViewState[c_viewStateIDLabelsColumn] = _labelsColumn;
    ViewState[c_viewStateIDControlsColumn] = _controlsColumn;
    ViewState[c_viewStateIDShowValidationMarkers] = _showValidationMarkers;
    ViewState[c_viewStateIDShowRequiredMarkers] = _showRequiredMarkers;
    ViewState[c_viewStateIDHelpProviders] = _showHelpProviders;
    ViewState[c_viewStateIDValidatorVisibility] = _validatorVisibility;

    return base.SaveViewState ();
  }

  /// <summary> Restore the view state to the form grids. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/LoadFormGridViewState/*' />
  protected virtual void LoadFormGridViewState (FormGrid formGrid, object savedState)
  {
    ArgumentUtility.CheckNotNull ("formGrid", formGrid);

    if (savedState == null)
      return;

    bool enableViewStateBackup = formGrid.Table.EnableViewState;
    formGrid.Table.EnableViewState = true;

    FormGridManager.InvokeLoadViewStateRecursive (formGrid.Table, savedState);

    formGrid.Table.EnableViewState = enableViewStateBackup;
  }

  /// <summary> Saves the view state of the form grids. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/SaveFormGridViewState/*' />
  protected virtual object SaveFormGridViewState (FormGrid formGrid)
  {
    ArgumentUtility.CheckNotNull ("formGrid", formGrid);

    bool enableViewStateBackup = formGrid.Table.EnableViewState;
    formGrid.Table.EnableViewState = true;

    object viewState = FormGridManager.InvokeSaveViewStateRecursive (formGrid.Table);

    formGrid.Table.EnableViewState = enableViewStateBackup;

    // recursive table view state: Triplet
    // 1: table view state                        - will be saved/loaded
    // 2: row indices: ArrayList<int> 
    // 3: row view states: ArrayList<Triplet>:
    //    1: row view state                       - should not be saved/loaded
    //    2: cell indices: ArrayList<int>
    //    3: cell view states: ArrayList<Triplet>:
    //       1: cell view state                   - should not be saved/loaded
    //       2: control indices
    //       3: control view states
    
    Triplet table = (Triplet)viewState;
    
    ArrayList rows = (ArrayList)table.Third;
    foreach (Triplet row in rows)
    {
      //  Remove the row's view state
      row.First = null;

      ArrayList cells = (ArrayList)row.Third;
      foreach (Triplet cell in cells)
      {
        //  Remove the cell's view state
        cell.First = null;
      }
    }

    return viewState;
  }

  /// <summary>
  ///   Encapsulates the invokation of <see cref="Control"/>'s LoadViewStateRecursive method.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/InvokeLoadViewStateRecursive/*' />
  protected static void InvokeLoadViewStateRecursive (object target, object viewState)
  {
    const BindingFlags bindingFlags = BindingFlags.DeclaredOnly 
                                    | BindingFlags.Instance 
                                    | BindingFlags.NonPublic
                                    | BindingFlags.InvokeMethod;

    typeof (Control).InvokeMember (
      "LoadViewStateRecursive",
      bindingFlags,
      null,
      target,
      new object[] {viewState});
  }

  /// <summary>
  ///   Encapsulates the invokation of <see cref="Control"/>'s SaveViewStateRecursive method.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/InvokeSaveViewStateRecursive/*' />
  protected static object InvokeSaveViewStateRecursive (object target)
  {
    const BindingFlags bindingFlags = BindingFlags.DeclaredOnly 
                                    | BindingFlags.Instance 
                                    | BindingFlags.NonPublic
                                    | BindingFlags.InvokeMethod;

    object viewState = typeof (Control).InvokeMember (
        "SaveViewStateRecursive",
        bindingFlags,
        null,
        target,
        new object[] {});

    return viewState;
  }

  /// <summary> This member overrides <see cref="Control.Render"/>. </summary>
	protected override void Render(HtmlTextWriter output)
	{
    //  nothing, required get a usefull designer output without much coding.
  }

  /// <summary>
  ///   Analyzes the table layout and creates the appropriate <see cref="FormGridRow"/> isntances.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CreateFormGridRows/*' />
  protected virtual FormGridRow[] CreateFormGridRows (
    FormGrid formGrid,
    int labelsColumn,
    int controlsColumn)
  {
    ArgumentUtility.CheckNotNull ("formGrid", formGrid);

    ArrayList formGridRows = new ArrayList(formGrid.Table.Rows.Count);

    HtmlTableRowCollection rows = formGrid.Table.Rows;

    //  Form Grid Title

    bool hasTitleRow = false;

    //  Table may only have a single cell in the first row
    if (rows[0].Cells.Count == 1)
      hasTitleRow = true;

    if (hasTitleRow)
    {
      HtmlTableRow[] tableRows = new HtmlTableRow[1];
      tableRows[0] = rows[0];

      FormGridRow formGridRow = new FormGridRow (
        tableRows, 
        FormGridRowType.TitleRow,
        labelsColumn,
        controlsColumn);

      formGridRows.Add (formGridRow);
    }

    //  Form Grid Body

    for (int i = formGridRows.Count; i < rows.Count; i++)
    {
      //  If ControlsColumn cell contains controls: single row constellation
      bool hasOneDataRow =    rows[i].Cells.Count > _controlsColumn
                          &&  rows[i].Cells[_controlsColumn].Controls.Count > 0;

      //  If it is not a single row constellation
      //  and the table still has another row left
      //  and the next row contains at the label's cell
      bool hasTwoDataRows =     !hasOneDataRow
                            &&  i + 1 < rows.Count
                            &&  rows[i + 1].Cells.Count > _labelsColumn;

      //  One HtmlTableRow is one FormGrid DataRow
      if (hasOneDataRow)
      {
        HtmlTableRow[] tableRows = new HtmlTableRow[1];
        tableRows[0] = formGrid.Table.Rows[i];

        FormGridRow formGridRow = new FormGridRow (
          tableRows, 
          FormGridRowType.DataRow,
          labelsColumn,
          controlsColumn);
        
        formGridRows.Add (formGridRow);
      }
        //  Two HtmlTableRows get combined into one FormGrid DataRow
      else if (hasTwoDataRows)
      {
        HtmlTableRow[] tableRows = new HtmlTableRow[2];
        tableRows[0] = rows[i];
        tableRows[1] = rows[i + 1];

        FormGridRow formGridRow = new FormGridRow (
          tableRows, 
          FormGridRowType.DataRow,
          labelsColumn,
          controlsColumn);
        
        formGridRows.Add (formGridRow);

        i++;
      }
        //  Can't interpret layout of current HtmlTableRow
      else 
      {
        HtmlTableRow[] tableRows = new HtmlTableRow[1];
        tableRows[0] = rows[i];

        FormGridRow formGridRow = new FormGridRow (
          tableRows, 
          FormGridRowType.UnknownRow,          
          labelsColumn,
          controlsColumn);
       
        formGridRows.Add (formGridRow);
      }
    }

    return (FormGridRow[])formGridRows.ToArray(typeof(FormGridRow));
  }

  /// <summary> Validates all <see cref="BaseValidator"/> objects in the <see cref="FormGrid"/>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ValidateFormGrid/*' />
  protected virtual void ValidateFormGrid (FormGrid formGrid)
  {
    if (formGrid == null) throw new ArgumentNullException ("formGrid");

    foreach (FormGridRow formGridRow in formGrid.Rows)
    {
      if (formGridRow.Type != FormGridRowType.DataRow)
        continue;

      ValidateDataRow (formGridRow);
    }
  }

  /// <summary>
  ///   Validates the <see cref="BaseValidator"/> objects
  ///   and creates the appropriate validation marker and <see cref="ValidationError"/> objects.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ValidateDataRow/*' />
  protected virtual void ValidateDataRow (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    dataRow.ValidationMarker = null;
    ArrayList validationErrorList = new ArrayList();

    if (!dataRow.Visible)
      return;

    //  Check for validators and then their validation state
    //  Create a ValidationError object for each error
    //  Create the validationIcon

    string toolTip = "";

    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      BaseValidator validator = control as BaseValidator;

      //  Only for validators
      if (validator == null)
        continue;

      //  Validate
      validator.Validate();
      if (validator.IsValid)
        continue;

      //  Optimize: Expect the validated control in the same cell
      //  and look in the page only if not found locally
      Control controlToValidate = validator.NamingContainer.FindControl (
        validator.ControlToValidate);

      //  Ignore invisible controls
      if (!controlToValidate.Visible)
        continue;

      //  Get validation message
      string validationMessage = validator.ErrorMessage;

      //  Get tool tip, tool tip is validation message
      if (validationMessage != null && validationMessage.Length > 0)
      {
        if (toolTip.Length > 0)
          toolTip += Environment.NewLine;

        toolTip += validationMessage;
      }

      //  Build ValidationError
      validationErrorList.Add (
        new ValidationError (controlToValidate, validationMessage, validator));
    }

    bool hasValidationErrors = validationErrorList.Count > 0;

    if (hasValidationErrors)
      dataRow.ValidationMarker = GetValidationMarker (toolTip);

    dataRow.ValidationErrors = (ValidationError[])validationErrorList.ToArray (typeof (ValidationError));
  }

  /// <summary> Transforms the <see cref="HtmlTable"/> into a form grid. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/TransformIntoFormGridPreLoadViewState/*' />
  protected virtual void TransformIntoFormGridPreLoadViewState()
  {
    if (_hasCompletedTransformationStepPreLoadViewState)
      return;

    PopulateFormGridList (Parent);

    foreach (FormGrid formGrid in _formGrids.Values)
    {
      formGrid.Table.EnableViewState = false;

      formGrid.BuildIDCollection();

      LoadNewFormGridRows (formGrid);

      ApplyExternalHiddenSettings (formGrid);

      ComposeFormGridContents (formGrid);

      FormatFormGrid (formGrid);
    }

    _hasCompletedTransformationStepPreLoadViewState = true;
  }

  /// <summary> Transforms the <see cref="HtmlTable"/> into a form grid. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/TransformIntoFormGridPostValidation/*' />
  protected virtual void TransformIntoFormGridPostValidation()
  {
    foreach (FormGrid formGrid in _formGrids.Values)
    {
      foreach (FormGridRow formGridRow in formGrid.Rows)
      {
        if (formGridRow.Type == FormGridRowType.DataRow)
        {
          LoadMarkersIntoCell (formGridRow);
          LoadValidationMessagesIntoCell (formGridRow);
        }

        if (!formGridRow.CheckVisibility())
          formGridRow.Hide();

        AddShowEmptyCellsHack (formGridRow);
      }
    }
  }

  /// <summary>
  ///   Queries the parent hierarchy for an <see cref="IFormGridRowProvider"/> and inserts 
  ///   the provided new rows into the form grid.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/LoadNewFormGridRows/*' />
  protected virtual void LoadNewFormGridRows (FormGrid formGrid)
  {

    IFormGridRowProvider rowProvider = GetFormGridRowProvider (this);

    if (rowProvider == null)
      return;

    FormGridRowPrototypeCollection formGridRowPrototypes =
      rowProvider.GetListOfFormGridRowPrototypes (formGrid.Table.ID);

    foreach (FormGridRowPrototype prototype in formGridRowPrototypes)
    {
      int rowCount = 1;
      if (prototype.NewRowType == FormGridRowPrototype.RowType.ControlInRowAfterLabel)
        rowCount = 2;

      HtmlTableRow[] htmlTableRows = new HtmlTableRow [rowCount];

      //  Row with label
      htmlTableRows[0] = new HtmlTableRow();
      for (int idxCells = 0, columnCount = ControlsColumn + 1; idxCells <= columnCount; idxCells++)
        htmlTableRows[0].Cells.Add (new HtmlTableCell());

      //  Row with control
      if (prototype.NewRowType == FormGridRowPrototype.RowType.ControlInRowAfterLabel)
      {
        htmlTableRows[1] = new HtmlTableRow();
        for (int idxCells = 0, columnCount = ControlsColumn + 1; idxCells <= columnCount; idxCells++)
          htmlTableRows[1].Cells.Add (new HtmlTableCell());
      }

      //  Control in labels row
      if (prototype.NewRowType == FormGridRowPrototype.RowType.ControlInRowWithLabel)
      {
        htmlTableRows[0].Cells[ControlsColumn].Controls.Add (prototype.Control);
      }
        //  Control in row after labels row
      else if (prototype.NewRowType == FormGridRowPrototype.RowType.ControlInRowAfterLabel)
      {
        htmlTableRows[1].Cells[LabelsColumn].Controls.Add (prototype.Control);
      }

      FormGridRow newFormGridRow = new FormGridRow (
        htmlTableRows, 
        FormGridRowType.DataRow, 
        LabelsColumn, 
        ControlsColumn);

      formGrid.InsertNewFormGridRow (
        newFormGridRow, 
        prototype.ReleatedRowID, 
        prototype.PositionInFormGrid);
    }
  }

  /// <summary>
  ///   Queries the parent hierarchy for an <see cref="IFormGridRowProvider"/> and hides  
  ///   the rows identified as invisible rows.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ApplyExternalHiddenSettings/*' />
  protected virtual void ApplyExternalHiddenSettings (FormGrid formGrid)
  {
    IFormGridRowProvider rowProvider = GetFormGridRowProvider (this);

    if (rowProvider == null)
      return;

    StringCollection strings = rowProvider.GetListOfHiddenRows (formGrid.Table.ID);

    foreach (string id in strings)
    {
      FormGridRow row = formGrid.GetRowForID (id);

      if (row != null)
        formGrid.GetRowForID (id).Visible = false;
    }
  }

  /// <summary>
  ///   Find the closest parent <see cref="Control"/> impementing
  ///   <see cref="IFormGridRowProvider"/>.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetFormGridRowProvider/*' />
  protected IFormGridRowProvider GetFormGridRowProvider (Control control)
  {
    //  Control hierarchy doesn't implent this interface
    if (isFormGridRowProviderUndefined)
      return null;

    //  Provider has already been identified.
    if (_cachedFormGridRowProvider != null)
        return _cachedFormGridRowProvider;

    //  No control, no provider
    if (control == null)
      return null;

    //  Try to get the provider

    _cachedFormGridRowProvider  = control as IFormGridRowProvider;

    if (_cachedFormGridRowProvider != null)
      return _cachedFormGridRowProvider;

    //  End of hierarchy and not found -> no IformGridRowProvider defined.
    if (control.Parent == null)
      isFormGridRowProviderUndefined = true;

    //  Try the next level
    return GetFormGridRowProvider (control.Parent);
  }

  /// <summary>
  ///   Find the <see cref="IResourceManager"/> for this <see cref="FormGridManager"/>.
  /// </summary>
  /// <returns></returns>
  protected IResourceManager GetResourceManager()
  {
    //  Control hierarchy doesn't implent this interface
    if (isResourceManagerUndefined)
      return null;

    //  Provider has already been identified.
    if (_cachedResourceManager != null)
        return _cachedResourceManager;

    //  Try to get the resource manager

    _cachedResourceManager  = ResourceManagerUtility.GetResourceManager (this);

    return _cachedResourceManager;
  }

  /// <summary>
  ///   Composes all information required to transform the <see cref="HtmlTable"/> 
  ///   into a form grid.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ComposeFormGridContents/*' />
  protected virtual void ComposeFormGridContents (FormGrid formGrid)
  {
    if (formGrid == null) throw new ArgumentNullException ("formGrid");

    foreach (FormGridRow formGridRow in formGrid.Rows)
    {
      if (formGridRow.Type != FormGridRowType.DataRow)
        continue;
  
      ComposeDataRowContents (formGridRow);
    }
  }

  /// <summary>
  ///   Uses the information stored in <paramref name="formGrid"/> to format the 
  ///   <see cref="HtmlTable"/> as a form grid.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormatFormGrid/*' />
  protected virtual void FormatFormGrid (FormGrid formGrid)
  {
    ArgumentUtility.CheckNotNull ("formGrid", formGrid);

    bool isTopDataRow = true;

    foreach (FormGridRow formGridRow in formGrid.Rows)
    {
      switch (formGridRow.Type)
      {
        case FormGridRowType.TitleRow:
        {
          FormatTitleRow (formGridRow);
          break;
        }
        case FormGridRowType.DataRow:
        {
          FormatDataRow (formGridRow, isTopDataRow);
          isTopDataRow = false;
          break;
        }
        default:
        {
          break;
        }
      }
    }

    if (HasMarkersColumn)
      formGrid.DefaultControlsColumn++;

    //  Assign CSS-class to the table if none exists
    if (formGrid.Table.Attributes["class"] == null)
    {
      formGrid.Table.Attributes["class"] = CssClassTable;
    }
  }

  /// <summary> Custom formats the title row. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormatTitleRow/*' />
  protected virtual void FormatTitleRow (FormGridRow titleRow)
  {
    ArgumentUtility.CheckNotNull ("titleRow", titleRow);
    CheckFormGridRowType ("titleRow", titleRow, FormGridRowType.TitleRow);

    //  Title cell: first row, first cell
    titleRow.SetLabelsCell (0, 0);
   
    //  Adapt ColSpan for added markers column
    if (HasMarkersColumn)
    {
      titleRow.LabelsCell.ColSpan++;
      titleRow.ControlsColumn++;
    }

    //  Adapt ColSpan for added validation error message column
    if (HasValidationMessageColumn)
      titleRow.LabelsCell.ColSpan++;

    AssignCssClassToCell (titleRow.LabelsCell, CssClassTitleCell);

    if (!titleRow.CheckVisibility())
      titleRow.Hide();

    AddShowEmptyCellsHack (titleRow);
  }

  /// <summary> Assembles all the contents of the data row. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ComposeDataRowContents/*' />
  protected virtual void ComposeDataRowContents (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);

    dataRow.SetLabelsCell (0, dataRow.LabelsColumn);

    if (dataRow.HtmlTableRows.Count == 1)
      dataRow.SetControlsCell (0, dataRow.ControlsColumn);
    else
      dataRow.SetControlsCell (1, dataRow.LabelsColumn);

    CreateLabels (dataRow);

    CreateValidators (dataRow);

    OverrideValidators (dataRow);

    CreateRequiredMarker (dataRow);

    CreateHelpProvider(dataRow);

    HandleReadOnlyControls (dataRow);
  }

  /// <summary> Custom formats a data row. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormatDataRow/*' />
  protected virtual void FormatDataRow (FormGridRow dataRow, bool isTopDataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);

    if (dataRow.LabelsRowIndex != dataRow.ControlsRowIndex)
      dataRow.SetControlsCellDummy (dataRow.LabelsRowIndex, dataRow.ControlsColumn);

    CreateMarkersCell (dataRow);

    SetOrCreateValidationMessagesCell (dataRow);

    AssignCssClassesToCells (dataRow, isTopDataRow);

    AssignCssClassesToInputControls (dataRow);


    //  Not implemented, since FrameWork 1.1 takes care of names
    //  Future version might loose this
    //  //  Put a name-tag with the control's ID in front of each control with an validation error
    //  foreach (ValidationError validationError in validationErrors)
    //  {
    //    if (validationError == null)
    //      continue;
    //
    //    //  Add name to controls
    //  }
  }

  /// <summary>
  ///   Calls <see cref="AddShowEmptyCellHack"/> for the cells identified by 
  ///   <see cref="FormGridRow"/>.
  /// </summary>
  /// <param name="formGridRow">The <see cref="FormGridRow"/>, must not be <see langword="null"/>.</param>
  protected virtual void AddShowEmptyCellsHack (FormGridRow formGridRow)
  {
    ArgumentUtility.CheckNotNull ("formGridRow", formGridRow);

    AddShowEmptyCellHack (formGridRow.LabelsCell);
    AddShowEmptyCellHack (formGridRow.ControlsCell);
    AddShowEmptyCellHack (formGridRow.ControlsCellDummy);
    AddShowEmptyCellHack (formGridRow.MarkersCell);
    AddShowEmptyCellHack (formGridRow.ValidationMessagesCell);
    AddShowEmptyCellHack (formGridRow.ValidationMessagesCellDummy);
  }

  /// <summary> Creates the cell to be used for the markers. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CreateMarkersCell/*' />
  protected virtual void CreateMarkersCell (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);

    if (!HasMarkersColumn)
      return;

    //  Markers cell is before controls cell
    dataRow.LabelsRow.Cells.Insert (dataRow.ControlsColumn, new HtmlTableCell());
    dataRow.SetMarkersCell (dataRow.LabelsRowIndex, dataRow.ControlsColumn);

    //  Controls cell now one cell to the right
    dataRow.ControlsColumn++;    

    //  Control cell in second data row spans labels column to the end of the controls columns
    if (HasSeperateControlsRow(dataRow))
    {
      int colSpan = dataRow.ControlsColumn - dataRow.LabelsColumn + 1;
      dataRow.ControlsCell.ColSpan = colSpan;
    }

  }

  /// <summary>
  ///   Loads the markers or place holders into the <see cref="FormGridRow.MarkersCell"/> 
  ///   of the <paramref name="dataRow"/>.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/LoadMarkersIntoCell/*' />
  protected virtual void LoadMarkersIntoCell (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);

    if (!HasMarkersColumn)
      return;

    ArgumentUtility.CheckNotNull ("dataRow.MarkersCell", dataRow.MarkersCell);

    //  ValidationMarker and RequiredMarker share left-hand position
    //  ValidationMarker takes precedence

    if (ShowValidationMarkers && dataRow.ValidationMarker != null)
    {
      dataRow.MarkersCell.Controls.Add(dataRow.ValidationMarker);
    }
    else if (ShowRequiredMarkers && dataRow.RequiredMarker != null)
    {
      dataRow.MarkersCell.Controls.Add(dataRow.RequiredMarker);
    }
    else if (ShowValidationMarkers || ShowRequiredMarkers)
    {
      dataRow.MarkersCell.Controls.Add(GetBlankMarker());
    }

    //  HelpProvider takes right-hand side in column

    if (ShowHelpProviders)
    {
      if (dataRow.HelpProvider != null)
        dataRow.MarkersCell.Controls.Add(dataRow.HelpProvider);
      else
        dataRow.MarkersCell.Controls.Add(GetBlankMarker());
    }
  }

  /// <summary>
  ///   Applies the <c>FormGridManager</c>'s validator settings to all objects of 
  ///   <see cref="BaseValidator"/> inside the <paramref name="cell"/>.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/OverrideValidators/*' />
  protected virtual void OverrideValidators (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      BaseValidator validator = control as BaseValidator;

      //  Only for validators
      if (validator == null)
        continue;
      
      //  FormGrid override
      if (ValidatorVisibility != ValidatorVisibility.ShowValidators)
      {
        validator.Display = ValidatorDisplay.None;
        validator.EnableClientScript = false;
      }
    }
  }

  /// <summary>
  ///   Creates the labels from the controls inside <paramref name="controlsCell"/>
  ///   if they do not already exist.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CreateLabels/*' />
  protected virtual void CreateLabels (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.LabelsCell", dataRow.LabelsCell);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    //  Already has labels
    if (dataRow.LabelsCell.Controls.Count > 0)
      return;

    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      //  Query the controls for the string to be used as the labeling Text

      string newID = String.Empty;
      string newText = String.Empty;

      if (control is ISmartControl)
      {
        newID = control.ID + "_Label";
        newText = ((ISmartControl)control).DisplayName;
      }
      else if (   control is TextBox 
              ||  control is DropDownList
              ||  control is Table)
      {
        newID = control.ID + "_Label";

        //  Get Text
        IResourceManager resourceManager = GetResourceManager();

        if (resourceManager != null)
        {
          StringBuilder identifier = new StringBuilder (100);
          Type namingContainerType = dataRow.FormGrid.Table.NamingContainer.GetType();
          identifier.Append (namingContainerType.FullName);
          identifier.Append (".");
          identifier.Append (dataRow.FormGrid.Table.ID);
          identifier.Append (".");
          identifier.Append (newID);
          newText = resourceManager.GetString (identifier.ToString());
        }
      }
        //  The control found in this iteration does not get handled by this method.
      else
      {
        continue;
      }

      //  Add seperator if already a control in the cell
      
      if (dataRow.LabelsCell.Controls.Count > 0)
      {
        LiteralControl seperator = new LiteralControl(", ");

        //  Not default, but ViewState is needed
        seperator.EnableViewState = true;

        dataRow.LabelsCell.Controls.Add(seperator);
        
        //  Set Visible after control is added so ViewState knows about it
        seperator.Visible = control.Visible;
      }

      Label label = new Label();
      
      label.ID = newID;
      
      //  Insert the text provided by the control
      if (newText != null && newText != String.Empty)
      {
        string accessKey = String.Empty;

        label.Text = AccessKeyUtility.FormatLabelText (newText, false, out accessKey);
        
        ISmartControl smartControl = control as ISmartControl;
        if (smartControl != null && smartControl.UseLabel)
        {
          label.AccessKey = accessKey;
          label.AssociatedControlID = smartControl.TargetControl.ClientID;
        }
        else if (control is TextBox)
        {
          label.AccessKey = accessKey;
          label.AssociatedControlID = control.ClientID;
        }
      }
        //  Otherwise, create default text.
      else if (control.ID != null)
      {
        label.Text = control.ID.ToUpper();
        s_log.Warn ("No resource available for control '" + control.ID + "' in naming container '" + control.NamingContainer.GetType().FullName + "'.");
      }

      //  Should be default, but better safe than sorry
      label.EnableViewState = true;

      dataRow.LabelsCell.Controls.Add(label);

      //  Set Visible after control is added so ViewState knows about it
      label.Visible = control.Visible;
    }
  }

  /// <summary>
  ///   Creates the validators from the controls inside <paramref name="dataRow"/>
  ///   if they do not already exist.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CreateValidators/*' />
  protected virtual void CreateValidators (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    ArrayList smartControls = new ArrayList();
    ArrayList validators = new ArrayList();

    //  Split into smart controls and validators
    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      if (control is ISmartControl)
        smartControls.Add (control);
      else if (control is BaseValidator)
        validators.Add (control);
    }
    
    foreach (ISmartControl smartControl in smartControls)
    {
      if (!smartControl.Visible)
        continue;

      //  Create Validators only if none are assigned for the SmartControl
      foreach (BaseValidator validator in validators)
      {
        if (validator.ControlToValidate == smartControl.ID)
          return;
      }

      BaseValidator[] newValidators = smartControl.CreateValidators();

      foreach (BaseValidator validator in newValidators)
        dataRow.ControlsCell.Controls.Add (validator);
    }
  }

  /// <summary>
  ///   Queries the controls in <paramref name="dataRow"/> for their mandatory setting 
  ///   and creates the required marker if necessary.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CreateRequiredMarker/*' />
  protected virtual void CreateRequiredMarker (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.LabelsCell", dataRow.LabelsCell);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    foreach (Control control in dataRow.LabelsCell.Controls)
    {
      if (!control.Visible)
        continue;

      ISmartControl smartControl = control as ISmartControl;

      if (smartControl == null)
        continue;

      if (smartControl.IsRequired)
      {
        dataRow.RequiredMarker = GetRequiredMarker();

        //  We have a marker, rest would be redundant
        return;
      }
    }

    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      if (!control.Visible)
        continue;

      ISmartControl smartControl = control as ISmartControl;

      if (smartControl == null)
        continue;

      if (smartControl.IsRequired)
      {
        dataRow.RequiredMarker = GetRequiredMarker();

        //  We have a marker, rest would be redundant
        return;
      }
    }
  }

  /// <summary>
  ///   Queries the controls in <paramref name="dataRow"/> if they provide help
  ///   and creates a help provider.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CreateHelpProvider/*' />
  protected virtual void CreateHelpProvider(FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.LabelsCell", dataRow.LabelsCell);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    foreach (Control control in dataRow.LabelsCell.Controls)
    {
      if (!control.Visible)
        continue;

      ISmartControl smartControl = control as ISmartControl;

      if (smartControl == null)
        continue;

      string helpUrl = smartControl.HelpUrl;

      if (helpUrl != null && helpUrl != String.Empty)
      {
        helpUrl = UrlResolverUtility.GetHelpUrl (this, helpUrl);
        dataRow.HelpProvider = GetHelpProvider (helpUrl);

        //  We have a help provider, first come, only one served
        return;
      }
    }

    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      if (!control.Visible)
        continue;

      ISmartControl smartControl = control as ISmartControl;

      if (smartControl == null)
        continue;

      string helpUrl = smartControl.HelpUrl;

      if (helpUrl != null && helpUrl != String.Empty)
      {
        dataRow.HelpProvider = GetHelpProvider (helpUrl);

        //  We have a help provider, first come, only one served
        return;
      }
    }
  }

  /// <summary> Queries the control for it's read-only setting and transforms it if necessary. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/HandleReadOnlyControls/*' />
  protected virtual void HandleReadOnlyControls (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    for (int idxControl = 0; idxControl < dataRow.ControlsCell.Controls.Count; idxControl++)
    {
     Control control = dataRow.ControlsCell.Controls[idxControl];

      if (!control.Visible)
      continue;

      // TODO: Support for non-TextBox, non-ISmartControl controls with read-only option

      TextBox textBox = control as TextBox;

      if (textBox != null)
      {
        if (textBox.ReadOnly)
        {
          LiteralControl readOnlyValue = new LiteralControl (textBox.Text);
          dataRow.ControlsCell.Controls.RemoveAt (idxControl);
          dataRow.ControlsCell.Controls.AddAt (idxControl, readOnlyValue);
        }
      }
      else
      {
        //  The control found in this iteration does not get handled by this method.
        continue;
      }
    }
  }

  /// <summary> 
  ///   Sets the cell to be used for the validation messages, creating a new cell if necessary.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/SetOrCreateValidationMessagesCell/*' />
  protected virtual void SetOrCreateValidationMessagesCell (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    //  Validation message cell

    if (ValidatorVisibility == ValidatorVisibility.ValidationMessageInControlsColumn)
    {
      if (!HasSeperateControlsRow (dataRow))
        dataRow.SetValidationMessagesCell (dataRow.ControlsRowIndex, dataRow.ControlsColumn);
      else
        dataRow.SetValidationMessagesCell (dataRow.ControlsRowIndex, dataRow.LabelsColumn);
    }
        //  Validation message cell is after controls cell
    else if (HasValidationMessageColumn)
    {
      if (!HasSeperateControlsRow (dataRow))
      {
        dataRow.ControlsRow.Cells.Insert (dataRow.ControlsColumn + 1, new HtmlTableCell());
        dataRow.SetValidationMessagesCell (dataRow.ControlsRowIndex, dataRow.ControlsColumn + 1);
      }
      else
      {
        dataRow.ControlsRow.Cells.Insert (dataRow.LabelsColumn + 1, new HtmlTableCell());
        dataRow.SetValidationMessagesCell (dataRow.ControlsRowIndex, dataRow.LabelsColumn + 1);

        dataRow.LabelsRow.Cells.Insert (dataRow.ControlsColumn + 1, new HtmlTableCell());
        dataRow.SetValidationMessagesCellDummy (dataRow.LabelsRowIndex, dataRow.ControlsColumn + 1);
      }
    }
  }

  /// <summary> Outputs the validation messages into a <see cref="HtmlTableCell"/>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/LoadValidationMessagesIntoCell/*' />
  protected virtual void LoadValidationMessagesIntoCell (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ValidationMessagesCell", dataRow.ValidationMessagesCell);

    if (dataRow.ValidationErrors != null)
    {
      //  Get validation messages
      foreach (ValidationError validationError in dataRow.ValidationErrors)
      {
        if (validationError == null)
          continue;

        dataRow.ValidationMessagesCell.Controls.Add (
          validationError.ToDiv (CssClassValidationMessage));
      }
    }
  }

  /// <summary> Assign CSS classes for cells where none exist. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/AssignCssClassesToCells/*' />
  protected virtual void AssignCssClassesToCells (FormGridRow dataRow, bool isTopDataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.LabelsCell", dataRow.LabelsCell);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    //  Label Cell
    if (dataRow.LabelsCell.Attributes["class"] == null)
    {
      string cssClass = CssClassLabelsCell;

      if (isTopDataRow)
        cssClass += " " + CssClassTopDataRow;

      AssignCssClassToCell (dataRow.LabelsCell, cssClass);
    }

    //  Marker Cell
    if (dataRow.MarkersCell != null)
    {
      string cssClass = CssClassMarkersCell;

      if (isTopDataRow)
        cssClass += " " + CssClassTopDataRow;

      AssignCssClassToCell (dataRow.MarkersCell, cssClass);
    }

    //  Control Cell
    if (dataRow.ControlsCell.Attributes["class"] == null)
    {
      string cssClass = CssClassInputControlsCell;

      if (isTopDataRow && dataRow.ControlsCellDummy == null)
        cssClass += " " + CssClassTopDataRow;

      AssignCssClassToCell (dataRow.ControlsCell, cssClass);
    }

    //  Control Cell Dummy
    if (dataRow.ControlsCellDummy != null)
    {
      string cssClass = CssClassInputControlsCell;

      if (isTopDataRow)
        cssClass += " " + CssClassTopDataRow;

      AssignCssClassToCell (dataRow.ControlsCellDummy, cssClass);
    }

    //  Validation Message Cell
    if (    dataRow.ValidationMessagesCell != null
        &&  ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn)
    {
      string cssClass = CssClassValidationMessagesCell;

      if (isTopDataRow && dataRow.ValidationMessagesCellDummy == null)
        cssClass += " " + CssClassTopDataRow;

      AssignCssClassToCell (dataRow.ValidationMessagesCell, cssClass);
    }

    //  Validation Message Cell Dummy
    if (    dataRow.ValidationMessagesCellDummy != null
        &&  ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn)
    {
      string cssClass = CssClassValidationMessagesCell;

      if (isTopDataRow)
        cssClass += " " + CssClassTopDataRow;

      AssignCssClassToCell (dataRow.ValidationMessagesCellDummy, cssClass);
    }
  }

  /// <summary> Assign CSS classes to input controls where none exist. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/AssignCssClassesToInputControls/*' />
  protected virtual void AssignCssClassesToInputControls (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    for (int idxControl = 0; idxControl < dataRow.ControlsCell.Controls.Count; idxControl++)
    {
      Control control = dataRow.ControlsCell.Controls[idxControl];

      // TODO: Query ISmartControl
      //  if ((Control as ISmartControl).UseInputControlStyle)

      TextBox textBox = control as TextBox;
      DropDownList dropDownList = control as DropDownList;

      if (textBox != null)
      {
        if (textBox.CssClass.Length == 0)
          textBox.CssClass = CssClassInputControl;
      }
      else if (dropDownList != null)
      {
        if (dropDownList.CssClass.Length == 0)
          dropDownList.CssClass = CssClassInputControl;
      }
      else
      {
        //  The control found in this iteration does not get handled by this method.
        continue;
      }
    }
  }

  /// <summary>
  ///   Tests for an empty <c>class</c> attribute and assigns the <paramref name="cssClass"/> 
  ///   if empty.
  /// </summary>
  /// <param name="cell"> The <see cref="HtmlTableCell"/> to be used. </param>
  /// <param name="cssClass"> The <c>CSS-class</c> to assign. </param> 
  protected void AssignCssClassToCell (HtmlTableCell cell, string cssClass)
  {
    if (cell.Attributes["class"] == null || cell.Attributes["class"] == string.Empty)
    {
      cell.Attributes["class"] = cssClass;
    }
  }

  /// <summary>
  ///   Adds a white space to the <paramref name="cell"/> to force show the cell in the browser.
  /// </summary>
  /// <param name="cell"> The <see cref="HtmlTableCell"/> to be made visible. </param>
  protected virtual void AddShowEmptyCellHack (HtmlTableCell cell)
  {
    if (cell != null && cell.Controls.Count == 0)
    {
      cell.Controls.Add (new LiteralControl ("&nbsp;"));
    }
  }

  /// <summary> Returns the image URL for the images defined in the <c>FormGridManager</c>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetImageUrl/*' />
  protected virtual string GetImageUrl (FormGridImage image)
  {
    StringBuilder imageUrlBuilder = new StringBuilder (100);

    imageUrlBuilder.Append (image.ToString());
    imageUrlBuilder.Append (ImageExtension);

    string relativeUrl = imageUrlBuilder.ToString();

    string imageUrl = UrlResolverUtility.GetImageUrl (this, relativeUrl);

    if (imageUrl != null)
      return imageUrl;
    else
      return relativeUrl;  
  }

  /// <summary> Builds the input required marker. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetRequiredMarker/*' />
  protected virtual Control GetRequiredMarker()
  {
    Image requiredIcon = new Image();
    requiredIcon.ImageUrl = GetImageUrl (FormGridImage.RequiredField);
    
    requiredIcon.AlternateText = "*";
 
    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this);

    if (resourceManager != null)
    {
      string alternateText = resourceManager.GetString (
        typeof (FormGridManager), 
        ResourceIdentifiers.RequiredFieldAlternateText);

      if (alternateText != null)
        requiredIcon.AlternateText = alternateText;

      string toolTip = resourceManager.GetString (
        typeof (FormGridManager), 
        ResourceIdentifiers.RequiredFieldTitle);

      if (toolTip != null)
        requiredIcon.ToolTip = toolTip;
    }

    return requiredIcon;
  }

  /// <summary> Builds the help provider. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetHelpProvider/*' />
  protected virtual Control GetHelpProvider (string helpUrl)
  {
    Image helpIcon = new Image();
    helpIcon.ImageUrl = GetImageUrl (FormGridImage.Help);

    helpIcon.AlternateText = "?";
 
    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this);

    if (resourceManager != null)
    {
      string alternateText = resourceManager.GetString (
        typeof (FormGridManager), 
        ResourceIdentifiers.HelpAlternateText);

      if (alternateText != null)
        helpIcon.AlternateText = alternateText;

      string toolTip = resourceManager.GetString (
        typeof (FormGridManager), 
        ResourceIdentifiers.HelpTitle);

      if (toolTip != null)
        helpIcon.ToolTip = toolTip;
    }

    HtmlAnchor helpAnchor = new HtmlAnchor();
    helpAnchor.HRef = helpUrl;
    helpAnchor.Controls.Add (helpIcon);
    helpAnchor.Target = "_new";

    return helpAnchor;
  }

  /// <summary> Builds a new marker for validation errors. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetValidationMarker/*' />
  protected virtual Control GetValidationMarker (string toolTip)
  {
    Image validationErrorIcon = new Image();
    validationErrorIcon.ImageUrl = GetImageUrl (FormGridImage.ValidationError);

    validationErrorIcon.AlternateText = "!";
    validationErrorIcon.ToolTip = toolTip;
 
    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this);

    if (resourceManager != null)
    {
      string alternateText = resourceManager.GetString (
        typeof (FormGridManager), 
        ResourceIdentifiers.ValidationErrorInfoAlternateText);

      if (alternateText != null)
        validationErrorIcon.AlternateText = alternateText;
    }

    return validationErrorIcon;
  }

  /// <summary> Returns a spacer to be used instead of a marker. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetBlankMarker/*' />
  protected virtual Control GetBlankMarker()
  {
    Image spacer = new Image();
    spacer.ImageUrl = GetImageUrl (FormGridImage.Spacer);

    return spacer;  
  }

  /// <summary>
  ///   Compares the <paramref name="formGridRow"/>'s <see cref="FormGridRowType"/> against the 
  ///   type passed in <paramref name="expectedFormGridRowType"/>.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CheckFormGridRowType/*' />
  protected void CheckFormGridRowType (
    string argumentName,
    FormGridRow formGridRow,
    FormGridRowType expectedFormGridRowType)
  {
    if (formGridRow == null || formGridRow.Type != expectedFormGridRowType)
      throw new ArgumentException ("Specified FormGridRow is not set to type '" + expectedFormGridRowType.ToString() + "'.", argumentName);
  }

  /// <summary>
  ///   Tests the labels matches the controls row.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/HasSeperateControlsRow/*' />
  protected virtual bool HasSeperateControlsRow (FormGridRow dataRow)
  {
    return dataRow.LabelsRowIndex != dataRow.ControlsRowIndex;
  }

  /// <summary> Registers all suffixed tables for this <c>FormGridManager</c>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/PopulateFormGridList/*' />
  private void PopulateFormGridList (Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    //  Has already pupulated
    if (_formGrids.Keys.Count > 0)
      return;

    //  Add all table having the suffix
    foreach (Control childControl in control.Controls)
    {
      HtmlTable htmlTable = childControl as HtmlTable;

      if (htmlTable != null)
      {
        if (htmlTable.ID.EndsWith (_formGridSuffix))
        {
          FormGrid formGrid = new FormGrid (
            htmlTable, 
            new FormGrid.CreateRows(CreateFormGridRows),
            _labelsColumn,
            _controlsColumn);
          
          _formGrids[formGrid.GetHashCode()] = formGrid;
        }
      }

      //  For perfomance, only recursivly call PopulateFormGridList if control-collection is filled
      if (childControl.Controls.Count > 0)
        PopulateFormGridList (childControl);
    }
  }

  /// <summary> The suffix identifying all tables managed by this <c>FormGridManager</c>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridSuffix/*' />
  [CategoryAttribute("Behaviour")]
  [DefaultValue(c_formGridSuffix)]
  [Description("The suffix that must be appended to all tables to be used as a form grid.")]
  public string FormGridSuffix
  {
    get { return _formGridSuffix; }
    set { _formGridSuffix = value; }
  }

  /// <summary>
  ///   Specifies which column in the table or tables contains the labels.
  ///   Must be less than the value of <see cref="ControlsColumn"/>.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/LabelsColumn/*' />
  [CategoryAttribute("Appearance")]
  [DefaultValue(0)]
  [Description("The index of the label column in the form grid tables. Must be less than the ControlsColumn's index")]
  public int LabelsColumn
  {
    get
    {
      return _labelsColumn; 
    }
    set
    {
      if (Page.IsPostBack)  throw new InvalidOperationException ("Setting 'LabelsColumn' is only allowed during the initial page load");
      if (value >= _controlsColumn) throw new ArgumentOutOfRangeException ("'LabelsColumn' must be lower than 'ControlsColumn'");
      
      _labelsColumn = value;
    }
  }

  /// <summary>
  ///   Specifies which column in the table or tables contains the controls for single-line rows.
  ///   Must be higher than the value of <see cref="LabelsColumn"/>.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ControlsColumn/*' />
  [CategoryAttribute("Appearance")]
  [DefaultValue(1)]
  [Description("The index of the control column in the form grid tables. Must be higher than the LabelsColumn's index")]
  public int ControlsColumn
  {
    get
    {
      return _controlsColumn; 
    }
    set
    {
      if (Page.IsPostBack)  throw new InvalidOperationException ("Setting 'ControlsColumn' is only allowed during the initial page load");
      if (value <= _labelsColumn) throw new ArgumentOutOfRangeException ("'ControlsColumn' must be higher than 'LabelsColumn'");
      
      _controlsColumn = value;
    }
  }

  /// <summary> Defines how the validation messages are displayed. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ValidatorVisibility/*' />
  [CategoryAttribute("Behavior")]
  [DefaultValue(ValidatorVisibility.ValidationMessageInControlsColumn)]
  [Description("The position of the validation messages in the form grids.")]
  public ValidatorVisibility ValidatorVisibility
  {
    get { 
      return _validatorVisibility; }
    set { 
      _validatorVisibility = value; }
  }

  /// <summary> Enables/Disables the validation markers. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ShowValidationMarkers/*' />
  [CategoryAttribute("Behavior")]
  [DefaultValue(true)]
  [Description("Enables/Disables the validation markers.")]
  public bool ShowValidationMarkers
  {
    get { return _showValidationMarkers; }
    set { _showValidationMarkers = value; }
  }

  /// <summary> Enables/Disables the required markers. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ShowRequiredMarkers/*' />
  [CategoryAttribute("Behavior")]
  [DefaultValue(true)]
  [Description("Enables/Disables the required markers.")]
  public bool ShowRequiredMarkers
  {
    get { return _showRequiredMarkers; }
    set { _showRequiredMarkers = value; }
  }

  /// <summary> Enables/Disables the help providers. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ShowHelpProviders/*' />
  [CategoryAttribute("Behavior")]
  [DefaultValue(true)]
  [Description("Enables/Disables the help providers.")]
  public bool ShowHelpProviders
  {
    get { return _showHelpProviders; }
    set { _showHelpProviders = value; }
  }

  /// <summary> Returns <see langname="true"/> if the markers column is needed. </summary>
  protected virtual bool HasMarkersColumn
  {
    get
    {
      return _showValidationMarkers || _showRequiredMarkers || _showHelpProviders;
    }
  }

  /// <summary>
  ///   Returns <see langname="true"/> if the validation messages are shown in an extra column.
  /// </summary>
  protected virtual bool HasValidationMessageColumn
  {
    get
    { 
      return ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn;
    }
  }

  /// <summary> Directory for the images, starting at with application root. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ImageDirectory/*' />
  protected virtual string ImageDirectory
  { get { return "images/"; } }

  /// <summary> Extension of the images. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ImageExtension/*' />
  protected virtual string ImageExtension
  { get { return ".gif"; } }

  #region protected virtual string CssClass...

  /// <summary> CSS-Class applied to the form grid tables' <c>table</c> tag. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassTable/*' />
  protected virtual string CssClassTable
  { get { return "formGridTable"; } }

  /// <summary> CSS-Class applied to the cell containing the header. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassTitleCell/*' />
  protected virtual string CssClassTitleCell
  { get { return "formGridTitleCell"; } }

  /// <summary> CSS-Class applied to the cells containing the labels. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassLabelsCell/*' />
  protected virtual string CssClassLabelsCell
  { get { return "formGridLabelsCell"; } }

  /// <summary>
  ///   CSS-Class applied to the cells containing the marker controls
  ///   (required, validation error, help).
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassMarkersCell/*' />
  protected virtual string CssClassMarkersCell
  { get { return "formGridMarkersCell"; } }

  /// <summary> CSS-Class applied to the cells containing the input controls. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassInputControlsCell/*' />
  protected virtual string CssClassInputControlsCell
  { get { return "formGridControlsCell"; } }

  /// <summary> CSS-Class applied to the cells containing the validation messages. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassValidationMessagesCell/*' />
  protected virtual string CssClassValidationMessagesCell
  { get { return "formGridValidationMessagesCell"; } }

  /// <summary> CSS-Class additionally applied to the first row after the header row. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassTopDataRow/*' />
  protected virtual string CssClassTopDataRow
  { get { return "formGridTopDataRow"; } }

  /// <summary> CSS-Class applied to the input controls. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassInputControl/*' />
  protected virtual string CssClassInputControl
  { get { return "formGridInputControl"; } }

  /// <summary> CSS-Class applied to the individual validation messages </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/CssClassValidationMessage/*' />
  protected virtual string CssClassValidationMessage
  { get { return "formGridValidationMessage"; } }

  #endregion
}

/// <summary> Defiens how the validators are displayed in the FormGrid. </summary>
public enum ValidatorVisibility
{
  /// <summary> Don't display the validation messages. </summary>
  HideValidators,

  /// <summary> Leave displaying the validation messages to the individual validation controls. </summary>
  ShowValidators,

  /// <summary>
  ///   Display the validation message in the same cell as as the invalid control's.
  ///   Default implementation display each message inside it own <c>div</c>-tag.
  /// </summary>
  ValidationMessageInControlsColumn,
  
  /// <summary>
  ///   Display the validation message in a new cell inserted after the invalid control's cell.
  ///   Default implementation display each message inside it own <c>div</c>-tag.
  /// </summary>
  ValidationMessageAfterControlsColumn
}

}