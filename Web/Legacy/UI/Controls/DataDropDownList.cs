using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;

using Rubicon.Globalization;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
/// This drop down list class provides direct access to Int32 list values
/// </summary>
/// <remarks>
/// Can be used for accessing database IDs of list entries.
/// </remarks>
public class DataDropDownList: ExtendedDropDownList
{
  // types

  // static members and constants

  private const int c_emptyIntValue = -1;
  private readonly Guid c_emptyGuidValue = Guid.Empty;
  private readonly string c_emptyStringValue = string.Empty;

  // member fields

  bool _isRequired = false;

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

  /// <summary>
  /// Returns the value of the selected item, or -1 if no item or the empty item is selected.
  /// </summary>
  public override int SelectedInt32Value
  {
    get 
    { 
      string selectedValue = GetSelectedValue (); 
      if (selectedValue != c_emptyStringValue)
        return int.Parse (selectedValue);
      else
        return c_emptyIntValue;
    }
    set 
    {
      string val;
      if (value != c_emptyIntValue)
        val = value.ToString ();
      else
        val = c_emptyStringValue;
        
      SetSelectedValue (val); 
    }
  }

  /// <summary>
  /// Returns the value of the selected item, or Guid.Empty if no item or the empty item is selected.
  /// </summary>
  public override Guid SelectedGuidValue
  {
    get 
    { 
      string selectedValue = GetSelectedValue (); 
      if (selectedValue != c_emptyStringValue)
        return new Guid (selectedValue);
      else
        return c_emptyGuidValue;
    }
    set 
    {
      string val;
      if (value != c_emptyGuidValue)
        val = value.ToString ();
      else
        val = c_emptyStringValue;
        
      SetSelectedValue (val); 
    }
  }

  /// <summary>
  /// Returns the value of the selected item, or string.Empty if no item or the empty item is selected.
  /// </summary>
  public new string SelectedValue
  {
    get { return GetSelectedValue (); }
    set { SetSelectedValue (value); }
  }

  private void SetSelectedValue (string value)
  {
    string val = value;
    if (val == c_emptyStringValue)
    {
      SetEmpty();
      return;
    }

    base.SelectedValue = val;
  }

  private string GetSelectedValue ()
  {
    ListItem item = SelectedItem;
    if (item == null)
      return c_emptyStringValue;
    
    if (item.Value != c_emptyStringValue)
      return item.Value;    
    else
      return c_emptyStringValue;
      
  }

  protected override void OnPreRender (EventArgs e)
  {
    // delete all empty items (except the one that is selected)
    if (_isRequired)
    {
      for (int i = this.Items.Count - 1; i >= 0; --i)
      {
        ListItem item = this.Items[i];
        if (this.SelectedIndex != i && item.Value == c_emptyStringValue)
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
  public override void ClearItems()
  {
    this.Items.Clear();
    AddEmptyItem();
  }

  public bool IsEmpty
  {
    get { return this.SelectedValue == c_emptyStringValue; }
  }

  public void SetEmpty()
  {
    if (this.Items.Count < 1 || this.Items[0].Value != c_emptyStringValue)
      AddEmptyItem();

    this.SelectedIndex = 0;
  }

  public bool Contains (object value)
  {
    return (Items.FindByValue (value.ToString()) != null);
  }

  private void AddEmptyItem()
  {
    this.Items.Insert (0, new ListItem (string.Empty, c_emptyStringValue));
  }
}
}
