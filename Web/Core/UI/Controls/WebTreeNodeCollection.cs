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
    CheckNode ("value", node);
    base.OnInsert (index, value);
  }

  protected override void OnInsertComplete(int index, object value)
  {
    base.OnInsertComplete (index, value);
    WebTreeNode node = (WebTreeNode) value;
    node.SetParent (_treeView, _parentNode);
    if (node.IsSelected)
      node.SetSelected (true);
  }

  protected override void OnSet(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (WebTreeNode));
    WebTreeNode node = (WebTreeNode) newValue;
    CheckNode ("newValue", node);
    base.OnSet (index, oldValue, newValue);
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    base.OnSetComplete (index, oldValue, newValue);
    WebTreeNode node = (WebTreeNode) newValue;
    node.SetParent (_treeView, _parentNode);
    if (node.IsSelected)
      node.SetSelected (true);
  }

  private void CheckNode (string arguemntName, WebTreeNode node)
  {
    if (_treeView != null && ! ControlHelper.IsDesignMode ((IControl) _treeView))
    {
      if (StringUtility.IsNullOrEmpty (node.NodeID))
        throw new ArgumentException ("The node does not contain a 'NodeID' and can therfor not be inserted into the collection.", arguemntName);
    }
    if (Find (node.NodeID) != null)
      throw new ArgumentException ("The collection already contains a node with NodeID '" + node.NodeID + "'.", arguemntName);
  }

  protected internal void SetParent (WebTreeView treeView, WebTreeNode parentNode)
  {
    _treeView = treeView; 
    _parentNode = parentNode; 
    foreach (WebTreeNode node in List)
      node.SetParent (_treeView, parentNode);
  }

  /// <summary>
  ///   Finds the <see cref="WebTreeNode"/> with a <see cref="WebTreeNode.NodeID"/> of <paramref name="id"/>.
  /// </summary>
  /// <param name="id"> The ID to look for. </param>
  /// <returns> A <see cref="WebTreeNode"/> or <see langword="null"/> if no mathcing node was found. </returns>
  public WebTreeNode Find (string id)
  {
    foreach (WebTreeNode node in InnerList)
    {
      if (node.NodeID == id)
        return node;
    }
    return null;
  }

  //  /// <summary>
  //  ///   Sets the <see cref="WebTreeNode.IsExpanded"/> of all nodes in this collection, including all child nodes.
  //  /// </summary>
  //  /// <param name="expand"> <see langword="true"/> to expand all nodes, <see langword="false"/> to collapse them. </param>
  //  public void SetExpansion (bool expand)
  //  {
  //    foreach (WebTreeNode node in InnerList)
  //    {
  //      node.IsExpanded = expand;
  //      if (expand)
  //        node.ExpandAll();
  //      else
  //        node.CollapseAll();
  //    }
  //  }
}

}
