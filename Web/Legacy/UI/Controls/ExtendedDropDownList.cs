using System;
using System.Collections;
using System.Web.UI.WebControls;

using Rubicon;
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
      this.SelectedValue = selectedValue;
  }

  public void Add (string text, int value)
  {
    this.Items.Add (new ListItem (text, value.ToString()));
  }

  public ListItem GetListItemByValue (string value)
  {
    foreach (ListItem item in this.Items)
    {
      if (item.Value == value)
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

  public new int SelectedValue
  {
    get 
    { 
      return int.Parse (base.SelectedValue); 
    }
    set 
    {  
      for (int i = 0; i < this.Items.Count; ++i)
      {
        try
        {
          if (value == int.Parse (this.Items[i].Value))
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
}
}
