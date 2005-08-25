using System;
using System.Collections;
using System.Collections.Specialized;

using Microsoft.Web.UI.WebControls;

using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

[CLSCompliant (false)] // because Microsoft.Web.UI.WebControls.BasePostBackControl is not CLS compliant
public class TabStrip : Microsoft.Web.UI.WebControls.TabStrip, IResourceDispatchTarget
{
  protected override RenderPathID RenderPath
  {
    get 
    { 
    	if (this.IsDesignMode)
		    return RenderPathID.DesignerPath;
      else
	  	  return RenderPathID.DownLevelPath;
    }
  }

  public bool ContainsTabID (string tabID)
  {
    foreach (Microsoft.Web.UI.WebControls.TabItem tab in this.Items)
    {
      if (tab.ID == tabID)
        return true;
    }

    return false;
  }

  public TabItem GetTabItem (string tabID)
  {
    foreach (TabItem tab in Items)
    {
      if (tab.ID == tabID)
        return tab;
    }

    return null;
  }

  public bool ShowTab (string tabID)
  {
    int tabCount = 0;

    for (int i = 0; i < this.Items.Count; ++i)
    { 
      if ( this.Items[i].GetType() == typeof( Microsoft.Web.UI.WebControls.Tab ) )
        tabCount++;

      if (this.Items[i].ID == tabID)
      {
        this.SelectedIndex = tabCount - 1; //  '-1' because first tab has index 0
        return true;
      }
    }

    return false;
  }

  public void Dispatch (IDictionary values)
  {
    foreach (DictionaryEntry entry in values)
    {
      string key = entry.Key.ToString ();
      int posColon = key.IndexOf (":");
      if (posColon >=0)
      {
        string tabName = key.Substring (0, posColon);
        string text = entry.Value.ToString ();        
        
        TabItem tab = GetTabByName (tabName);
        tab.Text = text;
      }
    }
  }

  public TabItem GetTabByName (string tabName)
  {
    foreach (TabItem tab in this.Items)
    {
      if (tab.ID == tabName)
        return tab;
    }

    return null;
  }

  public TabItem GetSelectedTab ()
  {
    int tabIndex = 0;
    for (int i = 0; i < Items.Count; i++)
    {
      object tabStripItem = Items[i];
      if ((tabStripItem as Microsoft.Web.UI.WebControls.Tab) != null)
      {
        if (tabIndex == SelectedIndex)
          return Items[i];

        tabIndex++;
      }
    }

    return null;
  }
}

[CLSCompliant (false)] // because Microsoft.Web.UI.WebControls.BasePostBackControl is not CLS compliant
public class MultiPage : Microsoft.Web.UI.WebControls.MultiPage
{
  protected override RenderPathID RenderPath
  {
    get 
    { 
    	if (this.IsDesignMode)
		    return RenderPathID.DesignerPath;
      else
	  	  return RenderPathID.DownLevelPath;
    }
  }

}
  
[CLSCompliant (false)] // because Microsoft.Web.UI.WebControls.BaseRichControl is not CLS compliant
public class PageView : Microsoft.Web.UI.WebControls.PageView
{
  protected override RenderPathID RenderPath
  {
    get 
    { 
    	if (this.IsDesignMode)
		    return RenderPathID.DesignerPath;
      else
	  	  return RenderPathID.DownLevelPath;
    }
  }

}
}
