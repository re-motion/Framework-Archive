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
    base.OnInsert (index, value);
  }

  protected override void OnInsertComplete(int index, object value)
  {
    base.OnInsertComplete (index, value);
    WebTab tab = (WebTab) value;
    tab.SetParent (_tabStrip);
    if (tab.IsSelected)
      tab.SetSelected (true);
  }

  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (WebTab));
    base.OnSet (index, oldValue, newValue);
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    base.OnSetComplete (index, oldValue, newValue);
    WebTab tab = (WebTab) newValue;
    tab.SetParent (_tabStrip);
    if (tab.IsSelected)
      tab.SetSelected (true);
  }

  protected internal void SetParent (WebTabStrip tabStrip)
  {
    _tabStrip = tabStrip; 
    foreach (WebTab tab in List)
      tab.SetParent (_tabStrip);
  }
}

}
