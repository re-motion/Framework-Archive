using System;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing.Design;
using Rubicon.NullableValueTypes;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Globalization;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BocTreeView: BusinessObjectBoundWebControl
{
  // constants

  // types
  
  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };

  private static readonly object s_clickEvent = new object();

  // member fields
  private WebTreeView _treeView;
  
  /// <summary> The <see cref="IBusinessObject"/> displayed by the <see cref="BocTreeView"/>. </summary>
  private IList _value = null;
  private bool _enableTreeNodeCaching = true;
  private Pair[] _nodeViewStates;

  // construction and destruction
  public BocTreeView()
  {
    _treeView = new WebTreeView (this);
  }

  // methods and properties
  /// <summary> Overrides the parent control's <c>CreateChildControls</c> method. </summary>
  protected override void CreateChildControls()
  {
    _treeView.ID = ID + "_Boc_TreeView";
    _treeView.Click += new WebTreeNodeClickEventHandler(TreeView_Click);
    _treeView.SetEvaluateTreeNodeDelegate (new EvaluateWebTreeNode (EvaluateTreeNode));
    _treeView.SetCreateRootTreeNodesDelegate (new CreateRootWebTreeNodes (CreateRootTreeNodes));
    _treeView.EnableTreeNodeViewState = ! _enableTreeNodeCaching;
    Controls.Add (_treeView);
  }

  /// <summary> Handles the tree view's <see cref="WebTreeView.Click"/> event. </summary>
  private void TreeView_Click(object sender, WebTreeNodeClickEventArgs e)
  {
    OnClick (e.Node, e.Path);
  }

  /// <summary> Fires the <see cref="Click"/> event. </summary>
  protected virtual void OnClick (WebTreeNode node, string[] path)
  {
    WebTreeNodeClickEventHandler handler = (WebTreeNodeClickEventHandler) Events[s_clickEvent];
    if (handler != null)
    {
      WebTreeNodeClickEventArgs e = new WebTreeNodeClickEventArgs (node, path);
      handler (this, e);
    }
  }

  /// <summary>
  ///   Calls the parent's <c>OnPreRender</c> method and ensures that the sub-controls are properly initialized.
  /// </summary>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    //  First call
    EnsureChildControlsPreRendered();
  }

  /// <summary> Calls the parent's <c>Render</c> method and ensures that the sub-controls are properly initialized. </summary>
  protected override void Render (HtmlTextWriter writer)
  {
    //  Second call has practically no overhead
    //  Required to get optimum designer support.
    EnsureChildControlsPreRendered();

    base.Render (writer);
  }

  /// <summary> Overrides the parent control's <c>PreRenderChildControls</c> method. </summary>
  protected override void PreRenderChildControls()
  {
    _treeView.Width = Width;
    _treeView.Height = Height;
  }

  private void CreateRootTreeNodes()
  {
    if (! _enableTreeNodeCaching || ! Page.IsPostBack)
    {
      if (Value != null)
      {
        foreach (IBusinessObjectWithIdentity businessObject in Value)
          CreateRootNode (Nodes, businessObject, ! EnableTopLevelExpander);
      }
    }
    else
    {
      LoadNodeViewStateRecursive (_nodeViewStates, _treeView.Nodes);
    }
  }

  private void EvaluateTreeNode (WebTreeNode node)
  {
    ArgumentUtility.CheckNotNull ("node", node);

    BusinessObjectTreeNode businessObjectNode = node as BusinessObjectTreeNode;
    BusinessObjectPropertyTreeNode propertyNode = node as BusinessObjectPropertyTreeNode;
  
    if (businessObjectNode != null)
      CreateAppendBusinessObjectNodeChildren (businessObjectNode);
    else if (propertyNode != null)
      CreateAppendPropertyNodeChildren (propertyNode);
    else
      node.IsEvaluated = false;
  }

  private void CreateRootNode (
      WebTreeNodeCollection rootNodes, 
      IBusinessObjectWithIdentity rootBusinessObject,
      bool expandNode)
  {
    BusinessObjectTreeNode rootNode = CreateBusinessObjectNode (rootBusinessObject);
    rootNodes.Add (rootNode);
    if (expandNode)
      rootNode.EvaluateExpand();
    else
      rootNode.IsEvaluated = false;
  }

  private void CreateAppendBusinessObjectNodeChildren (BusinessObjectTreeNode businessObjectNode)
  {
    IBusinessObjectWithIdentity businessObject = businessObjectNode.BusinessObject;
    BusinessObjectPropertyTreeNodeInfo[] propertyNodeInfos = GetPropertyNodes (businessObject);
    if (propertyNodeInfos == null)
    {
      businessObjectNode.IsEvaluated = false;
      return;
    }

    if (propertyNodeInfos.Length == 1)
      CreateAppendBusinessObjectNodes (businessObjectNode.Children, businessObject, propertyNodeInfos[0].Property);
    else
      CreateAppendPropertyNodes (businessObjectNode.Children, businessObject, propertyNodeInfos);

    businessObjectNode.IsEvaluated = true;
  }

  private void CreateAppendPropertyNodeChildren (BusinessObjectPropertyTreeNode propertyNode)
  {
    IBusinessObjectWithIdentity parentBusinessObject = null;
    if (propertyNode.ParentNode != null)
    {
      parentBusinessObject = ((BusinessObjectTreeNode) propertyNode.ParentNode).BusinessObject;
      if (parentBusinessObject == null)
      {
      }
    }
    else if (DataSource.BusinessObject != null)
      parentBusinessObject = (IBusinessObjectWithIdentity) DataSource.BusinessObject;

    if (parentBusinessObject != null)
      CreateAppendBusinessObjectNodes (propertyNode.Children, parentBusinessObject, propertyNode.Property);

    propertyNode.IsEvaluated = true;
  }

  private void CreateAppendBusinessObjectNodes (
      WebTreeNodeCollection businessObjectNodes, 
      IBusinessObjectWithIdentity parentBusinessObject,
      IBusinessObjectReferenceProperty property)
  {
    IList children = (IList) parentBusinessObject.GetProperty (property);
    foreach (IBusinessObjectWithIdentity childBusinessObject in children)
    {
      BusinessObjectTreeNode childNode = CreateBusinessObjectNode (childBusinessObject);
      businessObjectNodes.Add (childNode);
    }
  }
  
  private void CreateAppendPropertyNodes (
      WebTreeNodeCollection propertyNodes, 
      IBusinessObjectWithIdentity parentBusinessObject,
      BusinessObjectPropertyTreeNodeInfo[] propertyNodeInfos)
  {
    foreach (BusinessObjectPropertyTreeNodeInfo propertyNodeInfo in propertyNodeInfos)
    {
      BusinessObjectPropertyTreeNode propertyNode = new BusinessObjectPropertyTreeNode (
          propertyNodeInfo.Property.Identifier, 
          propertyNodeInfo.Text, 
          propertyNodeInfo.Icon, 
          propertyNodeInfo.Property);
      propertyNode.IsEvaluated = false;
      propertyNodes.Add (propertyNode);
    }
  }

  private BusinessObjectTreeNode CreateBusinessObjectNode (IBusinessObjectWithIdentity businessObject)
  {
    string id = businessObject.UniqueIdentifier;
    string text = businessObject.DisplayName;
    IconInfo icon = BusinessObjectBoundWebControl.GetIcon (
        businessObject, 
        businessObject.BusinessObjectClass.BusinessObjectProvider);
    BusinessObjectTreeNode node = new BusinessObjectTreeNode (id, text, icon, businessObject);
    node.IsEvaluated = false;
    return node;
  }

  protected virtual BusinessObjectPropertyTreeNodeInfo[] GetPropertyNodes (IBusinessObjectWithIdentity businessObject)
  {
    if (Property == null)
      return null;
    
    string title = Property.DisplayName;
    IconInfo icon = BusinessObjectBoundWebControl.GetIcon (
        businessObject, 
        Property.ReferenceClass.BusinessObjectProvider);
    return new BusinessObjectPropertyTreeNodeInfo[] { new BusinessObjectPropertyTreeNodeInfo (title, icon, Property) };
  }

  /// <summary>
  ///   Loads the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> or uses the cached
  ///   information if <paramref name="interim"/> is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to load the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void LoadValue (bool interim)
  {
    if (DataSource != null && DataSource.BusinessObject != null)
      ValueImplementation = DataSource.BusinessObject;
  }

  /// <summary> Calls the parent's <c>LoadViewState</c> method and restores this control's specific data. </summary>
  /// <param name="savedState"> An <see cref="Object"/> that represents the control state to be restored. </param>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    
    base.LoadViewState (values[0]);
    if (_enableTreeNodeCaching)
      _nodeViewStates = (Pair[]) values[1];
  }

  /// <summary> Calls the parent's <c>SaveViewState</c> method and saves this control's specific data. </summary>
  /// <returns> Returns the server control's current view state. </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[2];

    values[0] = base.SaveViewState();
    if (_enableTreeNodeCaching)
      values[1] = SaveNodeViewStateRecursive (_treeView.Nodes);

    return values;
  }

  /// <summary> Loads the settings of the <paramref name="nodes"/> from <paramref name="viewState"/>. </summary>
  private void LoadNodeViewStateRecursive (Pair[] nodeViewStates, WebTreeNodeCollection nodes)
  {
    foreach (Pair nodeViewState in nodeViewStates)
    {
      object[] values = (object[]) nodeViewState.First;
      string nodeID = (string) values[0];
      bool isExpanded = (bool) values[1];
      bool isEvaluated = (bool) values[2];
      string text = (string) values[3];
      IconInfo icon = (IconInfo) values[4];
      bool isBusinessObjectTreeNode = (bool) values[5];
      
      WebTreeNode node;
      if (isBusinessObjectTreeNode)
        node = new BusinessObjectTreeNode (nodeID, text, icon, null);
      else
        node = new BusinessObjectPropertyTreeNode (nodeID, text, icon, null);
      node.IsExpanded = isExpanded;
      node.IsEvaluated = isEvaluated;
      nodes.Add (node);

      LoadNodeViewStateRecursive ((Pair[]) nodeViewState.Second, node.Children);
    }
  }

  /// <summary> Saves the settings of the  <paramref name="nodes"/> and returns this view state </summary>
  private Pair[] SaveNodeViewStateRecursive (WebTreeNodeCollection nodes)
  {
    Pair[] nodeViewStates = new Pair[nodes.Count];
    for (int i = 0; i < nodes.Count; i++)
    {
      WebTreeNode node = (WebTreeNode) nodes[i];    
      Pair nodeViewState = new Pair();
      object[] values = new object[6];
      values[0] = node.NodeID;
      values[1] = node.IsExpanded;
      values[2] = node.IsEvaluated;
      values[3] = node.Text;
      values[4] = node.Icon;
      values[5] = node is BusinessObjectTreeNode;
      nodeViewState.First = values;
      nodeViewState.Second = SaveNodeViewStateRecursive (node.Children);
      nodeViewStates[i] = nodeViewState;
    }
    return nodeViewStates;
  }

  /// <summary> Gets a value that indicates whether properties with the specified multiplicity are supported. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is true. </returns>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }


  /// <summary>
  ///   The list of<see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
  ///   implementations that can be bound to this control.
  /// </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using it's ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (Control) this; }
  }

  /// <summary> The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
  /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectReferenceProperty Property
  {
    get { return (IBusinessObjectReferenceProperty) base.Property; }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectReferenceProperty));
      if (value.IsList == false)
        throw new ArgumentException ("Only properties supporting IList can be assigned to the BocTreeView.", "value");
      base.Property = (IBusinessObjectReferenceProperty) value; 
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> An object implementing <see cref="IBusinessObject"/>. </value>
  [Browsable (false)]
  public new IList Value
  {
    get { return _value; }
    set 
    {
      if (value != null)
        ArgumentUtility.CheckItemsNotNullAndType ("value", value, typeof (IBusinessObjectWithIdentity));
      _value = value; 
    }
  }

  /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set 
    {
      if (value is IList)
        Value = (IList) value; 
      else
        Value = new IBusinessObjectWithIdentity[] { (IBusinessObjectWithIdentity) value };
    }
  }


  /// <summary> Gets the tree nodes displayed by this tree view. </summary>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public WebTreeNodeCollection Nodes
  {
    get { return _treeView.Nodes; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether to show the top level expander and automatically expand the 
  ///   child nodes if the expander is hidden.
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("If cleared, the top level expender will be hidden and the child nodes expanded for the top level nodes.")]
  [DefaultValue (true)]
  public bool EnableTopLevelExpander
  {
    get { return _treeView.EnableTopLevelExpander; }
    set { _treeView.EnableTopLevelExpander = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether to show scroll bars. Requires also a width for the tree view.
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("If set, the tree view shows srcoll bars. Requires a witdh in addition to this setting to actually enable the scrollbars.")]
  [DefaultValue (false)]
  public bool EnableScrollBars
  {
    get { return _treeView.EnableScrollBars; }
    set { _treeView.EnableScrollBars = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether to enable word wrapping.
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("If set, word wrap will be enabled for the tree node's text.")]
  [DefaultValue (false)]
  public bool EnableWordWrap
  {
    get { return _treeView.EnableWordWrap; }
    set { _treeView.EnableWordWrap = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether to show the connection lines between the nodes.
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("If cleared, the tree nodes will not be connected by lines.")]
  [DefaultValue (true)]
  public bool ShowLines
  {
    get { return _treeView.ShowLines; }
    set { _treeView.ShowLines = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether the evaluated tree nodes will be cached.
  /// </summary>
  /// <remarks> 
  ///   Clear this flag if you want to reload all evaluated tree nodes during each post back. 
  ///   This could be required if the tree must show only the current nodes instead of the nodes that have 
  ///   been in the tree during the first evaluation.
  /// </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("If cleared, the evaluated tree nodes will be reloaded during each postback.")]
  [DefaultValue (true)]
  public bool EnableTreeNodeCaching
  {
    get { return _enableTreeNodeCaching; }
    set
    { 
      _enableTreeNodeCaching = value; 
      _treeView.EnableTreeNodeViewState = ! value;
    }
  }

  /// <summary> Occurs when a node is clicked. </summary>
  [Category ("Action")]
  [Description ("Occurs when a node is clicked.")]
  public event WebTreeNodeClickEventHandler Click
  {
    add { Events.AddHandler (s_clickEvent, value); }
    remove { Events.RemoveHandler (s_clickEvent, value); }
  }
}

public class BusinessObjectPropertyTreeNodeInfo
{
  private string _text;
  private IconInfo _icon;
  private IBusinessObjectReferenceProperty _property;

  public BusinessObjectPropertyTreeNodeInfo (string text, IconInfo icon, IBusinessObjectReferenceProperty property)
  {
    _text = text;
    _icon = icon;
    _property = property;
  }

  public string Text
  {
    get { return _text; }
    set { _text = value; }
  }

  public IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  public IBusinessObjectReferenceProperty Property
  {
    get { return _property; }
    set { _property = value; }
  }

}

}
