using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
public class WebTreeNode: IControlItem
{
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;
  private string _nodeID = "";
  private string _text = "";
  private string _icon = "";
  private WebTreeNodeCollection _children;
  private WebTreeView _treeView;
  private WebTreeNode _parentNode;
  private bool _isExpanded = false;
  private bool _isEvaluated = false;

  public WebTreeNode (string nodeID, string text, string icon)
  {
    NodeID = nodeID;
    _text = text;
    _icon = icon;
    _children = new WebTreeNodeCollection (null);
    _children.SetParent (null, this);
  }

  public WebTreeNode (string nodeID, string text)
    : this (nodeID, text, null)
  {
  }

  public WebTreeNode()
    : this (null, null, null)
  {
  }

  public void Collapse()
  {
    IsExpanded = false;
  }

  public void CollapseAll()
  {
    _children.SetExpandsion (false);
  }
  
  public void Expand()
  {
    IsExpanded = true;
  }
  
  public void ExpandAll()
  {
    _children.SetExpandsion (true);
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  protected internal void SetParent (WebTreeView treeView, WebTreeNode parentNode)
  {
    _treeView = treeView; 
    _parentNode = parentNode; 
    _children.SetParent (_treeView, this);
  }

  /// <summary>
  ///   Must be unique within the collection of tree nodes. Must not be <see langword="null"/> or emtpy.
  /// </summary>
  [DefaultValue ("")]
  public virtual string NodeID
  {
    get { return _nodeID; }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      WebTreeNodeCollection nodes = null;
      if (ParentNode != null)
        nodes = ParentNode.Children;
      else if (TreeView != null)
        nodes = TreeView.Nodes;
      if (nodes != null)
      {
        if (nodes.Find (value) != null)
          throw new ArgumentException ("The collection already contains a node with NodeID '" + value + "'.", "value");
      }

      _nodeID = value; 
    }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public virtual string Text
  {
    get { return _text; }
    set { _text = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public virtual string Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("The child nodes contained in this tree node.")]
  [DefaultValue ((string) null)]
  public WebTreeNodeCollection Children
  {
    get { return _children; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTreeView TreeView
  {
    get { return _treeView; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTreeNode ParentNode
  {
    get { return _parentNode; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsExpanded
  {
    get { return _isExpanded; }
    set { _isExpanded = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsEvaluated
  {
    get { return _isEvaluated; }
    set { _isEvaluated = value; }
  }

  /// <summary> Gets or sets the control to which this object belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public Control OwnerControl
  {
    get { return OwnerControlImplementation;  }
    set { OwnerControlImplementation = value; }
  }

  protected virtual Control OwnerControlImplementation
  {
    get { return _ownerControl;  }
    set
    { 
      if (_ownerControl != value)
      {
        _ownerControl = value;
        if (_children != null)
          _children.OwnerControl = value;
        OnOwnerControlChanged();
      }
    }
  }
}

}
