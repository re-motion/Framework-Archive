using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Design;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A collection of <see cref="WebTreeNode"/> objects. </summary>
[Editor (typeof (WebTreeNodeCollectionEditor), typeof (UITypeEditor))]
public class WebTreeNodeCollection: ControlItemCollection
{
  private WebTreeView _treeView;
  private WebTreeNode _parentNode;

  /// <summary> Initializes a new instance. </summary>
  public WebTreeNodeCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public WebTreeNodeCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (WebTreeNode)})
  {
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new WebTreeNode this[int index]
  {
    get { return (WebTreeNode) List[index]; }
    set { List[index] = value; }
  }

  protected override void OnInsert(int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (WebTreeNode));
    WebTreeNode node = (WebTreeNode) value;
    if (Find (node.NodeID) != null)
      throw new ArgumentException ("The collection already contains a node with NodeID '" + node.NodeID + "'.", "value");
    base.OnInsert (index, value);
  }

  protected override void OnInsertComplete(int index, object value)
  {
    base.OnInsertComplete (index, value);
    ((WebTreeNode) value).SetParent (_treeView, _parentNode);
  }

  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (WebTreeNode));
    WebTreeNode node = (WebTreeNode) newValue;
    if (Find (node.NodeID) != null)
      throw new ArgumentException ("The collection already contains a node with NodeID '" + node.NodeID + "'.", "newValue");
    base.OnSet (index, oldValue, newValue);
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    base.OnSetComplete (index, oldValue, newValue);
    ((WebTreeNode) newValue).SetParent (_treeView, _parentNode);
  }

  protected internal void SetParent (WebTreeView treeView, WebTreeNode parentNode)
  {
    _treeView = treeView; 
    _parentNode = parentNode; 
    foreach (WebTreeNode node in List)
      node.SetParent (_treeView, parentNode);
  }

  public WebTreeNode Find (string id)
  {
    foreach (WebTreeNode node in InnerList)
    {
      if (node.NodeID == id)
        return node;
    }
    return null;
  }

  public void SetExpandsion (bool expand)
  {
    foreach (WebTreeNode node in InnerList)
    {
      node.IsExpanded = expand;
      if (expand)
        node.ExpandAll();
      else
        node.CollapseAll();
    }
  }
}

}
