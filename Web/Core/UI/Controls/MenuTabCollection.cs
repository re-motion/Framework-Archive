using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using Rubicon.Utilities;
using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UI.Controls
{

[Editor (typeof (MainMenuTabCollectionEditor), typeof (UITypeEditor))]
public class MainMenuTabCollection: WebTabCollection
{
  /// <summary> Initializes a new instance. </summary>
  public MainMenuTabCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public MainMenuTabCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (SubMenuTab)})
  {
  }

  public int Add (MainMenuTab tab)
  {
    return base.Add (tab);
  }

  public void AddRange (params MainMenuTab[] tabs)
  {
    base.AddRange (tabs);
  }

  public void Insert (int index, MainMenuTab tab)
  {
    base.Insert (index, tab);
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new MainMenuTab this[int index]
  {
    get { return (MainMenuTab) List[index]; }
    set { List[index] = value; }
  }
}

[Editor (typeof (SubMenuTabCollectionEditor), typeof (UITypeEditor))]
public class SubMenuTabCollection: WebTabCollection
{
  private MainMenuTab _parent;

  /// <summary> Initializes a new instance. </summary>
  public SubMenuTabCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public SubMenuTabCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (SubMenuTab)})
  {
  }

  protected override void OnInsertComplete (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (SubMenuTab));

    base.OnInsertComplete (index, value);
    SubMenuTab tab = (SubMenuTab) value;
    tab.SetParent (_parent);
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (SubMenuTab));

    base.OnSetComplete (index, oldValue, newValue);
    SubMenuTab tab = (SubMenuTab) newValue;
    tab.SetParent (_parent);
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public MainMenuTab Parent
  {
    get { return _parent; }
  }

  protected internal void SetParent (MainMenuTab parent)
  {
    ArgumentUtility.CheckNotNull ("parent", parent);
    _parent = parent;
    for (int i = 0; i < InnerList.Count; i++)
      ((SubMenuTab) InnerList[i]).SetParent (_parent);
  }

  public int Add (SubMenuTab tab)
  {
    return base.Add (tab);
  }

  public void AddRange (params SubMenuTab[] tabs)
  {
    base.AddRange (tabs);
  }

  public void Insert (int index, SubMenuTab tab)
  {
    base.Insert (index, tab);
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new SubMenuTab this[int index]
  {
    get { return (SubMenuTab) List[index]; }
    set { List[index] = value; }
  }
}

}
