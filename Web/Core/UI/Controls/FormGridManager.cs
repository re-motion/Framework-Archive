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

using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Web.UI.Globalization;
using Rubicon.Globalization;

namespace Rubicon.Web.UI.Controls
{

/// <summary> Transforms one or more tables into form grids. </summary>
/// <include file='doc\include\FormGridManager.xml' path='FormGridManager/Class/*' />
//  TODO: HTMLEncode
[ToolboxItemFilter("System.Web.UI")]
public class FormGridManager : Control, IControl, IResourceDispatchTarget, ISupportsPostLoadControl
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
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.Web.UI.Globalization.FormGridManager")]
  protected enum ResourceIdentifier
  {
    /// <summary>The alternate text for the required icon.</summary>
    RequiredFieldAlternateText,
    /// <summary>The alternate text for the help icon.</summary>
    HelpAlternateText,
    /// <summary>The alternate text for the validation error icon.</summary>
    ValidationErrorInfoAlternateText,
    /// <summary>The tool tip text for the required icon.</summary>
    RequiredFieldTitle,
    /// <summary>The tool tip text for the help icon.</summary>
    HelpTitle,
    //  Not used, title always set to message
    //  ValidationErrorInfoTitle,
  }
  
  protected enum TransformationStep
  {
    TransformationNotStarted = 0,
    PreLoadViewStateTransformationCompleted = 1,
    PostLoadTransformationCompleted = 2,
    PostValidationTransformationCompleted = 3
  }

  /// <summary>
  ///   Wrapper class for a single HtmlTable plus the additional information
  ///   added through the <see cref="FormGridManager"/>.
  /// </summary>
  protected class FormGrid
  {
    /// <summary> The <see cref="HtmlTable"/> used as a base for the form grid. </summary>
    private HtmlTable _table;

    /// <summary> The <see cref="FormGridRow"/> collection for this <see cref="FormGrid"/>. </summary>
    private FormGridRowCollection _rows;

    /// <summary> The column normally containing the labels. </summary>
    private int _defaultLabelsColumn;

    /// <summary> The column normally containing the controls. </summary>
    private int _defaultControlsColumn;

    /// <summary> 
    ///   Initializes a new isntance of the <see cref="FormGrid"/> class with the 
    ///   <see cref="HtmlTable"/> used as a form grid, the <see cref="FormGridRow"/> array 
    ///   and the indeces of the columns normally containing the labels and the controls.
    /// </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGrid/Constructor/*' />
    public FormGrid (
        HtmlTable table, 
        FormGridRow[] rows,
        int defaultLabelsColumn, 
        int defaultControlsColumn)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("rows", rows);

      _table = table;
      _defaultLabelsColumn = defaultLabelsColumn;
      _defaultControlsColumn = defaultControlsColumn;
      _rows = new FormGridRowCollection (this, rows);
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
        FormGridRowInfo.RowPosition positionInFormGrid)
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
        Rows.Add (newFormGridRow);
      }
        //  Insert after the related form grid row
      else if (positionInFormGrid == FormGridRowInfo.RowPosition.AfterRowWithID)
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
            //  We want to insert after the current position
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

        int idxFormGridRow = Rows.IndexOf (relatedRow);
        //  After the index of the related row
        idxFormGridRow++;
        Rows.Insert (idxFormGridRow, newFormGridRow);
      }
        //  Insert before the related form grid row
      else if (positionInFormGrid == FormGridRowInfo.RowPosition.BeforeRowWithID)
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
        int idxFormGridRow = Rows.IndexOf (relatedRow);
        //  Before the related row
        Rows.Insert (idxFormGridRow, newFormGridRow);
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

    /// <summary> Gets or sets the index of the column used for labels. </summary>
    public int DefaultLabelsColumn
    {
      get { return _defaultLabelsColumn; }
      set { _defaultLabelsColumn = value; }
    }

    /// <summary> Gets or sets the index of the column used for controls. </summary>
    /// <remarks>
    ///   Note that controls using a seperate row may exist in the column <see cref="DefaultLabelsColumn"/>.
    /// </remarks>
    public int DefaultControlsColumn
    {
      get { return _defaultControlsColumn; }
      set { _defaultControlsColumn = value; }
    }
  }

  /// <summary> A collection of <see cref="FormGridRow"/> objects. </summary>
  protected sealed class FormGridRowCollection: CollectionBase, IList
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
        if (index < 0 || index >= InnerList.Count) 
          throw new ArgumentOutOfRangeException ("index");
        return (FormGridRow)InnerList[index];
      }
    }

    /// <summary> Allows only the insertion of objects of type of <see cref="FormGridRow"/>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRowCollection/OnInsert/*' />
    protected override void OnInsert(int index, object value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckType ("value", value, typeof (FormGridRow));
      FormGridRow formGridRow = (FormGridRow) value; 
      if (formGridRow.HtmlTableRows[0].Parent != _ownerFormGrid.Table)
        throw new InvalidOperationException ("The FormGridRow that attempted to be inserted at position " + index + " contains HtmlTableRows belonging to the table '" + formGridRow.HtmlTableRows[0].Parent.ID + "', but the FormGrid encapsulates the table '" +_ownerFormGrid.Table.ID + "'.");
      formGridRow._formGrid = _ownerFormGrid;
      base.OnInsert (index, value);
    }

    public int IndexOf (object value)
    {
      return InnerList.IndexOf (value);
    }

    public void Insert (int index, object value)
    {
      OnInsert (index, value);
      InnerList.Insert (index, value);
      OnInsertComplete (index, value);
    }

    public void Add (object value)
    {
      OnInsert (InnerList.Count, value);
      int index = InnerList.Add (value);
      OnInsertComplete (index, value);
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
    private Image _validationMarker;

    /// <summary>The required marker for this <c>FormGridRow</c>. </summary>
    private Image _requiredMarker;

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

    private bool _isGenerated;

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
      int controlsColumn,
      bool isGenerated)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlTableRows", htmlTableRows);

      _htmlTableRows = new ReadOnlyHtmlTableRowCollection(htmlTableRows);
      _type = type;
      _validationErrors = new ValidationError[]{};
      _labelsColumn = labelsColumn;
      _controlsColumn = controlsColumn;
      _visible = true;
      _controls = new Hashtable(0);
      _isGenerated = isGenerated;

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
      if (   rowIndex >= _htmlTableRows.Count
          && rowIndex < 0)
      {
        string tableID = _formGrid.Table.ID;
        throw new ArgumentOutOfRangeException ("rowIndex", rowIndex, string.Format ("Error while formatting HtmlTable '{0}': The rowIndex exceeds the number of rows in the row-group being formatted. Rows in the row-group:", tableID, _htmlTableRows.Count));
      }

      if (   cellIndex >= _htmlTableRows[rowIndex].Cells.Count
          || cellIndex < 0)
      {
        string tableID = _formGrid.Table.ID;
        int htmlRowIndex = _formGrid.Table.Controls.IndexOf (_htmlTableRows[rowIndex]);
        throw new ArgumentOutOfRangeException ("cellIndex", cellIndex, string.Format ("Error while formatting HtmlTable '{0}', row {1}: The row has no cell at index {2}.", tableID, htmlRowIndex, cellIndex));
      }
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
    ///   its own <see cref="Visible"/> property is set to false.
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
    ///   Sets the <see cref="FormGridRow"/> and its contained <see cref="HtmlTableRow"/> objects
    ///   invisible.
    /// </summary>
    public virtual void Hide()
    {
      _visible = false;

      foreach (HtmlTableRow row in _htmlTableRows)
        row.Visible = false;
    }

    /// <summary>
    ///   Sets the <see cref="FormGridRow"/> and its contained <see cref="HtmlTableRow"/> 
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

    /// <summary> Gets or sets a value indicating whether this row has been generated from a <see cref="IFormGridRowProvider"/>. </summary>
    public bool IsGenerated
    {
      get { return _isGenerated; }
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
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/whether/*' />
    public Image ValidationMarker
    {
      get { return _validationMarker; }
      set { _validationMarker = value; }
    }

    /// <summary> The required marker for this <c>FormGridRow</c>. </summary>
    /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormGridRow/RequiredMarker/*' />
    public Image RequiredMarker
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
        if (index < 0 || index >= InnerList.Count) 
          throw new ArgumentOutOfRangeException ("index");
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

  private const string c_generatedLabelSuffix = "_Label";
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
  private static readonly ILog s_log = LogManager.GetLogger (typeof (FormGridManager));

  // member fields

  /// <summary>
  ///   Hashtable&lt;string uniqueID, <see cref="FormGrid"/>&gt;
  /// </summary>
  private Hashtable _formGrids = new Hashtable();

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

  /// <summary> 
  ///   State variable for the two part transformation process. 
  ///   Hashtable&lt;FormGrid, TransformationStep completedStep&gt; 
  /// </summary>
  private Hashtable _completedTransformationStep = new Hashtable();

//  /// <summary> State variable for automatic validators creation. </summary>
//  private bool _hasValidatorsCreated;

  /// <summary> Caches the <see cref="IFormGridRowProvider"/> for this <see cref="FormGridManager"/>. </summary>
  private IFormGridRowProvider _cachedFormGridRowProvider;

  /// <summary>
  ///   <see langword="true"/> if the control hierarchy doesn't implement <see cref="IFormGridRowProvider"/>.
  /// </summary>
  private bool isFormGridRowProviderUndefined;

  /// <summary> Caches the <see cref="ResourceManagerSet"/> for this <see cref="FormGridManager"/>. </summary>
  private ResourceManagerSet _cachedResourceManager;

  /// <summary> <see langword="true"/> if PostBack and ViewState was loaded. </summary>
  private bool _hasViewState = false;

  private bool _formGridListPopulated = false;

  // construction and disposing

  /// <summary> Simple constructor. </summary>
  public FormGridManager()
  {
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
    EnsureTransformationStep (TransformationStep.PostLoadTransformationCompleted);

    bool isValid = true;
    foreach (FormGrid formGrid in _formGrids.Values)
      isValid &= ValidateFormGrid (formGrid);

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
    EnsureTransformationStep (TransformationStep.PreLoadViewStateTransformationCompleted);

    Hashtable formGridControls = new Hashtable();

    //  Parse the values

    foreach (DictionaryEntry entry in values)
    {
      //  Compound key: "tableUniqueID:controlUniqueID:property"
      string key = (string)entry.Key;

      int posColon = key.IndexOf (':');

      //  Split after table id
      string tableID = key.Substring (0, key.IndexOf (":"));
      string elementIDProperty = key.Substring (posColon + 1);

      FormGrid formGrid = (FormGrid)_formGrids[tableID];

      if (formGrid != null)
      {
        //  Get the controls for the current FormGrid
        Hashtable controls = (Hashtable) formGridControls[tableID];

        //  If no hashtable exists, create it and insert it into the formGridControls hashtable.
        if (controls == null)
        {
          controls = new Hashtable();
          formGridControls[tableID] = controls;
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
          s_log.Warn ("FormGridManager '" + UniqueID + "' on page '" + Page.ToString() + "' received a resource with an invalid key '" + key + "'. Required format: 'tableUniqueID:controlUniqueID:property'.");
        }
      }
      else
      {
        //  Invalid form grid
        s_log.Warn ("FormGrid '" + tableID + "' is not managed by FormGridManager '" + UniqueID + "' on page '" + Page.ToString() + "'.");
      }
    }

    //  Assign the values

    foreach (DictionaryEntry formGridEntry in formGridControls)
    {
      string tableID = (string)formGridEntry.Key;
      FormGrid formGrid = (FormGrid)_formGrids[tableID];
      
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
              Control associatedControl = NamingContainer.FindControl (label.AssociatedControlID);

              ISmartControl smartControl = control as ISmartControl;
              if (smartControl != null && smartControl.UseLabel)
              {
                string accessKey;
                label.Text = SmartLabel.FormatLabelText (label.Text, true, out accessKey);
                label.AccessKey = accessKey;
              }
              else if (control is DropDownList || control is HtmlSelect)
              {
                label.Text = SmartLabel.FormatLabelText (label.Text, false);
                label.AccessKey = "";
              }
              else
              {
                string accessKey;
                label.Text = SmartLabel.FormatLabelText (label.Text, true, out accessKey);
                label.AccessKey = accessKey;
              }
            }
            else
            {
              label.Text = SmartLabel.FormatLabelText (label.Text, false);
            }
          }
        }
        else
        {
          //  Invalid control
          s_log.Warn ("FormGrid '" + tableID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a control with UniqueID '" + controlID + "'.");
        }
      }
    }
  }
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    NamingContainer.Load += new EventHandler(NamingContainer_Load);
    NamingContainer.PreRender += new EventHandler(NamingContainer_PreRender);
  }

  private void NamingContainer_Load (object sender, EventArgs e)
  {
    EnsureFormGridListPopulated();
    if (    ! ControlHelper.IsDesignMode (this, Context)
        &&  Page.IsPostBack
        &&  ! _hasViewState)
    {
      throw new InvalidOperationException ("FormGrid '" + ID + "' did not receive a view state.");
    }
  }

  private void NamingContainer_PreRender (object sender, EventArgs e)
  {
    string key = typeof (FormGridManager).FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (FormGridManager), ResourceType.Html, "FormGrid.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }
  }

  /// <summary>
  ///   Calls <see cref="EnsureTransformationStep"/> with 
  ///   <see cref="TransformationStep.PreLoadViewStateTransformationCompleted"/>.
  /// </summary>
  private void Table_Load (object sender, EventArgs e)
  {
    string formGridID = ((HtmlTable) sender).UniqueID;
    FormGrid formGrid = (FormGrid) _formGrids[formGridID];
    EnsureTransformationStep (formGrid, TransformationStep.PreLoadViewStateTransformationCompleted);
  }

  /// <summary>
  /// Optionally called after the <c>Load</c> event.
  /// </summary>
  public void OnPostLoad()
  {
    EnsureTransformationStep (TransformationStep.PostLoadTransformationCompleted);
  }

  /// <summary>
  ///   Calls <see cref="EnsureTransformationStep"/> with 
  ///   <see cref="TransformationStep.PostValidationTransformationCompleted"/>.
  /// </summary>
  private void Table_PreRender (object sender, EventArgs e)
  {
    string formGridID = ((HtmlTable) sender).UniqueID;
    FormGrid formGrid = (FormGrid) _formGrids[formGridID];
    EnsureTransformationStep (formGrid, TransformationStep.PostValidationTransformationCompleted);
  }

  /// <summary> This member overrides <see cref="Control.LoadViewState"/>. </summary>
  protected override void LoadViewState (object savedState)
  {
    //  Get view state for the form grid manager

    if (savedState != null)
    {
      _hasViewState = true;

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
    EnsureTransformationStep (TransformationStep.PreLoadViewStateTransformationCompleted);


    //  Restore the view state to the form grids

    Hashtable formGridViewStates = (Hashtable) ViewState[c_viewStateIDFormGrids];

    if (formGridViewStates != null)
    {
      foreach (FormGrid formGrid in _formGrids.Values)
      {
        object viewState = formGridViewStates[formGrid.Table.UniqueID];

        LoadFormGridViewState (formGrid, viewState);
      }
    }
  }

  /// <summary> This member overrides <see cref="Control.SaveViewState"/>. </summary>
  protected override object SaveViewState()
  {
    // Hashtable<string, object>
    Hashtable formGridViewStates = new Hashtable (_formGrids.Count);

    foreach (FormGrid formGrid in _formGrids.Values)
    {
      object formGridViewState = SaveFormGridViewState (formGrid);
      formGridViewStates.Add (formGrid.Table.UniqueID, formGridViewState);
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
  private void LoadFormGridViewState (FormGrid formGrid, object savedState)
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
  private object SaveFormGridViewState (FormGrid formGrid)
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

    //  Completely empty table could result in a null reference as a list
    if (table.Third == null)
        return viewState;

    ArrayList rows = (ArrayList)table.Third;
    foreach (Triplet row in rows)
    {
      //  Remove the row's view state
      row.First = null;

      //  Cells without any view state relevant data
      //  result in a null reference as a list.
      if (row.Third == null)
        continue;

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
  private static void InvokeLoadViewStateRecursive (object target, object viewState)
  {
    const BindingFlags bindingFlags = BindingFlags.DeclaredOnly 
                                    | BindingFlags.Instance 
                                    | BindingFlags.NonPublic
                                    | BindingFlags.InvokeMethod;

    //  HACK: FormGridManager: Reflection on internal void Control.LoadViewStateRecursive (object)
    //  internal void System.Web.UI.Control.LoadViewStateRecursive (object)
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
  private static object InvokeSaveViewStateRecursive (object target)
  {
    const BindingFlags bindingFlags = BindingFlags.DeclaredOnly 
                                    | BindingFlags.Instance 
                                    | BindingFlags.NonPublic
                                    | BindingFlags.InvokeMethod;

    //  HACK: FormGridManager: Reflection on internal object Control.SaveViewStateRecursive()
    //  internal object System.Web.UI.Control.LoadViewStateRecursive()
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

  /// <summary> Analyzes the table layout and creates the appropriate <see cref="FormGridRow"/> isntances. </summary>
  private FormGridRow[] CreateFormGridRows (HtmlTable table, int labelsColumn, int controlsColumn)
  {
    ArgumentUtility.CheckNotNull ("table", table);

    ArrayList formGridRows = new ArrayList(table.Rows.Count);

    HtmlTableRowCollection rows = table.Rows;

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
        controlsColumn,
        false);

      formGridRows.Add (formGridRow);
    }

    //  Form Grid Body

    for (int i = formGridRows.Count; i < rows.Count; i++)
    {
      bool isDataRow = rows[i].Cells.Count > _controlsColumn;

      //  If ControlsColumn cell contains controls: single row constellation
      bool hasOneDataRow =   isDataRow
                          && rows[i].Cells[_controlsColumn].Controls.Count > 0;

      //  If it is not a single row constellation
      //  and the table still has another row left
      //  and the next row contains at the label's cell
      bool hasTwoDataRows =    isDataRow
                            && !hasOneDataRow
                            && i + 1 < rows.Count
                            && rows[i + 1].Cells.Count > _labelsColumn;

      if (! isDataRow)
      {
        //  One HtmlTableRow is one FormGrid DataRow
        HtmlTableRow[] tableRows = new HtmlTableRow[1];
        tableRows[0] = table.Rows[i];

        FormGridRow formGridRow = new FormGridRow (
          tableRows, 
          FormGridRowType.UnknownRow,
          labelsColumn,
          controlsColumn,
          false);
        
        formGridRows.Add (formGridRow);
      }
      else if (hasOneDataRow)
      {
        //  One HtmlTableRow is one FormGrid DataRow
        HtmlTableRow[] tableRows = new HtmlTableRow[1];
        tableRows[0] = table.Rows[i];

        FormGridRow formGridRow = new FormGridRow (
          tableRows, 
          FormGridRowType.DataRow,
          labelsColumn,
          controlsColumn,
          false);
        
        formGridRows.Add (formGridRow);
      }
      else if (hasTwoDataRows)
      {
        //  Two HtmlTableRows get combined into one FormGrid DataRow
        HtmlTableRow[] tableRows = new HtmlTableRow[2];
        tableRows[0] = rows[i];
        tableRows[1] = rows[i + 1];

        FormGridRow formGridRow = new FormGridRow (
          tableRows, 
          FormGridRowType.DataRow,
          labelsColumn,
          controlsColumn,
          false);
        
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
          controlsColumn,
          false);
       
        formGridRows.Add (formGridRow);
      }
    }

    return (FormGridRow[])formGridRows.ToArray(typeof(FormGridRow));
  }

  /// <summary> Validates all <see cref="BaseValidator"/> objects in the <see cref="FormGrid"/>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ValidateFormGrid/*' />
  private bool ValidateFormGrid (FormGrid formGrid)
  {
    bool isValid = true;
    foreach (FormGridRow formGridRow in formGrid.Rows)
    {
      if (formGridRow.Type != FormGridRowType.DataRow)
        continue;

      isValid &= ValidateDataRow (formGridRow);
    }
    return isValid;
  }

  /// <summary> Validates the <see cref="BaseValidator"/> objects. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ValidateDataRow/*' />
  protected bool ValidateDataRow (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    if (dataRow.ControlsCell == null)
      return true;

    dataRow.ValidationMarker = null;
    ArrayList validationErrorList = new ArrayList();

    if (!dataRow.Visible)
      return true;

    //  Check for validators and then their validation state
    //  Create a ValidationError object for each error
    //  Create the validationIcon

    bool isValid = true;
    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      IValidator validator = control as IValidator;

      //  Only for validators
      if (validator == null)
        continue;

      //  Validate
      validator.Validate();
      isValid &= validator.IsValid;
    }
    return isValid;
  }

  /// <summary>
  ///   Creates the appropriate validation marker and <see cref="ValidationError"/> objects and adds them to the 
  ///   <paramref name="dataRow"/>.
  /// </summary>
  /// <param name="dataRow"> The <see cref="FormGridRow"/> for which the validation errors will analyzed and registered. </param>
  private void RegisterValidationErrors (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    if (dataRow.ControlsCell == null)
      return;

    dataRow.ValidationMarker = null;
    ArrayList validationErrorList = new ArrayList();

    if (!dataRow.Visible)
      return;

    //  Check for validators and then their validation state
    //  Create a ValidationError object for each error
    //  Create the validationIcon

    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      IValidator validator = control as IValidator;
      if (validator == null || validator.IsValid)
        continue;

      //  Get control to validate
      Control controlToValidate = null;

      BaseValidator baseValidator = control as BaseValidator;
      IBaseValidator iBaseValidator = control as IBaseValidator;

      if (baseValidator != null)
        controlToValidate = control.NamingContainer.FindControl (baseValidator.ControlToValidate);
      else if (iBaseValidator != null)
        controlToValidate = control.NamingContainer.FindControl (iBaseValidator.ControlToValidate);

      //  Only visible controls: Build ValidationError
      if (controlToValidate.Visible)
        validationErrorList.Add (new ValidationError (controlToValidate, validator, dataRow.LabelsCell.Controls));
    }

    bool hasValidationErrors = validationErrorList.Count > 0;

    if (hasValidationErrors)
      dataRow.ValidationMarker = GetValidationMarker (string.Empty);

    dataRow.ValidationErrors = (ValidationError[])validationErrorList.ToArray (typeof (ValidationError));
  }

  protected void EnsureTransformationStep (TransformationStep stepToBeCompleted)
  {
    EnsureFormGridListPopulated();

    foreach (FormGrid formGrid in _formGrids.Values)
      EnsureTransformationStep (formGrid, stepToBeCompleted);
  }

  private void EnsureTransformationStep (FormGrid formGrid, TransformationStep stepToBeCompleted)
  {
    object boxedCompletedStep = _completedTransformationStep [formGrid];
    TransformationStep completedStep = TransformationStep.TransformationNotStarted;
    if (boxedCompletedStep != null)
      completedStep = (TransformationStep) boxedCompletedStep;

    if (   completedStep < TransformationStep.PreLoadViewStateTransformationCompleted 
        && completedStep < stepToBeCompleted)
    {
      completedStep = TransformIntoFormGridPreLoadViewState (formGrid);
    }
    if (   completedStep < TransformationStep.PostLoadTransformationCompleted 
        && completedStep < stepToBeCompleted)
    {
      completedStep = TransformIntoFormGridPostLoad (formGrid);
    }
    if (   completedStep < TransformationStep.PostValidationTransformationCompleted
        && completedStep < stepToBeCompleted)
    {
      completedStep = TransformIntoFormGridPostValidation (formGrid);
    }
  }

  private TransformationStep TransformIntoFormGridPreLoadViewState (FormGrid formGrid)
  {
    formGrid.Table.EnableViewState = false;

    formGrid.BuildIDCollection();
    LoadNewFormGridRows (formGrid);
    ApplyExternalHiddenSettings (formGrid);
    ComposeFormGridContents (formGrid);
    FormatFormGrid (formGrid);
    TransformationStep completedStep = TransformationStep.PreLoadViewStateTransformationCompleted;
    _completedTransformationStep[formGrid] = completedStep;
    return completedStep;
  }

  private TransformationStep TransformIntoFormGridPostLoad (FormGrid formGrid)
  {
    foreach (FormGridRow formGridRow in formGrid.Rows)
    {
      if (formGridRow.Type == FormGridRowType.DataRow)
      {
        CreateValidators (formGridRow);
        ApplyValidatorSettings (formGridRow);
      }
    }

    TransformationStep completedStep = TransformationStep.PostLoadTransformationCompleted;
    _completedTransformationStep[formGrid] = completedStep;
    return completedStep;
  }  

  private TransformationStep TransformIntoFormGridPostValidation (FormGrid formGrid)
  {
    foreach (FormGridRow formGridRow in formGrid.Rows)
    {
      if (formGridRow.IsGenerated)
        UpdateGeneratedRowsVisibility (formGridRow);

      if (!formGridRow.CheckVisibility())
      {
        formGridRow.Hide();
      }
      else
      {
        if (formGridRow.Type == FormGridRowType.DataRow)
        {
          CreateRequiredMarker (formGridRow);
          CreateHelpProvider(formGridRow);

          RegisterValidationErrors (formGridRow);

          LoadMarkersIntoCell (formGridRow);
          if (    ValidatorVisibility == ValidatorVisibility.ValidationMessageInControlsColumn
              ||  ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn)
          {
            LoadValidationMessagesIntoCell (formGridRow);
          }
        }
        AddShowEmptyCellsHack (formGridRow);
      }
    }

    TransformationStep completedStep = TransformationStep.PostValidationTransformationCompleted;
    _completedTransformationStep[formGrid] = completedStep;
    return completedStep;
  }

  /// <summary>
  ///   Queries the parent hierarchy for an <see cref="IFormGridRowProvider"/> and inserts 
  ///   the provided new rows into the form grid.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/LoadNewFormGridRows/*' />
  private void LoadNewFormGridRows (FormGrid formGrid)
  {
    IFormGridRowProvider rowProvider = GetFormGridRowProvider (this);

    if (rowProvider == null)
      return;

    FormGridRowInfoCollection formGridRowInfos = rowProvider.GetAdditionalRows (formGrid.Table);
    if (formGridRowInfos == null)
      return;

    foreach (FormGridRowInfo prototype in formGridRowInfos)
    {
      int rowCount = 1;
      if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowAfterLabel)
        rowCount = 2;

      HtmlTableRow[] htmlTableRows = new HtmlTableRow [rowCount];

      //  Row with label
      htmlTableRows[0] = new HtmlTableRow();
      for (int idxCells = 0, columnCount = ControlsColumn + 1; idxCells <= columnCount; idxCells++)
        htmlTableRows[0].Cells.Add (new HtmlTableCell());

      //  Row with control
      if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowAfterLabel)
      {
        htmlTableRows[1] = new HtmlTableRow();
        for (int idxCells = 0, columnCount = ControlsColumn + 1; idxCells <= columnCount; idxCells++)
          htmlTableRows[1].Cells.Add (new HtmlTableCell());
      }

      //  Control in labels row
      if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowWithLabel)
      {
        htmlTableRows[0].Cells[ControlsColumn].Controls.Add (prototype.Control);
      }
        //  Control in row after labels row
      else if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowAfterLabel)
      {
        htmlTableRows[1].Cells[LabelsColumn].Controls.Add (prototype.Control);
      }

      FormGridRow newFormGridRow = new FormGridRow (
        htmlTableRows, 
        FormGridRowType.DataRow, 
        LabelsColumn, 
        ControlsColumn,
        true);

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
  private void ApplyExternalHiddenSettings (FormGrid formGrid)
  {
    IFormGridRowProvider rowProvider = GetFormGridRowProvider (this);

    if (rowProvider == null)
      return;

    StringCollection strings = rowProvider.GetHiddenRows (formGrid.Table);
    if (strings == null)
      return; 

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
  private IFormGridRowProvider GetFormGridRowProvider (Control control)
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

  /// <summary> Find the <see cref="IResourceManager"/> for this <see cref="FormGridManager"/>. </summary>
  protected IResourceManager GetResourceManager()
  {
    //  Provider has already been identified.
    if (_cachedResourceManager != null)
        return _cachedResourceManager;

    //  Get the resource managers

    IResourceManager localResourceManager = 
        MultiLingualResourcesAttribute.GetResourceManager (typeof (ResourceIdentifier), true);
    IResourceManager namingContainerResourceManager = 
        ResourceManagerUtility.GetResourceManager (NamingContainer);
    _cachedResourceManager = new ResourceManagerSet (localResourceManager, namingContainerResourceManager);

    return _cachedResourceManager;
  }

  /// <summary>
  ///   Composes all information required to transform the <see cref="HtmlTable"/> 
  ///   into a form grid.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/ComposeFormGridContents/*' />
  private void ComposeFormGridContents (FormGrid formGrid)
  {
    if (formGrid == null) throw new ArgumentNullException ("formGrid");

    foreach (FormGridRow formGridRow in formGrid.Rows)
    {
      if (formGridRow.Type != FormGridRowType.DataRow)
        continue;
  
      formGridRow.SetLabelsCell (0, formGridRow.LabelsColumn);

      if (formGridRow.HtmlTableRows.Count == 1)
        formGridRow.SetControlsCell (0, formGridRow.ControlsColumn);
      else
        formGridRow.SetControlsCell (1, formGridRow.LabelsColumn);

      HandleReadOnlyControls (formGridRow);

      CreateLabels (formGridRow);
    }
  }

  /// <summary>
  ///   Uses the information stored in <paramref name="formGrid"/> to format the 
  ///   <see cref="HtmlTable"/> as a form grid.
  /// </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/FormatFormGrid/*' />
  private void FormatFormGrid (FormGrid formGrid)
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
        case FormGridRowType.UnknownRow:
        {
          FormatUnknownRow (formGridRow);
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

  /// <summary> Custom formats the unknown rows. </summary>
  protected virtual void FormatUnknownRow (FormGridRow row)
  {
    ArgumentUtility.CheckNotNull ("row", row);
    CheckFormGridRowType ("row", row, FormGridRowType.UnknownRow);

    HtmlTableCell cell;
    if (row.HtmlTableRows[0].Cells.Count > row.LabelsColumn)
      cell = row.SetLabelsCell (0, row.LabelsColumn);
    else
      cell = row.HtmlTableRows[0].Cells[row.HtmlTableRows[0].Cells.Count - 1];
   
    //  Adapt ColSpan for added markers column
    if (HasMarkersColumn)
    {
      cell.ColSpan++;
      row.ControlsColumn++;
    }

    //  Adapt ColSpan for added validation error message column
    if (HasValidationMessageColumn)
      cell.ColSpan++;

    if (!row.CheckVisibility())
      row.Hide();

    AddShowEmptyCellsHack (row);
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
  protected void CreateMarkersCell (FormGridRow dataRow)
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
      string toolTip = string.Empty;
      foreach (ValidationError validationError in dataRow.ValidationErrors)
      {
        //  Get validation message
        string validationMessage = validationError.ValidationMessage;
        //  Get tool tip, tool tip is validation message
        if (validationMessage != null && validationMessage.Length > 0)
        {
          if (toolTip.Length > 0)
            toolTip += Environment.NewLine;
          toolTip += validationMessage;
        }
      }
      dataRow.ValidationMarker.ToolTip = toolTip;
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
  ///   type <see cref="BaseValidator"/> inside the <paramref name="dataRow"/>.
  /// </summary>
  /// <param name="dataRow"> The <see cref="FormGridRow"/> containing the validators to be overridden. </param>
  protected virtual void ApplyValidatorSettings (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.ControlsCell", dataRow.ControlsCell);

    foreach (Control control in dataRow.ControlsCell.Controls)
    {
      BaseValidator baseValidator = control as BaseValidator;
      IBaseValidator iBaseValidator = control as IBaseValidator;

      //  Only for validators
      if (baseValidator == null && iBaseValidator == null)
        continue;
      
      //  FormGrid override
      if (ValidatorVisibility != ValidatorVisibility.ShowValidators)
      {
        if (baseValidator != null)
        {
          baseValidator.Display = ValidatorDisplay.None;
          baseValidator.EnableClientScript = false;
        }
        else if (iBaseValidator != null)
        {
          iBaseValidator.Display = ValidatorDisplay.None;
          iBaseValidator.EnableClientScript = false;
        }
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
      if (! control.Visible)
        continue;

      //  Query the controls for the string to be used as the labeling Text

      Control label = null;
      string newID = control.ID + c_generatedLabelSuffix;
      //  SmartLabel knows how the get the contents from ISmartControl
      if (control is ISmartControl)
      {
        SmartLabel smartLabel = new SmartLabel();
        smartLabel.ForControl = control.ID;
        label = smartLabel;
      }
        //  For these controls, the label's text will come from the resource dispatcher  
        //  auto:FormGridManagerUniqueID:TableUniqueID:ControlUniqueID_Label:Text
        //  auto:ControlUniqueID_Label:Text should also work
      else if (  control is TextBox 
              || control is ListControl
              || control is Table
              || control is HtmlInputControl
              || control is HtmlSelect
              || control is HtmlTextArea
              || control is HtmlTable)
      {
        Label primitiveLabel = new Label();
        if (! (control is DropDownList || control is HtmlSelect))
          primitiveLabel.AssociatedControlID = control.ID;
        label = primitiveLabel;
      }
      else
      {
        //  The control found in this iteration does not get handled by this method.
        continue;
      }

      label.ID = newID;

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

      //  Should be default, but better safe than sorry
      label.EnableViewState = true;

      dataRow.LabelsCell.Controls.Add(label);
    }
  }

  /// <summary>
  ///   Creates the validators from the controls inside <paramref name="dataRow"/> if they do not already exist.
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
      else if (control is BaseValidator || control is IBaseValidator)
        validators.Add (control);
    }
    
    foreach (ISmartControl smartControl in smartControls)
    {
      if (!smartControl.Visible)
        continue;

      //  Create Validators only if none are assigned for the SmartControl
      foreach (IValidator validator in validators)
      {
        BaseValidator baseValidator = validator as BaseValidator;
        IBaseValidator iBaseValidator = validator as IBaseValidator;
        if (   baseValidator != null
            && baseValidator.ControlToValidate == smartControl.ID)
        {
          return;
        }
        else if (   iBaseValidator != null
                 && iBaseValidator.ControlToValidate == smartControl.ID)
        {
          return;
        }
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
        helpUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (FormGridManager), ResourceType.HelpPage, helpUrl);
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

  protected void UpdateGeneratedRowsVisibility (FormGridRow dataRow)
  {
    ArgumentUtility.CheckNotNull ("dataRow", dataRow);
    CheckFormGridRowType ("dataRow", dataRow, FormGridRowType.DataRow);
    ArgumentUtility.CheckNotNull ("dataRow.LabelsCell", dataRow.LabelsCell);
    ArgumentUtility.CheckNotNull ("dataRow.ControlCell", dataRow.ControlsCell);

    if (dataRow.LabelsCell.Controls.Count == 1 && dataRow.ControlsCell.Controls.Count == 1)
    {
      dataRow.LabelsCell.Controls[0].Visible = dataRow.ControlsCell.Controls[0].Visible;
    }
    else
    {
      bool isControlVisible = false;
      foreach (Control control in dataRow.ControlsCell.Controls)
      {
        if (control.Visible)
        {
          isControlVisible = true;
          break;
        }
      }
      if (! isControlVisible)
      {
        foreach (Control label in dataRow.LabelsCell.Controls)
        {
          label.Visible = false;
        }
      }
    }
  }

  /// <summary> Queries the control for its read-only setting and transforms it if necessary. </summary>
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
          readOnlyValue.ID = textBox.ID;
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
  protected void SetOrCreateValidationMessagesCell (FormGridRow dataRow)
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

        dataRow.ValidationMessagesCell.Controls.Add (validationError.ToDiv (CssClassValidationMessage));
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
  protected string GetImageUrl (FormGridImage image)
  {
    string relativeUrl = image.ToString() + ImageExtension;

    string imageUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (FormGridManager), ResourceType.Image, relativeUrl);

    if (imageUrl != null)
      return imageUrl;
    else
      return relativeUrl;  
  }

  /// <summary> Builds the input required marker. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetRequiredMarker/*' />
  protected virtual Image GetRequiredMarker()
  {
    Image requiredIcon = new Image();
    requiredIcon.ImageUrl = GetImageUrl (FormGridImage.RequiredField);
    
    IResourceManager resourceManager = GetResourceManager();

    requiredIcon.AlternateText = resourceManager.GetString (ResourceIdentifier.RequiredFieldAlternateText);
    requiredIcon.ToolTip = resourceManager.GetString (ResourceIdentifier.RequiredFieldTitle);

    return requiredIcon;
  }

  /// <summary> Builds the help provider. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetHelpProvider/*' />
  protected virtual Control GetHelpProvider (string helpUrl)
  {
    Image helpIcon = new Image();
    helpIcon.ImageUrl = GetImageUrl (FormGridImage.Help);
 
    IResourceManager resourceManager = GetResourceManager();

    helpIcon.AlternateText = resourceManager.GetString (ResourceIdentifier.HelpAlternateText);
    helpIcon.ToolTip = resourceManager.GetString (ResourceIdentifier.HelpTitle);

    HtmlAnchor helpAnchor = new HtmlAnchor();
    helpAnchor.HRef = helpUrl;
    helpAnchor.Controls.Add (helpIcon);
    helpAnchor.Target = "_new";

    return helpAnchor;
  }

  /// <summary> Builds a new marker for validation errors. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetValidationMarker/*' />
  protected Image GetValidationMarker (string toolTip)
  {
    Image validationErrorIcon = new Image();
    validationErrorIcon.ImageUrl = GetImageUrl (FormGridImage.ValidationError);

    validationErrorIcon.ToolTip = toolTip;
 
    IResourceManager resourceManager = GetResourceManager();

    validationErrorIcon.AlternateText = 
        resourceManager.GetString (ResourceIdentifier.ValidationErrorInfoAlternateText);

    return validationErrorIcon;
  }

  /// <summary> Returns a spacer to be used instead of a marker. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/GetBlankMarker/*' />
  protected Control GetBlankMarker()
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
  protected bool HasSeperateControlsRow (FormGridRow dataRow)
  {
    return dataRow.LabelsRowIndex != dataRow.ControlsRowIndex;
  }

  protected void EnsureFormGridListPopulated()
  {
    if (! _formGridListPopulated)
    {
      PopulateFormGridList (NamingContainer);
      _formGridListPopulated = true;
    }
  }

  /// <summary> Registers all suffixed tables for this <c>FormGridManager</c>. </summary>
  /// <include file='doc\include\FormGridManager.xml' path='FormGridManager/PopulateFormGridList/*' />
  private void PopulateFormGridList (Control control)
  {
    //  Add all table having the suffix
    foreach (Control childControl in control.Controls)
    {
      HtmlTable htmlTable = childControl as HtmlTable;
      if (htmlTable != null && htmlTable.ID != null && htmlTable.ID.EndsWith (_formGridSuffix))
        Add (htmlTable);

      if (! (childControl is TemplateControl))
        PopulateFormGridList (childControl);
    }
  }

  public void Add (HtmlTable table)
  {
    if (! _formGrids.Contains (table.UniqueID))
    {
      FormGridRow[] rows = CreateFormGridRows (table, _labelsColumn, _controlsColumn);
      _formGrids[table.UniqueID] = new FormGrid (table, rows, _labelsColumn, _controlsColumn);
      table.Load += new EventHandler (Table_Load);
      table.PreRender += new EventHandler (Table_PreRender);
    }
  }

  [Browsable (false)]
  public HtmlTable[] Tables
  {
    get 
    {
      ArrayList tables = new ArrayList();
      foreach (FormGrid grid in _formGrids.Values)
        tables.Add (grid.Table);
      return (HtmlTable[]) tables.ToArray (typeof (HtmlTable));
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