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
using Rubicon.Web;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> Object bound tree view. </summary>
/// <remarks>
///   <para>
///     Override <see cref="GetPropertyNodes"/> to set the properties for which to display business object child nodes.
///     Otherwise <see cref="Property"/> is used for every level in the tree.
///   </para><para>
///     Call <see cref="InvalidateTreeNodes"/> to force a refresh of the tree view in case the cached nodes
///     no longer represent he current object model.
///   </para>
/// </remarks>
/// <exception cref="InvalidOperationException">
///   Thrown during <see cref="WebTreeView"/>'s call to the <c>EvaluateWebTreeNode</c> delegate 
///   if <see cref="EnableTreeNodeCaching"/> is <see langword="true"/> and a tree node's 
///   <see cref="IBusinessObjectWithIdentity"/> or <see cref="IBusinessObjectReferenceProperty"/> could not restored
///   from the underlying object model.
/// </exception>
[DefaultEvent ("Click")]
public class BocTreeView: BusinessObjectBoundWebControl
{
  // constants

  // types
  
  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };

  private static readonly object s_clickEvent = new object();
  private static readonly object s_selectionChangedEvent = new object();

  // member fields
  private WebTreeView _treeView;
  
  /// <summary> The <see cref="IBusinessObject"/> displayed by the <see cref="BocTreeView"/>. </summary>
  private IList _value = null;
  private bool _enableTreeNodeCaching = true;
  private Pair[] _nodesViewState;
  private bool _isRebuildRequired = false;

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
    _treeView.SelectionChanged += new WebTreeNodeEventHandler(TreeView_SelectionChanged);
    _treeView.SetEvaluateTreeNodeDelegate (new EvaluateWebTreeNode (EvaluateTreeNode));
    _treeView.SetInitializeRootTreeNodesDelegate (new InitializeRootWebTreeNodes (InitializeRootWebTreeNodes));
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
    BocTreeNodeClickEventHandler handler = (BocTreeNodeClickEventHandler) Events[s_clickEvent];
    if (handler != null)
    {
      ArgumentUtility.CheckNotNullAndType ("node", node, typeof (BocTreeNode));
      BusinessObjectTreeNode businessObjectNode = node as BusinessObjectTreeNode;
      BusinessObjectPropertyTreeNode propertyNode = node as BusinessObjectPropertyTreeNode;
    
      BocTreeNodeClickEventArgs e = null;
      if (businessObjectNode != null)
        e = new BocTreeNodeClickEventArgs (businessObjectNode, path);
      else if (propertyNode != null)
        e = new BocTreeNodeClickEventArgs (propertyNode, path);
      
      handler (this, e);
    }
  }

  /// <summary> Handles the tree view's <see cref="WebTreeView.SelectionChanged"/> event. </summary>
  private void TreeView_SelectionChanged (object sender, WebTreeNodeEventArgs e)
  {
    OnSelectionChanged (e.Node);
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  protected virtual void OnSelectionChanged (WebTreeNode node)
  {
    BocTreeNodeEventHandler handler = (BocTreeNodeEventHandler) Events[s_selectionChangedEvent];
    if (handler != null)
    {
      ArgumentUtility.CheckNotNullAndType ("node", node, typeof (BocTreeNode));
      BusinessObjectTreeNode businessObjectNode = node as BusinessObjectTreeNode;
      BusinessObjectPropertyTreeNode propertyNode = node as BusinessObjectPropertyTreeNode;
    
      BocTreeNodeEventArgs e = null;
      if (businessObjectNode != null)
        e = new BocTreeNodeEventArgs (businessObjectNode);
      else if (propertyNode != null)
        e = new BocTreeNodeEventArgs (propertyNode);
      
      handler (this, e);
    }
  }

  /// <summary>
  ///   Sets the tree view to be rebuilded with the current business objects. 
  ///   Must be called before or during the <c>PostBackEvent</c> to affect the tree view.
  /// </summary>
  public void InvalidateTreeNodes()
  {
    _isRebuildRequired = true;
  }

  public void RefreshTreeNodes()
  {
    BocTreeNode selectedNode = (BocTreeNode) _treeView.SelectedNode;
    string selectedNodePath = selectedNodePath = _treeView.FormatNodePath (selectedNode);

    InvalidateTreeNodes();
    InitializeRootWebTreeNodes();

    if (! StringUtility.IsNullOrEmpty (selectedNodePath))
    {
      string[] pathSegments;
      selectedNode = (BocTreeNode) _treeView.ParseNodePath (selectedNodePath, out pathSegments);
      if (selectedNode != null)
        selectedNode.IsSelected = true;
    }

    if (selectedNodePath != _treeView.FormatNodePath (selectedNode))
      OnSelectionChanged (selectedNode);
  }

  /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
  }

  /// <summary> Overrides the parent control's <c>PreRenderChildControls</c> method. </summary>
  protected override void PreRenderChildControls()
  {
    _treeView.Width = Width;
    _treeView.Height = Height;
  }

  private void InitializeRootWebTreeNodes()
  {
    if (! _enableTreeNodeCaching || ! Page.IsPostBack)
    {
      CreateRootTreeNodes();
    }
    else
    {
      if (_isRebuildRequired)
        RebuildTreeNodes();
      else
        LoadNodesViewStateRecursive (_nodesViewState, _treeView.Nodes);
    }
  }

  private void CreateRootTreeNodes()
  {
    if (Value != null)
    {
      foreach (IBusinessObjectWithIdentity businessObject in Value)
      {
        BusinessObjectTreeNode node = CreateBusinessObjectNode (null, businessObject);
        _treeView.Nodes.Add (node);
        if (EnableTopLevelExpander)
          node.IsEvaluated = false;
        else
          node.EvaluateExpand();
      }
    }
  }

  private void RebuildTreeNodes()
  {
    if (! _enableTreeNodeCaching)
      return;

    _treeView.Nodes.Clear();
    CreateRootTreeNodes();
    ApplyNodesViewStateRecursive (_nodesViewState, _treeView.Nodes);
  }

  private void ApplyNodesViewStateRecursive (Pair[] nodesViewState, WebTreeNodeCollection nodes)
  {
    foreach (Pair nodeViewState in nodesViewState)
    {
      object[] values = (object[]) nodeViewState.First;
      string nodeID = (string) values[0];
      WebTreeNode node = nodes.Find (nodeID);
      if (node != null)
      {
        if (! node.IsEvaluated)
        {
          bool isEvaluated = (bool) values[2];
          bool isExpanded = (bool) values[1];
          if (isEvaluated)
            node.EvaluateExpand();
          node.IsExpanded = isExpanded;
        }
        if (node.Children.Count == 0)
          node.IsExpanded = false;
        ApplyNodesViewStateRecursive ((Pair[]) nodeViewState.Second, node.Children);
      }
    }
  }

  private void EvaluateTreeNode (WebTreeNode node)
  {
    ArgumentUtility.CheckNotNullAndType ("node", node, typeof (BocTreeNode));

    if (node.IsEvaluated)
      return;

    BusinessObjectTreeNode businessObjectNode = node as BusinessObjectTreeNode;
    BusinessObjectPropertyTreeNode propertyNode = node as BusinessObjectPropertyTreeNode;
  
    if (businessObjectNode != null)
      CreateAndAppendBusinessObjectNodeChildren (businessObjectNode);
    else if (propertyNode != null)
      CreateAndAppendPropertyNodeChildren (propertyNode);
  }

  private void CreateAndAppendBusinessObjectNodeChildren (BusinessObjectTreeNode businessObjectNode)
  {
    IBusinessObjectWithIdentity businessObject = businessObjectNode.BusinessObject;
    BusinessObjectPropertyTreeNodeInfo[] propertyNodeInfos = GetPropertyNodes (businessObject);
    if (propertyNodeInfos != null && propertyNodeInfos.Length > 0)
    {
      if (propertyNodeInfos.Length == 1)
        CreateAndAppendBusinessObjectNodes (businessObjectNode.Children, businessObject, propertyNodeInfos[0].Property);
      else
        CreateAndAppendPropertyNodes (businessObjectNode.Children, businessObject, propertyNodeInfos);
    }
    businessObjectNode.IsEvaluated = true;
  }

  private void CreateAndAppendPropertyNodeChildren (BusinessObjectPropertyTreeNode propertyNode)
  {
    if (propertyNode.ParentNode == null)
      throw new ArgumentException ("BusinessObjectPropertyTreeNode with NodeID '" + propertyNode.NodeID + "' has no parent node but property nodes cannot be used as root nodes.");

    BusinessObjectTreeNode parentNode = (BusinessObjectTreeNode) propertyNode.ParentNode;
    CreateAndAppendBusinessObjectNodes (propertyNode.Children, parentNode.BusinessObject, propertyNode.Property);
    propertyNode.IsEvaluated = true;
  }

  private void CreateAndAppendBusinessObjectNodes (
      WebTreeNodeCollection businessObjectNodes, 
      IBusinessObjectWithIdentity parentBusinessObject,
      IBusinessObjectReferenceProperty property)
  {
    IList children = GetBusinessObjects (parentBusinessObject, property);
    foreach (IBusinessObjectWithIdentity childBusinessObject in children)
    {
      BusinessObjectTreeNode childNode = CreateBusinessObjectNode (property, childBusinessObject);
      businessObjectNodes.Add (childNode);
    }
  }
  
  private void CreateAndAppendPropertyNodes (
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

  private BusinessObjectTreeNode CreateBusinessObjectNode (
    IBusinessObjectReferenceProperty property,
    IBusinessObjectWithIdentity businessObject)
  {
    string id = businessObject.UniqueIdentifier;
    string text = businessObject.DisplayName;
    IconInfo icon = BusinessObjectBoundWebControl.GetIcon (
        businessObject, 
        businessObject.BusinessObjectClass.BusinessObjectProvider);
    BusinessObjectTreeNode node = new BusinessObjectTreeNode (id, text, icon, property, businessObject);
    node.IsEvaluated = false;
    return node;
  }

  protected virtual IBusinessObjectWithIdentity[] GetBusinessObjects (
      IBusinessObjectWithIdentity parent,
      IBusinessObjectReferenceProperty property)
  {
    IList children = (IList) parent.GetProperty (property);
    ArrayList childrenList = new ArrayList (children);
    return  (IBusinessObjectWithIdentity[]) childrenList.ToArray (typeof (IBusinessObjectWithIdentity));
  }

  protected virtual BusinessObjectPropertyTreeNodeInfo[] GetPropertyNodes (IBusinessObjectWithIdentity businessObject)
  {
    if (Property == null)
    {
      ArrayList referenceListPropertyInfos = new ArrayList();
      foreach (IBusinessObjectProperty property in businessObject.BusinessObjectClass.GetPropertyDefinitions ())
      {
        IBusinessObjectReferenceProperty referenceProperty = property as IBusinessObjectReferenceProperty;
        if (   referenceProperty != null
            && referenceProperty.IsList 
            && referenceProperty.ReferenceClass is IBusinessObjectClassWithIdentity
            && referenceProperty.IsAccessible (businessObject.BusinessObjectClass, businessObject))
        {
          referenceListPropertyInfos.Add (new BusinessObjectPropertyTreeNodeInfo (referenceProperty));
        }
      }
      return (BusinessObjectPropertyTreeNodeInfo[]) referenceListPropertyInfos.ToArray (typeof (BusinessObjectPropertyTreeNodeInfo));
    }
    
    return new BusinessObjectPropertyTreeNodeInfo[] { new BusinessObjectPropertyTreeNodeInfo (Property) };
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
      _nodesViewState = (Pair[]) values[1];
  }

  /// <summary> Calls the parent's <c>SaveViewState</c> method and saves this control's specific data. </summary>
  /// <returns> Returns the server control's current view state. </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[2];

    values[0] = base.SaveViewState();
    if (_enableTreeNodeCaching)
      values[1] = SaveNodesViewStateRecursive (_treeView.Nodes);

    return values;
  }

  /// <summary> Loads the settings of the <paramref name="nodes"/> from <paramref name="viewState"/>. </summary>
  private void LoadNodesViewStateRecursive (Pair[] nodesViewState, WebTreeNodeCollection nodes)
  {
    foreach (Pair nodeViewState in nodesViewState)
    {
      object[] values = (object[]) nodeViewState.First;
      string nodeID = (string) values[0];
      bool isExpanded = (bool) values[1];
      bool isEvaluated = (bool) values[2];
      bool isSelected = (bool) values[3];
      string text = (string) values[4];
      IconInfo icon = (IconInfo) values[5];
      bool isBusinessObjectTreeNode = (bool) values[7];
      
      WebTreeNode node;
      if (isBusinessObjectTreeNode)
      {
        node = new BusinessObjectTreeNode (nodeID, text, icon, null, null);
        string propertyIdentifier = (string) values[6];
        ((BusinessObjectTreeNode) node).PropertyIdentifier = propertyIdentifier;
      }
      else
      {
        node = new BusinessObjectPropertyTreeNode (nodeID, text, icon, null);
      }
      node.IsExpanded = isExpanded;
      node.IsEvaluated = isEvaluated;
      if (isSelected)
        node.IsSelected = true;
      nodes.Add (node);

      LoadNodesViewStateRecursive ((Pair[]) nodeViewState.Second, node.Children);
    }
  }

  /// <summary> Saves the settings of the  <paramref name="nodes"/> and returns this view state </summary>
  private Pair[] SaveNodesViewStateRecursive (WebTreeNodeCollection nodes)
  {
    Pair[] nodesViewState = new Pair[nodes.Count];
    for (int i = 0; i < nodes.Count; i++)
    {
      WebTreeNode node = (WebTreeNode) nodes[i];    
      Pair nodeViewState = new Pair();
      object[] values = new object[8];
      values[0] = node.NodeID;
      values[1] = node.IsExpanded;
      values[2] = node.IsEvaluated;
      values[3] = node.IsSelected;
      values[4] = node.Text;
      values[5] = node.Icon;
      if (node is BusinessObjectTreeNode)
      { 
        values[6] = ((BusinessObjectTreeNode) node).PropertyIdentifier;
        values[7] = true;
      }
      else
      {
        values[7] = false;
      }
      nodeViewState.First = values;
      nodeViewState.Second = SaveNodesViewStateRecursive (node.Children);
      nodesViewState[i] = nodeViewState;
    }
    return nodesViewState;
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
  /// <value> Returns always <see langword="true"/>. </value>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using its ClientID.
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
  /// <value> The value must be of type <see cref="IList"/> or <see cref="IBusinessObjectWithIdentity"/>. </value>
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

  /// <summary> Gets or sets a flag that determines whether to enable word wrapping. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("If set, word wrap will be enabled for the tree node's text.")]
  [DefaultValue (false)]
  public bool EnableWordWrap
  {
    get { return _treeView.EnableWordWrap; }
    set { _treeView.EnableWordWrap = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to show the connection lines between the nodes. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("If cleared, the tree nodes will not be connected by lines.")]
  [DefaultValue (true)]
  public bool ShowLines
  {
    get { return _treeView.ShowLines; }
    set { _treeView.ShowLines = value; }
  }

  /// <summary> Gets or sets a flag that determines whether the evaluated tree nodes will be cached. </summary>
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
  public event BocTreeNodeClickEventHandler Click
  {
    add { Events.AddHandler (s_clickEvent, value); }
    remove { Events.RemoveHandler (s_clickEvent, value); }
  }

  /// <summary> 
  ///   Occurs when the selected node is changed. Fires for both client side changes or change by the 
  ///   <see cref="RefreshTreeNodes"/> method.
  /// </summary>
  [Category ("Action")]
  [Description ("Occurs when the selected node is changed. Fires for both client side changes and a change by the RefreshTreeNodes method.")]
  public event BocTreeNodeEventHandler SelectionChanged
  {
    add { Events.AddHandler (s_selectionChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
  }

  /// <summary> Gets the currently selected tree node. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocTreeNode SelectedNode
  {
    get { return (BocTreeNode) _treeView.SelectedNode; }
  }

//  public void EnsureTreeNodesCreated()
//  {
//    _treeView.EnsureTreeNodesCreated();
//  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocTreeView"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocTreeView</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocTreeView"; } }
  #endregion
}

public class BusinessObjectPropertyTreeNodeInfo
{
  private string _text;
  private IconInfo _icon;
  private IBusinessObjectReferenceProperty _property;

  public BusinessObjectPropertyTreeNodeInfo (IBusinessObjectReferenceProperty property)
  {
    _text = property.DisplayName;
    _icon = null;
    _property = property;
  }

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

/// <summary> Represents the method that handles the <c>Click</c> event raised when clicking on a tree node. </summary>
public delegate void BocTreeNodeClickEventHandler (object sender, BocTreeNodeClickEventArgs e);

/// <summary> Provides data for the <c>Click</c> event. </summary>
public class BocTreeNodeClickEventArgs: BocTreeNodeEventArgs
{
  private string[] _path;

  /// <summary> Initializes a new instance. </summary>
  public BocTreeNodeClickEventArgs (BusinessObjectTreeNode node, string[] path)
    : base (node)
  {
    _path = path;
  }

  /// <summary> Initializes a new instance. </summary>
  public BocTreeNodeClickEventArgs (BusinessObjectPropertyTreeNode node, string[] path)
    : base (node)
  {
  }

  /// <summary> The ID path for the clicked node. </summary>
  public string[] Path
  {
    get { return _path; }
  }
}

/// <summary> Represents the method that handles events raised by a <see cref="BocTreeNode"/>. </summary>
public delegate void BocTreeNodeEventHandler (object sender, BocTreeNodeEventArgs e);

/// <summary> Provides data for the events raised by a <see cref="BocTreeNode"/>. </summary>
public class BocTreeNodeEventArgs: WebTreeNodeEventArgs
{
  /// <summary> Initializes a new instance. </summary>
  public BocTreeNodeEventArgs (BusinessObjectTreeNode node)
    : base (node)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public BocTreeNodeEventArgs (BusinessObjectPropertyTreeNode node)
    : base (node)
  {
  }

  public new BocTreeNode Node
  {
    get { return (BocTreeNode) base.Node; }
  }

  public BusinessObjectTreeNode BusinessObjectTreeNode
  {
    get { return Node as BusinessObjectTreeNode; }
  }

  public BusinessObjectPropertyTreeNode PropertyTreeNode
  {
    get { return Node as BusinessObjectPropertyTreeNode; }
  }
}

}
