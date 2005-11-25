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

//[Editor (typeof (WebTabCollectionEditor), typeof (UITypeEditor))]
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

  protected override void OnValidate (object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (WebTab));

    WebTab tab = (WebTab) value;
    EnsureDesignModeTabInitialized (tab);
    if (StringUtility.IsNullOrEmpty (tab.ItemID))
      throw new ArgumentException ("The tab does not have an 'ItemID'. It can therfor not be inserted into the collection.", "value");
    base.OnValidate (value);
  }

  private void EnsureDesignModeTabInitialized (WebTab tab)
  {
    ArgumentUtility.CheckNotNull ("tab", tab);
    if (   StringUtility.IsNullOrEmpty (tab.ItemID)
        && _tabStrip != null && ControlHelper.IsDesignMode ((Control) _tabStrip))
    {
      int index = InnerList.Count;
      do {
        index++;
        string itemID = "Tab" + index.ToString();
        if (Find (itemID) == null)
        {
          tab.ItemID = itemID;
          if (StringUtility.IsNullOrEmpty (tab.Text))
          {
            tab.Text = "Tab " + index.ToString();
          }
          break;
        }
      } while (true);
    }
  }

  protected override void OnInsertComplete (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (WebTab));

    base.OnInsertComplete (index, value);
    WebTab tab = (WebTab) value;
    tab.SetParent (_tabStrip);
    InitalizeSelectedTab();
  }

  protected override void OnSetComplete (int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (WebTab));

    base.OnSetComplete (index, oldValue, newValue);
    WebTab tab = (WebTab) newValue;
    tab.SetParent (_tabStrip);
  }

  public void AddRange (WebTabCollection values)
  {
    base.AddRange (values);
  }

  protected internal void SetParent (WebTabStrip tabStrip)
  {
    ArgumentUtility.CheckNotNull ("tabStrip", tabStrip);

    _tabStrip = tabStrip; 
    for (int i = 0; i < InnerList.Count; i++)
      ((WebTab) InnerList[i]).SetParent (_tabStrip);
    InitalizeSelectedTab();
  }

  /// <summary>
  ///   Finds the <see cref="WebTab"/> with a <see cref="WebTab.ItemID"/> of <paramref name="id"/>.
  /// </summary>
  /// <param name="id"> The ID to look for. </param>
  /// <returns> A <see cref="WebTab"/> or <see langword="null"/> if no matching tab was found. </returns>
  public new WebTab Find (string id)
  {
    return (WebTab) base.Find (id);
  }

  private void InitalizeSelectedTab()
  {
    if (   _tabStrip != null 
        && (_tabStrip.Page == null || ! _tabStrip.Page.IsPostBack)
        && _tabStrip.SelectedTab == null 
        && InnerList.Count > 0)
    {
      _tabStrip.SetSelectedTab ((WebTab) InnerList[0]);
    }
  }
}

}
