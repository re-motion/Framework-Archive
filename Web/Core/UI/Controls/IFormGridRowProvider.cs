using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   Interface for allowing the <see cref="FormGridManager"/> to query it's parent page
///   for rows to be isnerted and rows to be hidden.
/// </summary>
public interface IFormGridRowProvider
{
  StringCollection GetListOfHiddenRows (string table);
  FormGridRowPrototypeCollection GetListOfFormGridRowPrototypes (string table);
}

/// <summary>
/// 
/// </summary>
public class FormGridRowProvider
{
  private Hashtable _hiddenRows;

  private Hashtable _newRows;

  public FormGridRowProvider()
  {
    _hiddenRows = new Hashtable();
    _newRows = new Hashtable();
  }

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
