using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{

[ToolboxData("<{0}:WebTreeView runat=server></{0}:WebTreeView>")]
public class WebTreeView : WebControl
{
  private WebTreeNodeCollection _nodes;

  public WebTreeView (Control ownerControl)
  {
    _nodes = new WebTreeNodeCollection (ownerControl);
    _nodes.SetParent (this, null);
  }

  public WebTreeView()
    :this (null)
  {
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("The tree nodes displayed by this tree view.")]
  [DefaultValue ((string) null)]
  public WebTreeNodeCollection Nodes
  {
    get { return _nodes; }
  }
}

[TypeConverter (typeof (ExpandableObjectConverter))]
public class WebTreeNode: IControlItem
{
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;
  private string _nodeID = "";
  private string _text = "";
  private string _icon = "";
  private WebTreeNodeCollection _nodes;
  private WebTreeView _treeView;
  private WebTreeNode _parentNode;

  public WebTreeNode (string nodeID, string text, string icon)
  {
    _nodeID = nodeID;
    _text = text;
    _icon = icon;
    _nodes = new WebTreeNodeCollection (null);
    _nodes.SetParent (null, this);
  }

  public WebTreeNode()
    : this (null, null, null)
  {
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  protected internal void SetParent (WebTreeView treeView, WebTreeNode parentNode)
  {
    _treeView = treeView; 
    _parentNode = parentNode; 
    _nodes.SetParent (_treeView, parentNode);
  }

  [DefaultValue ("")]
  public virtual string NodeID
  {
    get { return _nodeID; }
    set { _nodeID = value; }
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
  [Description ("The tree nodes displayed by this tree view.")]
  [DefaultValue ((string) null)]
  public WebTreeNodeCollection Nodes
  {
    get { return _nodes; }
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
        if (_nodes != null)
          _nodes.OwnerControl = value;
        OnOwnerControlChanged();
      }
    }
  }
}

}

//using System;
//using System.Collections;
//using System.ComponentModel;
//using System.Drawing.Design;
//using System.Web.UI;
//using Rubicon.Web.UI.Design;
//using Rubicon.Utilities;
//using Rubicon.Collections;

namespace Rubicon.Web.UI.Controls
{
using System.Drawing.Design;
using Rubicon.Web.UI.Design;

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
    : this (ownerControl, new Type[] {typeof (WebMenuItem)})
  {
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new WebTreeNode this[int index]
  {
    get { return (WebTreeNode) List[index]; }
    set { List[index] = value; }
  }

  protected override void OnInsertComplete(int index, object value)
  {
    base.OnInsertComplete (index, value);
    ((WebTreeNode) value).SetParent (_treeView, _parentNode);
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
}

}

//using System;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UI.Design
{
using Rubicon.Web.UI.Controls;
public class WebTreeNodeCollectionEditor: AdvancedCollectionEditor
{
  public WebTreeNodeCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (WebTreeNode)};
  }
}

}