using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;

using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

public class ExtendedDropDownList : DropDownList, IResourceDispatchTarget
{
  public void InitListFromEnum (Type resourceEnumType)
  {
    InitListFromEnum (resourceEnumType, -1);
  }

  public virtual void ClearItems ()
  {
    this.Items.Clear ();
  }

  public void InitListFromEnum (Type resourceEnumType, int selectedValue)
  {
    ClearItems ();

    EnumValue[] enumValues = EnumDescription.GetAllValues (resourceEnumType);

    foreach (EnumValue enumValue in enumValues)
    {
      
      this.Add (enumValue.Description, (int) enumValue.NumericValue);
    }

    if (selectedValue != -1)
      this.SelectedInt32Value = selectedValue;
  }

  public void Add (string text, object value)
  {
    this.Items.Add (new ListItem (text, value.ToString()));
  }

  public ListItem GetListItemByValue (object value)
  {
    foreach (ListItem item in this.Items)
    {
      if (item.Value.Equals (value.ToString ()))
      {
        return item;
      }
    }

    return null;
  }

  public void Dispatch (IDictionary values)
  {
    foreach (DictionaryEntry entry in values)
    {
      string key = entry.Key.ToString ();
      int posColon = key.IndexOf (":");
      if (posColon >=0)
      {
        string value = key.Substring (0, posColon);
        string text = entry.Value.ToString ();        

        ListItem item = GetListItemByValue (value);
        if (item != null)
          item.Text = text;
      }
    }
  }

  public virtual int SelectedInt32Value
  {
    get { return int.Parse (base.SelectedValue); }
    set { SetSelectedValue (value); }
  }

  public virtual Guid SelectedGuidValue
  {
    get { return new Guid (base.SelectedValue); }
    set { SetSelectedValue (value); }
  }

  public new string SelectedValue
  {
    get { return base.SelectedValue; }
    set { SetSelectedValue (value); }
  }


  private void SetSelectedValue (object value)
  {
    for (int i = 0; i < this.Items.Count; ++i)
    {
      if (this.Items[i].Value.Equals (value.ToString ()))
      {
        SelectedIndex = i;
        return;
      }
    }

    throw new ArgumentOutOfRangeException (
        "value", value, "List does not contain an item with the specified value.");    
  }
}
}
