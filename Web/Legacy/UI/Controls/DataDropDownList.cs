using System;
using System.Web;
using System.Web.UI.WebControls;

namespace Rubicon.Findit.Client.Controls
{

/// <summary>
/// This drop down list class provides direct access to Int32 list values
/// </summary>
/// <remarks>
/// Can be used for accessing database IDs of list entries.
/// </remarks>
public class DataDropDownList: DropDownList
{
  // types

  // static members and constants

  // member fields

  bool _isRequired = false;
  int _emptyValue = -1;

  // construction and disposing

  public DataDropDownList()
    : base ()
  {
    AddEmptyItem();
  }

  // methods and properties

  /// <summary>
  /// Specifies whether the user must select a valid item in this list.
  /// </summary>
  /// <remarks>
  /// The drop down list contains an empty item that represents the state 'no item selected'. If IsRequired
  /// is true and an item is already selected, the empty item is not displayed.
  /// This property is not saved in the control's view state. Default is false;
  /// </remarks>
  public bool IsRequired
  {
    get { return _isRequired; }
    set { _isRequired = value; }
  }

  public int EmptyValue
  {
    get { return _emptyValue; }
    set { _emptyValue = value; }
  }

  /// <summary>
  /// Returns the value of the selected item, or -1 if no item or the empty item is selected.
  /// </summary>
  public int SelectedValue
  {
    get
    {
      ListItem item = SelectedItem;
      if (item == null)
        return -1;
      
      try
      {
        int val = int.Parse(item.Value);
        if (val == -1)
          return _emptyValue;
        else
          return val;
      }
      catch (FormatException)
      {
        return -1;
      }
    }
    set
    {
      int val = value;
      if (val == _emptyValue)
      {
        SetEmpty();
      }

      for (int i = 0; i < this.Items.Count; ++i)
      {
        try
        {
          if (val == int.Parse (this.Items[i].Value))
          {
            SelectedIndex = i;
            return;
          }
        }
        catch (FormatException)
        {
        }
      }

      throw new ArgumentOutOfRangeException (
          "value", value, "List does not contain an item with the specified value.");
    }
  }

  protected override void OnPreRender (EventArgs e)
  {
    // delete all empty items (except the one that is selected)
    if (_isRequired)
    {
      for (int i = this.Items.Count - 1; i >= 0; --i)
      {
        ListItem item = this.Items[i];
        if (this.SelectedIndex != i && item.Value == "-1")
          this.Items.RemoveAt (i);
      }
    }

    base.OnPreRender (e);
  }

  /// <summary>
  /// Removes all items except the 'empty' item.
  /// </summary>
  /// <remarks>
  /// Do not use Items.Clear().
  /// </remarks>
  public void ClearItems()
  {
    this.Items.Clear();
    AddEmptyItem();
  }

  public bool IsEmpty()
  {
    return this.SelectedValue == this.EmptyValue;
  }

  public void SetEmpty()
  {
    if (this.Items.Count < 1 || this.Items[0].Value != "-1")
      AddEmptyItem();
    this.SelectedIndex = 0;
  }

  public void Add (string text, int value)
  {
    this.Items.Add (new ListItem (text, value.ToString()));
  }

  private void AddEmptyItem()
  {
    this.Items.Insert (0, new ListItem (string.Empty, "-1"));
  }
}

}
