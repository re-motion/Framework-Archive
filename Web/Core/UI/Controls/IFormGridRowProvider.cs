using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   Interface for allowing the <see cref="FormGridManager"/> to query it's parent page
///   for rows to be isnerted and rows to be hidden.
/// </summary>
public interface IFormGridRowProvider
{
  /// <summary>
  ///   Returns a list of IDs identifying the rows to be hidden in a form grid.
  /// </summary>
  /// <param name="table">The <see cref="HtmlTable"/> whose rows will be hidden.</param>
  /// <returns>A <see cref="StringCollection"/> containing the IDs.</returns>
  StringCollection GetListOfHiddenRows (string table);

  /// <summary>
  ///   Returns a list of <see cref="FormGridRowPrototype"/> objects used to contrstuct and then 
  ///   insert new rows into a form grid.
  /// </summary>
  /// <param name="table">The <see cref="HtmlTable"/> into which the new rows will be inserted.</param>
  /// <returns>A <see cref="FormGridRowPrototypeCollection"/> containing the prototypes.</returns>
  FormGridRowPrototypeCollection GetListOfFormGridRowPrototypes (string table);
}

/// <summary>
///   A class encapsulating the functionality of <see cref="IFormGridRowProvider"/>.
/// </summary>
/// <remarks>
///   Implementing the interface is reduced to holding an instance to this class and calling
///   the instance methods <see cref="GetListOfHiddenRows"/> and 
///   <see cref="GetListOfFormGridRowPrototypes"/> from the respective interface methods.
/// </remarks>
public class FormGridRowProvider
{
  /// <summary> The <see cref="Hashtable"/> of the hidden rows lists, using the table's ID as key. </summary>
  private Hashtable _hiddenRows;

  /// <summary> The <see cref="Hashtable"/> of the new rows lists, using the table's ID as key. </summary>
  private Hashtable _newRows;

  /// <summary> Simple constructor. </summary>
  public FormGridRowProvider()
  {
    _hiddenRows = new Hashtable();
    _newRows = new Hashtable();
  }

  /// <summary>
  ///   Returns a list of IDs identifying the rows to be hidden in a form grid.
  /// </summary>
  /// <remarks> 
  ///   Call this method from the implementation of IFormGridRowProvider.GetListOfHiddenRows.
  /// </remarks>
  /// <param name="table">The <see cref="HtmlTable"/> whose rows will be hidden.</param>
  /// <returns>A <see cref="StringCollection"/> containing the IDs.</returns>
  public StringCollection GetListOfHiddenRows (string table)
  {
    StringCollection rows = _hiddenRows[table.GetHashCode()] as StringCollection;

    if (rows == null)
    {
      rows = new StringCollection();
      _hiddenRows[table.GetHashCode()] = rows;
    }

    return rows;
  }

  /// <summary>
  ///   Returns a list of <see cref="FormGridRowPrototype"/> objects used to contrstuct and then 
  ///   insert new rows into a form grid.
  /// </summary>
  /// <remarks> 
  ///   Call this method from the implementation of IFormGridRowProvider.GetListOfFormGridRowPrototypes.
  /// </remarks>
  /// <param name="table">The <see cref="HtmlTable"/> into which the new rows will be inserted.</param>
  /// <returns>A <see cref="FormGridRowPrototypeCollection"/> containing the prototypes.</returns>
  public FormGridRowPrototypeCollection GetListOfFormGridRowPrototypes (string table)
  {
    FormGridRowPrototypeCollection rows =
      _newRows[table.GetHashCode()] as FormGridRowPrototypeCollection;

    if (rows == null)
    {
      rows = new FormGridRowPrototypeCollection();
      _newRows[table.GetHashCode()] = rows;
    }

    return rows;
  }
}

}
