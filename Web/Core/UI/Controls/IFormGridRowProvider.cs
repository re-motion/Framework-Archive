using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   Interface for allowing the <see cref="FormGridManager"/> to query it's parent page
///   for rows to be isnerted and rows to be hidden.
/// </summary>
public interface IFormGridRowProvider: IPage
{
  /// <summary>
  ///   Returns a list of IDs identifying the rows to be hidden in a form grid.
  /// </summary>
  /// <param name="table">The ID of the <see cref="HtmlTable"/> whose rows will be hidden.</param>
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

}
