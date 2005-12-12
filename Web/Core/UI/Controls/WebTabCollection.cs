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
public class WebTabCollection: ControlItemCollection, IControlStateManager
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

  protected override void ValidateNewValue (object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (WebTab));

    WebTab tab = (WebTab) value;
    EnsureDesignModeTabInitialized (tab);
    if (StringUtility.IsNullOrEmpty (tab.ItemID))
      throw new ArgumentException ("The tab does not have an 'ItemID'. It can therfor not be inserted into the collection.", "value");
    base.ValidateNewValue (value);
  }

  private void EnsureDesignModeTabInitialized (WebTab tab)
  {
    ArgumentUtility.CheckNotNull ("tab", tab);
    if (   StringUtility.IsNullOrEmpty (tab.ItemID)
        && (   _tabStrip != null && ControlHelper.IsDesignMode ((Control) _tabStrip))
            || (OwnerControl != null && ControlHelper.IsDesignMode ((Control) OwnerControl)))
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
    tab.SetTabStrip (_tabStrip);
    InitalizeSelectedTab();
  }

  protected override void OnSetComplete (int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("oldValue", oldValue, typeof (WebTab));
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (WebTab));

    base.OnSetComplete (index, oldValue, newValue);

    WebTab newTab = (WebTab) newValue;
    newTab.SetTabStrip (_tabStrip);

    WebTab oldTab = (WebTab) oldValue;
    oldTab.SetTabStrip (null);
    
    if (_tabStrip != null && oldTab.IsSelected)
    {
      bool isLastTab = index + 1 == InnerList.Count;
      if (isLastTab)
      {
        if (InnerList.Count > 1)
        {
          WebTab penultimateTab = (WebTab) InnerList[index - 1];
          _tabStrip.SetSelectedTabInternal (penultimateTab);
        }
        else
        {
          _tabStrip.SetSelectedTabInternal (null);
        }
      }
      else
      {
        WebTab nextTab = (WebTab) InnerList[index + 1];
        _tabStrip.SetSelectedTabInternal (nextTab);
      }
    }
  }

  protected override void OnRemoveComplete (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (WebTab));

    base.OnRemoveComplete (index, value);
    
    WebTab tab = (WebTab) value;
    tab.SetTabStrip (null);
    if (_tabStrip != null && tab.IsSelected)
    {
      bool wasLastTab = index == InnerList.Count;
      if (wasLastTab)
      {
        if (InnerList.Count > 1)
        {
          WebTab lastTab = (WebTab) InnerList[index - 1];
          _tabStrip.SetSelectedTabInternal (lastTab);
        }
        else
        {
          _tabStrip.SetSelectedTabInternal (null);
        }
      }
      else
      {
        WebTab nextTab = (WebTab) InnerList[index];
        _tabStrip.SetSelectedTabInternal (nextTab);
      }
    }
  }

  protected override void OnClear()
  {
    base.OnClear ();
    for (int i = 0; i < InnerList.Count; i++)
    {
      WebTab tab = (WebTab) InnerList[i];
      tab.SetTabStrip (null);
    }
    if (_tabStrip != null)
      _tabStrip.SetSelectedTabInternal (null);
  }

  public void AddRange (WebTabCollection values)
  {
    base.AddRange (values);
  }

  protected internal void SetTabStrip (WebTabStrip tabStrip)
  {
    ArgumentUtility.CheckNotNull ("tabStrip", tabStrip);

    _tabStrip = tabStrip; 
    for (int i = 0; i < InnerList.Count; i++)
      ((WebTab) InnerList[i]).SetTabStrip (_tabStrip);
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
      _tabStrip.SetSelectedTabInternal ((WebTab) InnerList[0]);
    }
  }

  void IControlStateManager.LoadControlState (object state)
  {
    LoadControlState (state);
  }

  protected virtual void LoadControlState (object state)
  {
    if (state == null)
      return;

    Pair[] tabsState = (Pair[]) state;
    for (int i = 0; i < tabsState.Length; i++)
    {
      Pair pair = tabsState[i];
      string itemID = (string) pair.First;
      WebTab tab = Find (itemID);
      if (tab != null)
        ((IControlStateManager) tab).LoadControlState (pair.Second);
    }
  }

  object IControlStateManager.SaveControlState ()
  {
    return SaveControlState();
  }

  protected virtual object SaveControlState()
  {
    ArrayList tabsState = new ArrayList();
    for (int i = 0; i < InnerList.Count; i++)
    {
      WebTab tab = (WebTab) InnerList[i];   
      object tabStateValue = ((IControlStateManager) tab).SaveControlState();
      if (tabStateValue != null)
      {
        Pair pair = new Pair();
        pair.First = tab.ItemID;
        pair.Second = tabStateValue;
        tabsState.Add (pair);
      }
    }
    if (tabsState.Count == 0)
      return null;
    else
      return (Pair[]) tabsState.ToArray (typeof (Pair));
  }
}

}
