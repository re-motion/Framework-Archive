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
    _treeView.EvaluateTreeNode += new EvaluateWebTreeNode (EvaluateTreeNode);
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
    foreach (IBusinessObjectWithIdentity businessObject in Value)
      CreateRootNode (Nodes, businessObject, ! EnableTopLevelExpander);
  }

  private void EvaluateTreeNode (WebTreeNode node)
  {
    ArgumentUtility.CheckNotNullAndType ("node", node, typeof (BocTreeNode));

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
    {
      //CreateAppendBusinessObjectNodeChildren (rootNode);
      rootNode.IsExpanded = true;
    }
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
    IBusinessObject parentBusinessObject = null;
    if (propertyNode.ParentNode != null)
      parentBusinessObject = ((BusinessObjectTreeNode) propertyNode.ParentNode).BusinessObject;
    else if (DataSource.BusinessObject != null)
      parentBusinessObject = DataSource.BusinessObject;

    if (parentBusinessObject != null)
      CreateAppendBusinessObjectNodes (propertyNode.Children, parentBusinessObject, propertyNode.Property);

    propertyNode.IsEvaluated = true;
  }

  private void CreateAppendBusinessObjectNodes (
      WebTreeNodeCollection businessObjectNodes, 
      IBusinessObject parentBusinessObject,
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
      IBusinessObject parentBusinessObject,
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

  protected virtual BusinessObjectPropertyTreeNodeInfo[] GetPropertyNodes (IBusinessObject businessObject)
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
    if (Property != null && DataSource != null && DataSource.BusinessObject != null)
    {
      ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
    }
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
    set { _value = value; }
  }

  /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IList) value; }
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

//  [Browsable (false)]
//  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
//  public EvaluateWebTreeNode EvaluateTreeNode
//  {
//    get { return _treeView.EvaluateTreeNode; }
//    set { _treeView.EvaluateTreeNode = value; }
//  }

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
