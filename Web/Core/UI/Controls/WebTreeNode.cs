using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A node for the <see cref="WebTreeView"/>. </summary>
[TypeConverter (typeof (ExpandableObjectConverter))]
public class WebTreeNode: IControlItem
{
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;
  private string _itemID = "";
  private string _text = "";
  private IconInfo _icon;
  private WebTreeNodeCollection _children;
  private WebTreeView _treeView;
  private WebTreeNode _parentNode;
  private bool _isExpanded = false;
  private bool _isEvaluated = false;
  private bool _isSelected = false;
  int _selectDesired = 0;

  /// <summary> Initalizes a new instance. </summary>
  public WebTreeNode (string itemID, string text, IconInfo icon)
  {
    ItemID = itemID;
    _text = text;
    _icon = icon;
    _children = new WebTreeNodeCollection (null);
    _children.SetParent (null, this);
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTreeNode (string itemID, string text, string iconUrl)
    : this (itemID, text, new IconInfo (iconUrl, Unit.Empty, Unit.Empty))
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTreeNode (string itemID, string text)
    : this (itemID, text, string.Empty)
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTreeNode()
    : this (null, null, new IconInfo (string.Empty, Unit.Empty, Unit.Empty))
  {
  }

  //  /// <summary> Collapses the current node. </summary>
  //  public void Collapse()
  //  {
  //    IsExpanded = false;
  //  }
  //
  //  /// <summary> Collapses the current node and all child nodes. </summary>
  //  public void CollapseAll()
  //  {
  //    Collapse();
  //    _children.SetExpansion (false);
  //  }
  //  
  //  /// <summary> Expands the current node. </summary>
  //  public void Expand()
  //  {
  //    IsExpanded = true;
  //  }
  //  
  //  /// <summary> Expands the current node and all child nodes. </summary>
  //  public void ExpandAll()
  //  {
  //    Expand();
  //    _children.SetExpansion (true);
  //  }

  /// <summary> Evaluates the current node. </summary>
  public void Evaluate()
  {
    if (_treeView != null)
      _treeView.EvaluateTreeNodeInternal (this);
  }

  /// <summary> Evaluates and expands the current node. </summary>
  public void EvaluateExpand()
  {
    Evaluate();
    IsExpanded = true;
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary> Sets this node's <see cref="WebTreeView"/> and parent <see cref="WebTreeNode"/>. </summary>
  protected internal void SetParent (WebTreeView treeView, WebTreeNode parentNode)
  {
    _treeView = treeView; 
    if (_selectDesired == 1)
    {
      _selectDesired = 0;
      IsSelected = true;
    }
    else if (_selectDesired == -1)
    {
      _selectDesired = 0;
      IsSelected = false;
    }
    _parentNode = parentNode; 
    _children.SetParent (_treeView, this);
  }

  /// <summary> Sets the node's selection state. </summary>
  protected internal void SetSelected(bool value)
  {
    _isSelected = value;
    if (_treeView == null)
      _selectDesired = value ? 1 : -1;
  }

  public override string ToString()
  {
    string displayName = ItemID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Text;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets or sets the ID of this node. </summary>
  /// <remarks> Must be unique within the collection of tree nodes. Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this node.")]
  [NotifyParentProperty (true)]
  [ParenthesizePropertyName (true)]
  public virtual string ItemID
  {
    get { return _itemID; }
    set
    {
      //  Could not be added to collection in designer with this check enabled.
      //ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      if (! StringUtility.IsNullOrEmpty (value))
      {
        WebTreeNodeCollection nodes = null;
        if (ParentNode != null)
          nodes = ParentNode.Children;
        else if (TreeView != null)
          nodes = TreeView.Nodes;
        if (nodes != null)
        {
          if (nodes.Find (value) != null)
            throw new ArgumentException ("The collection already contains a node with ItemID '" + value + "'.", "value");
        }
      }
      _itemID = value; 
    }
  }

  private bool ShouldSerializeItemID()
  {
    return true;
  }

  /// <summary> Gets or sets the text displayed in this node. </summary>
  /// <remarks> Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text displayed in this node.")]
  [NotifyParentProperty (true)]
  public virtual string Text
  {
    get { return _text; }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      _text = value; 
    }
  }

  /// <summary> Gets or sets the icon displayed in this tree node. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The icon displayed in this tree node.")]
  [NotifyParentProperty (true)]
  public virtual IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  private bool ShouldSerializeIcon()
  {
    return IconInfo.ShouldSerialize (_icon);
  }

  private void ResetIcon()
  {
    _icon.Reset();
  }

  /// <summary> 
  ///   Gets the child nodes of for node. The node has to be evaluated before the actual child nodes can be accessed.
  /// </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [MergableProperty(false)]
  //  Default category
  [Description ("The child nodes contained in this tree node.")]
  [DefaultValue ((string) null)]
  public virtual WebTreeNodeCollection Children
  {
    get { return _children; }
  }

  /// <summary> Gets the <see cref="WebTreeView"/> to which this node belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTreeView TreeView
  {
    get { return _treeView; }
  }

  /// <summary> Gets the parent <see cref="WebTreeNode"/> of this node. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTreeNode ParentNode
  {
    get { return _parentNode; }
  }

  /// <summary> Gets or sets a flag that determines whether this node is expanded. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsExpanded
  {
    get { return _isExpanded; }
    set { _isExpanded = value; }
  }

  /// <summary> Gets or sets a flag that determines whether this node's child collection has been populated. </summary>
  /// <remarks> Does not evaluate the tree node. </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsEvaluated
  {
    get { return _isEvaluated; }
    set { _isEvaluated = value; }
  }

  /// <summary> Gets or sets a flag that determines whether this node is the selected node of the tree view. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsSelected
  {
    get { return _isSelected; }
    set 
    {
      SetSelected(value);
      if (_treeView != null)
      {
        if (value)
          _treeView.SetSelectedNode (this);
        else if (this == _treeView.SelectedNode)
          _treeView.SetSelectedNode (null);
      }
    }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "Node"; }
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
