using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Design;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Controls
{

//[Editor (typeof (WebTreeNodeCollectionEditor), typeof (UITypeEditor))]
public class WebTabCollection: ControlItemCollection
{
  private WebTabStrip _tabStrip;

  /// <summary> Initializes a new instance. </summary>
  public WebTabCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public WebTabCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (WebTab)})
  {
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new WebTab this[int index]
  {
    get { return (WebTab) List[index]; }
    set { List[index] = value; }
  }

  protected override void OnInsert(int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (WebTab));
    WebTab tab = (WebTab) value;
    CheckTab ("value", tab);
    base.OnInsert (index, value);
  }

  protected override void OnInsertComplete(int index, object value)
  {
    base.OnInsertComplete (index, value);
    WebTab tab = (WebTab) value;
    tab.SetParent (_tabStrip);
    InitalizeSelectedTab();
  }

  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (WebTab));
    WebTab tab = (WebTab) newValue;
    CheckTab ("newValue", tab);
    base.OnSet (index, oldValue, newValue);
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    base.OnSetComplete (index, oldValue, newValue);
    WebTab tab = (WebTab) newValue;
    tab.SetParent (_tabStrip);
  }

  private void CheckTab (string arguemntName, WebTab tab)
  {
    if (_tabStrip != null && ! ControlHelper.IsDesignMode ((Control) _tabStrip))
    {
      if (! tab.IsSeparator)
      {
        if (StringUtility.IsNullOrEmpty (tab.TabID))
          throw new ArgumentException ("The tab is no separator tab and does not contain a 'TabID'. It can therfor not be inserted into the collection.", arguemntName);
        if (Find (tab.TabID) != null)
          throw new ArgumentException ("The collection already contains a tab with TabID '" + tab.TabID + "'.", arguemntName);
      }
    }
  }

  protected internal void SetParent (WebTabStrip tabStrip)
  {
    _tabStrip = tabStrip; 
    foreach (WebTab tab in List)
      tab.SetParent (_tabStrip);
    InitalizeSelectedTab();
  }

  /// <summary>
  ///   Finds the <see cref="WebTab"/> with a <see cref="WebTab.TabID"/> of <paramref name="id"/>.
  /// </summary>
  /// <param name="id"> The ID to look for. </param>
  /// <returns> A <see cref="WebTab"/> or <see langword="null"/> if no matching tab was found. </returns>
  public WebTab Find (string id)
  {
    foreach (WebTab tab in InnerList)
    {
      if (tab.TabID == id)
        return tab;
    }
    return null;
  }

  private void InitalizeSelectedTab()
  {
    if (   _tabStrip != null 
        && (_tabStrip.Page == null || ! _tabStrip.Page.IsPostBack)
        && _tabStrip.SelectedTab == null 
        && InnerList.Count > 0)
    {
      WebTab currentTab = null;
      for (int i = 0; i < InnerList.Count; i++)
      {
        currentTab = (WebTab) InnerList[i];
        if (! currentTab.IsSeparator)
          break;
      }
      if (! currentTab.IsSeparator)
        _tabStrip.SetSelectedTab (currentTab);
    }
  }
}

}
