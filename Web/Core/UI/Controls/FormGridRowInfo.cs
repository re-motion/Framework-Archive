using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;

namespace Rubicon.Web.UI.Controls
{
/// <summary> The prototype for the automatic creation of a new <see cref="FormGridManager.FormGridRow"/>. </summary>
public class FormGridRowPrototype
{
  /// <summary> The possible positions for inserting the new row relative to a given ID. </summary>
  public enum RowPosition
  {
    /// <summary> Place the row before the row containing the ID. </summary>
    BeforeRowWithID,

    /// <summary> Place the row after the row containing the ID. </summary>
    AfterRowWithID
  }

  /// <summary> The possible layouts for the new <see cref="FormGridManager.FormGridRow"/>. </summary>
  public enum RowType
  {
    /// <summary> Label and Control will be placed into the same row. </summary>
    ControlInRowWithLabel,

    /// <summary> The Control will be placed in a seperat row, following the Label's row. </summary>
    ControlInRowAfterLabel
  }

  /// <summary> The control to the inserted into the row. </summary>
  private Control _control;

  /// <summary>
  ///   The <see cref="RowType"/> for the new <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  private RowType _newRowType;

  /// <summary>
  ///   The <see cref="RowPosition"/> for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  private RowPosition _positionInFormGrid;

  /// <summary>
  ///   The row used as a point of reference for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  private string _releatedRowID;

  /// <summary> Simple Constructor </summary>
  /// <param name="control"> The control to the inserted into the row. </param>
  /// <param name="newRowType">
  ///   The <see cref="RowType"/> for the new <see cref="FormGridManager.FormGridRow"/>.
  /// </param>
  /// <param name="relatedRowID">
  ///   The row used as a point of reference for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </param>
  /// <param name="positionInFormGrid">
  ///   The <see cref="RowPosition"/> for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </param>
  public FormGridRowPrototype (
    Control control, 
    RowType newRowType,
    string relatedRowID,
    RowPosition positionInFormGrid)
  {
    _control = control;
    _newRowType = newRowType;
    _positionInFormGrid = positionInFormGrid;
    _releatedRowID = relatedRowID;
  }

  /// <summary>  The control to the inserted into the row.  </summary>
  public Control Control
  {
    get { return _control; }
  }

  /// <summary>
  ///   The <see cref="RowType"/> for the new <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  public RowType NewRowType
  {
    get { return _newRowType; }
  }

  /// <summary> 
  ///   The row used as a point of reference when inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  public string ReleatedRowID
  {
    get { return _releatedRowID; }
  }
 
  /// <summary> 
  ///   The <see cref="RowPosition"/> for inserting the new <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  public RowPosition PositionInFormGrid
  {
    get { return _positionInFormGrid; }
  }
}

}
