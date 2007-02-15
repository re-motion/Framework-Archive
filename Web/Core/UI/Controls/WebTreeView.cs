using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A tree view. </summary>
/// <include file='doc\include\UI\Controls\WebTreeView.xml' path='WebTreeView/Class/*' />
[ToolboxData("<{0}:WebTreeView runat=server></{0}:WebTreeView>")]
[DefaultEvent ("Click")]
public class WebTreeView: WebControl, IControl, IPostBackEventHandler, IResourceDispatchTarget
{
  // constants
  #region private const string c_nodeIcon...
  private const string c_nodeIconF = "TreeViewF.gif";
  private const string c_nodeIconFMinus = "TreeViewFMinus.gif";
  private const string c_nodeIconFPlus = "TreeViewFPlus.gif";
  private const string c_nodeIconI = "TreeViewI.gif";
  private const string c_nodeIconL = "TreeViewL.gif";
  private const string c_nodeIconLMinus = "TreeViewLMinus.gif";
  private const string c_nodeIconLPlus = "TreeViewLPlus.gif";
  private const string c_nodeIconMinus = "TreeViewMinus.gif";
  private const string c_nodeIconPlus = "TreeViewPlus.gif";
  private const string c_nodeIconR = "TreeViewR.gif";
  private const string c_nodeIconRMinus = "TreeViewRMinus.gif";
  private const string c_nodeIconRPlus = "TreeViewRPlus.gif";
  private const string c_nodeIconT = "TreeViewT.gif";
  private const string c_nodeIconTMinus = "TreeViewTMinus.gif";
  private const string c_nodeIconTPlus = "TreeViewTPlus.gif";
  private const string c_nodeIconWhite = "TreeViewWhite.gif";
  #endregion

  /// <summary> The separator used for the node path. </summary>
  private const char c_pathSeparator = '\t';
  /// <summary> The prefix for the expansion command. </summary>
  private const string c_expansionCommandPrefix = "Expand=";
  /// <summary> The prefix for the click command. </summary>
  private const string c_clickCommandPrefix = "Click=";

  // statics
  private static readonly object s_clickEvent = new object();
  private static readonly object s_selectionChangedEvent = new object();

  // types

  // fields
  // The URL resolved icon paths.
  #region private IconInfo _resolvedNodeIcon...
  private IconInfo _resolvedNodeIconF;
  private IconInfo _resolvedNodeIconFMinus;
  private IconInfo _resolvedNodeIconFPlus;
  private IconInfo _resolvedNodeIconI;
  private IconInfo _resolvedNodeIconL;
  private IconInfo _resolvedNodeIconLMinus;
  private IconInfo _resolvedNodeIconLPlus;
  private IconInfo _resolvedNodeIconMinus;
  private IconInfo _resolvedNodeIconPlus;
  private IconInfo _resolvedNodeIconR;
  private IconInfo _resolvedNodeIconRMinus;
  private IconInfo _resolvedNodeIconRPlus;
  private IconInfo _resolvedNodeIconT;
  private IconInfo _resolvedNodeIconTMinus;
  private IconInfo _resolvedNodeIconTPlus;
  private IconInfo _resolvedNodeIconWhite;
  #endregion

  /// <summary> The nodes in this tree view. </summary>
  private WebTreeNodeCollection _nodes;
  private Triplet[] _nodesViewState;
  private bool _isLoadViewStateCompleted = false;
  private bool _enableTopLevelExpander = true;
  private bool _enableLookAheadEvaluation = false;

  private bool _enableScrollBars = false;
  private bool _enableWordWrap = false;
  private bool _showLines = true;
  private bool _enableTreeNodeViewState = true;
  private bool _hasTreeNodesCreated = false;
  private WebTreeNode _selectedNode = null;

  /// <summary>
  ///   The delegate called before a node with <see cref="WebTreeNode.IsEvaluated"/> set to <see langword="false"/>
  ///   is expanded.
  /// </summary>
  private EvaluateWebTreeNode _evaluateTreeNode;
  private InitializeRootWebTreeNodes _initializeRootTreeNodes;

  //  construction and destruction

  /// <summary> Initalizes a new instance. </summary>
  public WebTreeView (Control ownerControl)
  {
    _nodes = new WebTreeNodeCollection (ownerControl);
    _nodes.SetParent (this, null);
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTreeView()
    :this (null)
  {
  }

  //  methods and properties

//  public void RaisePostDataChangedEvent()
//  {
//  }
//
//  public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
//  {
//    return false;
//  }

  /// <summary> Implementation of the <see cref="IPostBackEventHandler"/> interface. </summary>
  /// <param name="eventArgument"> &lt;command prefix&gt;&lt;node path&gt;</param>
  void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);
    EnsureTreeNodesCreated();

    eventArgument = eventArgument.Trim();
    if (eventArgument.StartsWith (c_expansionCommandPrefix))
      HandleExpansionCommandEvent (eventArgument.Substring (c_expansionCommandPrefix.Length));
    else if (eventArgument.StartsWith (c_clickCommandPrefix))
      HandleClickCommandEvent (eventArgument.Substring (c_clickCommandPrefix.Length));
    else
      throw new ArgumentException ("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
  }

  /// <summary> Handles the expansion command (i.e. expands/collapses the clicked tree node). </summary>
  /// <param name="eventArgument"> The path to the clicked tree node. </param>
  private void HandleExpansionCommandEvent (string eventArgument)
  {
    string[] pathSegments;
    WebTreeNode clickedNode = ParseNodePath (eventArgument, out pathSegments);
    if (clickedNode != null)
    {
      if (clickedNode.IsEvaluated)
      {
        clickedNode.IsExpanded = ! clickedNode.IsExpanded;
        if (clickedNode.IsExpanded && EnableLookAheadEvaluation)
          clickedNode.EvaluateChildren();
      }
      else
      {
        clickedNode.EvaluateExpand();
        if (EnableLookAheadEvaluation)
          clickedNode.EvaluateChildren();
      }
    }
  }

  /// <summary> Handles the click command. </summary>
  /// <param name="eventArgument"> The path to the clicked tree node. </param>
  private void HandleClickCommandEvent (string eventArgument)
  {
    string[] pathSegments;
    WebTreeNode clickedNode = ParseNodePath (eventArgument, out pathSegments);
    bool isSelectionChanged = _selectedNode != clickedNode;
    SetSelectedNode (clickedNode);
    OnClick (clickedNode, pathSegments);
    if (isSelectionChanged)
      OnSelectionChanged (clickedNode);
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

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  protected virtual void OnSelectionChanged (WebTreeNode node)
  {
    WebTreeNodeEventHandler handler = (WebTreeNodeEventHandler) Events[s_selectionChangedEvent];
    if (handler != null)
    {
      WebTreeNodeEventArgs e = new WebTreeNodeEventArgs (node);
      handler (this, e);
    }
  }

  public void SetEvaluateTreeNodeDelegate (EvaluateWebTreeNode evaluateTreeNode)
  {
    _evaluateTreeNode = evaluateTreeNode;
  }

  public void SetInitializeRootTreeNodesDelegate (InitializeRootWebTreeNodes initializeRootTreeNodes)
  {
    _initializeRootTreeNodes = initializeRootTreeNodes;
  }

  //  /// <summary> Collapses all nodes of this tree view. Only the root nodes will remain visible. </summary>
  //  public void CollapseAll()
  //  {
  //    _nodes.SetExpansion (false);
  //  }
  //    
  //  /// <summary> Expands all nodes of this tree view.</summary>
  //  public void ExpandAll()
  //  {
  //    _nodes.SetExpansion (true);
  //  }

  protected void EnsureTreeNodesCreated()
  {
    if (_hasTreeNodesCreated)
      return;

    _hasTreeNodesCreated = true;

    if (_initializeRootTreeNodes != null) 
      _initializeRootTreeNodes();

    if (_nodesViewState != null)
      LoadNodesViewStateRecursive (_nodesViewState, _nodes);

    if (_nodes.Count == 0)
      _hasTreeNodesCreated = false;
  }

  /// <summary>
  ///   Calles the delegate set using <see cref="SetEvaluateTreeNodeDelegate"/> with the passed <paramref name="node"/>.
  /// </summary>
  /// <exception cref="NullReferenceException">
  ///   Thrown if no method is registered for this delegate but a node with 
  ///   <see cref="WebTreeNode.IsEvaluated"/> set to <see langword="false"/> is going to be expanded.
  /// </exception>
  /// <exception cref="InvalidOperationException"> 
  ///   Thrown if the registered method has not set the <see cref="WebTreeNode.IsEvaluated"/> flag.
  /// </exception>
  protected internal void EvaluateTreeNodeInternal (WebTreeNode node)
  {
    if (_evaluateTreeNode == null) 
      throw new NullReferenceException ("EvaluateTreeNode has no method registered but tree node '" + node.ItemID + "' is not evaluated.");
    _evaluateTreeNode (node);
    if (! node.IsEvaluated) 
      throw new InvalidOperationException ("EvaluateTreeNode called for tree node '" + node.ItemID + "' but did not evaluate the tree node.");
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    if (Page != null && ! Page.IsPostBack)
      _isLoadViewStateCompleted = true;
  }

  /// <summary> Calls the parent's <c>LoadViewState</c> method and restores this control's specific data. </summary>
  /// <param name="savedState"> An <see cref="Object"/> that represents the control state to be restored. </param>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    
    base.LoadViewState (values[0]);
    if (_enableTreeNodeViewState)
      _nodesViewState = (Triplet[]) values[1];
    else
      _nodesViewState = null;

    _isLoadViewStateCompleted = true;
  }

  /// <summary> Calls the parent's <c>SaveViewState</c> method and saves this control's specific data. </summary>
  /// <returns> Returns the server control's current view state. </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[2];

    values[0] = base.SaveViewState();
    if (_enableTreeNodeViewState)
      values[1] = SaveNodesViewStateRecursive (_nodes);

    return values;
  }

  /// <summary> Loads the settings of the <paramref name="nodes"/> from <paramref name="nodesViewState"/>. </summary>
  private void LoadNodesViewStateRecursive (Triplet[] nodesViewState, WebTreeNodeCollection nodes)
  {
    for (int i = 0; i < nodesViewState.Length; i++)
    {
      Triplet nodeViewState = (Triplet) nodesViewState[i];
      string nodeID = (string) nodeViewState.First;
      WebTreeNode node = nodes.Find (nodeID);
      if (node != null)
      {
        object[] values = (object[]) nodeViewState.Second;
        node.IsExpanded = (bool) values[0];
        if (! node.IsEvaluated)
        {
          bool isEvaluated = (bool) values[1];
          if (isEvaluated)
            EvaluateTreeNodeInternal (node);
        }
        bool isSelected = (bool) values[2];
        if (isSelected)
          node.IsSelected = true;
        LoadNodesViewStateRecursive ((Triplet[]) nodeViewState.Third, node.Children);
      }
    }
  }

  /// <summary> Saves the settings of the  <paramref name="nodes"/> and returns this view state </summary>
  private Triplet[] SaveNodesViewStateRecursive (WebTreeNodeCollection nodes)
  {
    EnsureTreeNodesCreated();
    Triplet[] nodesViewState = new Triplet[nodes.Count];
    for (int i = 0; i < nodes.Count; i++)
    {
      WebTreeNode node = nodes[i];    
      Triplet nodeViewState = new Triplet();
      nodeViewState.First = node.ItemID;
      object[] values = new object[3];
      values[0] = node.IsExpanded;
      values[1] = node.IsEvaluated;
      values[2] = node.IsSelected;
      nodeViewState.Second = values;
      nodeViewState.Third = SaveNodesViewStateRecursive (node.Children);
      nodesViewState[i] = nodeViewState;
    }
    return nodesViewState;
  }


  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  void IResourceDispatchTarget.Dispatch (IDictionary values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    Dispatch (values);
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  protected virtual void Dispatch (IDictionary values)
  {
    //  Dispatch simple properties
    ResourceDispatcher.DispatchGeneric (this, values);
  }

  /// <summary> Loads the resources into the control's properties. </summary>
  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    if (Rubicon.Web.Utilities.ControlHelper.IsDesignMode ((Control) this))
      return;

    string key;

    key = ResourceManagerUtility.GetGlobalResourceKey (AccessKey);
    if (! StringUtility.IsNullOrEmpty (key))
      AccessKey = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
    if (! StringUtility.IsNullOrEmpty (key))
      ToolTip = resourceManager.GetString (key);

    Nodes.LoadResources (resourceManager);
  }

  /// <summary> Overrides the parent control's <c>OnPreRender</c> method. </summary>
  protected override void OnPreRender(EventArgs e)
  {
    string key = typeof (WebTreeView).FullName + "_Style";
    string styleSheetUrl = null;
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (WebTreeView), ResourceType.Html, "TreeView.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    EnsureTreeNodesCreated();
    
    base.OnPreRender (e);

    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this, true);
    LoadResources (resourceManager);
  }

  /// <summary> Overrides the parent control's <c>TagKey</c> property. </summary>
  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  { 
    base.AddAttributesToRender (writer);
    if (_enableScrollBars)
      writer.AddStyleAttribute ("overflow", "auto");
  }

  /// <summary> Overrides the parent control's <c>RenderContents</c> method. </summary>
  protected override void RenderContents (HtmlTextWriter writer)
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
    {
      WcagHelper.Instance.HandleError (1, this);
    }

    ResolveNodeIcons();
    RenderNodes (writer, _nodes, true);
    if (ControlHelper.IsDesignMode (this, Context) && _nodes.Count == 0)
      RenderDesignModeContents (writer);
  }

  /// <summary> Renders the <paremref name="nodes"/> onto the <paremref name="writer"/>. </summary>
  private void RenderNodes (HtmlTextWriter writer, WebTreeNodeCollection nodes, bool isTopLevel)
  {
    for (int i = 0; i < nodes.Count; i++)
    {
      WebTreeNode node = (WebTreeNode) nodes[i];
      bool isFirstNode = i == 0;
      bool isLastNode = i + 1 == nodes.Count;

      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin node block

      bool hasExpander =   ! isTopLevel 
                        || isTopLevel && _enableTopLevelExpander;

      RenderNode (writer, node, isFirstNode, isLastNode, hasExpander); 
      bool hasChildren = node.Children.Count > 0;
      if (! hasExpander)
        node.IsExpanded = true;
      if (hasChildren && node.IsExpanded)
        RenderNodeChildren (writer, node, isLastNode, hasExpander);       

      writer.RenderEndTag(); // End node block
    }
  }

  /// <summary> Renders the <paramref name="node"/> onto the <paremref name="writer"/>. </summary>
  /// <param name="isFirstNode"> <see langword="true"/> if the node is the first node in the collection. </param>
  /// <param name="isLastNode"> <see langword="true"/> if the node is the last node in the collection. </param>
  private void RenderNode (
      HtmlTextWriter writer, 
      WebTreeNode node, 
      bool isFirstNode, 
      bool isLastNode, 
      bool hasExpander)
  {
    if (! _enableWordWrap)
      writer.AddStyleAttribute ("white-space", "nowrap");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNode);  
    writer.RenderBeginTag (HtmlTextWriterTag.Div);

    string nodePath = FormatNodePath (node);

    if (hasExpander)
      RenderNodeExpander (writer, node, nodePath, isFirstNode, isLastNode);
    RenderNodeHead (writer, node, nodePath);

    writer.RenderEndTag();
  }

  /// <summary> Renders the <paramref name="node"/>'s expander (i.e. +/-) onto the <paremref name="writer"/>. </summary>
  /// <param name="isFirstNode"> <see langword="true"/> if the node is the first node in the collection. </param>
  /// <param name="isLastNode"> <see langword="true"/> if the node is the last node in the collection. </param>
  private void RenderNodeExpander (
      HtmlTextWriter writer, 
      WebTreeNode node, 
      string nodePath, 
      bool isFirstNode, 
      bool isLastNode)
  {
    IconInfo nodeIcon = GetNodeIcon (node, isFirstNode, isLastNode);
    bool hasChildren = node.Children.Count > 0;
    bool isEvaluated = node.IsEvaluated;
    bool hasExpansionLink = hasChildren || ! isEvaluated;
    if (hasExpansionLink)
    {
      string argument = c_expansionCommandPrefix + nodePath;
      string postBackLink = Page.ClientScript.GetPostBackClientHyperlink (this, argument);
      writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
    }
    
    nodeIcon.Render (writer);
    if (hasExpansionLink)
      writer.RenderEndTag();
  }

  /// <summary> Renders the <paramref name="node"/>'s head (i.e. icon and text) onto the <paremref name="writer"/>. </summary>
  private void RenderNodeHead (HtmlTextWriter writer, WebTreeNode node, string nodePath)
  {
    if (node.IsSelected)
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeHeadSelected);  
    else
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeHead);  
    if (! StringUtility.IsNullOrEmpty (node.ToolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, node.ToolTip);

    writer.RenderBeginTag (HtmlTextWriterTag.Span);

    string argument = c_clickCommandPrefix + nodePath;
    string postBackLink = Page.ClientScript.GetPostBackClientHyperlink (this, argument);
    writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    if (   node.Icon != null 
        && ! StringUtility.IsNullOrEmpty (node.Icon.Url))
    {
      bool hasIconToolTip = ! StringUtility.IsNullOrEmpty (node.Icon.ToolTip);
      if (! hasIconToolTip)
        node.Icon.ToolTip = node.ToolTip;
      node.Icon.Render (writer);
      if (! hasIconToolTip)
        node.Icon.ToolTip = string.Empty;
      writer.Write ("&nbsp;");
    }
    if (! StringUtility.IsNullOrEmpty (node.Text))
      writer.Write (HttpUtility.HtmlEncode (node.Text));
    writer.RenderEndTag();

    writer.RenderEndTag();
  }

  /// <summary> Renders the <paramref name="node"/>'s children onto the <paremref name="writer"/>. </summary>
  /// <param name="isLastNode"> <see langword="true"/> if the node is the last node in the collection. </param>
  private void RenderNodeChildren (HtmlTextWriter writer, WebTreeNode node, bool isLastNode, bool hasExpander)
  {
    if (! hasExpander)
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTopLevelNodeChildren);  
    else if (isLastNode || ! _showLines)
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeChildrenNoLines);  
    else
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeChildren);  
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin child nodes

    RenderNodes (writer, node.Children, false);

    writer.RenderEndTag(); // End child nodes
  }

  /// <summary> Renders a dummy tree for design mode. </summary>
  private void RenderDesignModeContents (HtmlTextWriter writer)
  {
    WebTreeNodeCollection designModeNodes = new WebTreeNodeCollection (null);
    designModeNodes.SetParent (this, null);
    WebTreeNodeCollection nodes = designModeNodes;
    nodes.Add (new WebTreeNode ("node0", "Node 0"));
    nodes.Add (new WebTreeNode ("node1", "Node 1"));
    nodes.Add (new WebTreeNode ("node2", "Node 2"));
    RenderNodes (writer, designModeNodes, true);
  }

  /// <summary> Generates the string representation of the <paramref name="node"/>'s path. </summary>
  /// <remarks> ...&lt;node.Parent.Parent.ItemID&gt;|&lt;node.Parent.ItemID&gt;|&lt;ItemID&gt; </remarks>
  public string FormatNodePath (WebTreeNode node)
  {    
    if (node == null)
      return string.Empty;

    string parentPath = string.Empty;
    if (node.ParentNode != null)
    {
      parentPath = FormatNodePath (node.ParentNode);
      parentPath += c_pathSeparator;
    }
    return parentPath + node.ItemID;
  }

  /// <summary>
  ///   Parses the string generated by <see cref="FormatNodePath"/> and returns the node to which it points.
  /// </summary>
  /// <remarks> If the path cannot be resolved completly, the last valid node in the path is returned. </remarks>
  /// <param name="path"> The path to be parsed. </param>
  /// <param name="pathSegments"> Returns the IDs that comprised the path. </param>
  /// <returns> The <see cref="WebTreeNode"/> to which <paramref name="path"/> pointed. </returns>
  public WebTreeNode ParseNodePath (string path, out string[] pathSegments)
  {
    pathSegments = path.Split (c_pathSeparator);
    WebTreeNode currentNode = null;
    WebTreeNodeCollection currentNodes = _nodes;
    for (int i = 0; i < pathSegments.Length; i++)
    {
      string nodeID = pathSegments[i];
      WebTreeNode node = currentNodes.Find (nodeID);
      if (node == null)
        return currentNode;
      currentNode = node;
      currentNodes = currentNode.Children;
    }
    return currentNode;
  }

  /// <summary> Returns the URL of the node icon for the <paramref name="node"/>. </summary>
  /// <param name="isFirstNode"> <see langword="true"/> if the node is the first node in the collection. </param>
  /// <param name="isLastNode"> <see langword="true"/> if the node is the last node in the collection. </param>
  /// <returns> An image URL. </returns>
  private IconInfo GetNodeIcon (WebTreeNode node, bool isFirstNode, bool isLastNode)
  {
    bool hasChildren = node.Children.Count > 0;
    bool hasParent = node.ParentNode != null;
    bool isOnlyNode = isFirstNode && isLastNode;
    bool isExpanded = node.IsExpanded;
    bool isEvaluated = node.IsEvaluated;

    char expander = ' ';
    char type = 'r';

    if (! isEvaluated)
    {
      expander = '+';
    }
    else if (hasChildren)
    {
      if (isExpanded)
        expander = '-';
      else
        expander = '+';
    }
    else
    {
      expander = ' ';
    }

    if (hasParent)
    {
      if (isLastNode)
        type = 'L';
      else
        type = 'T';
    }
    else
    {
      if (isOnlyNode)
        type = 'r';
      else if (isFirstNode)
        type = 'F';
      else if (isLastNode)
        type = 'L';
      else
        type = 'T';
    }

    if (_showLines)
    {
      if (expander == ' ' && type == 'F')
        return _resolvedNodeIconF;
      else if (expander == '-' && type == 'F')
        return _resolvedNodeIconFMinus;
      else if (expander == '+' && type == 'F')
        return _resolvedNodeIconFPlus;
      else if (expander == ' ' && type == 'L')
        return _resolvedNodeIconL;
      else if (expander == '-' && type == 'L')
        return _resolvedNodeIconLMinus;
      else if (expander == '+' && type == 'L')
        return _resolvedNodeIconLPlus;
      else if (expander == ' ' && type == 'r')
        return _resolvedNodeIconR;
      else if (expander == '-' && type == 'r')
        return _resolvedNodeIconRMinus;
      else if (expander == '+' && type == 'r')
        return _resolvedNodeIconRPlus;
      else if (expander == ' ' && type == 'T')
        return _resolvedNodeIconT;
      else if (expander == '-' && type == 'T')
        return _resolvedNodeIconTMinus;
      else if (expander == '+' && type == 'T')
        return _resolvedNodeIconTPlus;
    }
    else
    {
      if (expander == ' ')
        return _resolvedNodeIconWhite;
      else if (expander == '-')
        return _resolvedNodeIconMinus;
      else if (expander == '+')
        return _resolvedNodeIconPlus;
    }

    return _resolvedNodeIconWhite;
  }

  /// <summary> Resolves the URLs for the node icons. </summary>
  private void ResolveNodeIcons()
  {
    _resolvedNodeIconF = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconF));
    _resolvedNodeIconFMinus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconFMinus));
    _resolvedNodeIconFPlus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconFPlus));
    _resolvedNodeIconI = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconI));
    _resolvedNodeIconL = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconL));
    _resolvedNodeIconLMinus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconLMinus));
    _resolvedNodeIconLPlus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconLPlus));
    _resolvedNodeIconMinus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconMinus));
    _resolvedNodeIconPlus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconPlus));
    _resolvedNodeIconR = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconR));
    _resolvedNodeIconRMinus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconRMinus));
    _resolvedNodeIconRPlus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconRPlus));
    _resolvedNodeIconT = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconT));
    _resolvedNodeIconTMinus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconTMinus));
    _resolvedNodeIconTPlus = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconTPlus));
    _resolvedNodeIconWhite = new IconInfo (
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconWhite));
  }
  
  /// <summary> Sets the selected tree node. </summary>
  internal void SetSelectedNode (WebTreeNode node)
  {
    if (node != null && node.TreeView != this)
      throw new InvalidOperationException ("Only tree nodes that are part of this tree can be selected.");
    if (_selectedNode != node)
    {
      if ((_selectedNode != null) && _selectedNode.IsSelected)
        _selectedNode.SetSelected (false);
      _selectedNode = node;
      if ((_selectedNode != null) && ! _selectedNode.IsSelected)
        _selectedNode.SetSelected (true);
    }
  }

  /// <summary> Gets the tree nodes displayed by this tree view. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [MergableProperty(false)]
  //  Default category
  [Description ("The tree nodes displayed by this tree view.")]
  [DefaultValue ((string) null)]
  public virtual WebTreeNodeCollection Nodes
  {
    get
    {
      if (_isLoadViewStateCompleted)
        EnsureTreeNodesCreated();
      return _nodes; 
    }
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
    get { return _enableTopLevelExpander; }
    set { _enableTopLevelExpander = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to evaluate the child nodes when expanding a tree node. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("If set, the child nodes will be evaluated when a node is expanded.")]
  [DefaultValue (false)]
  public bool EnableLookAheadEvaluation
  {
    get { return _enableLookAheadEvaluation; }
    set { _enableLookAheadEvaluation = value; }
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
    get { return _enableScrollBars; }
    set { _enableScrollBars = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to enable word wrapping. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("If set, word wrap will be enabled for the tree node's text.")]
  [DefaultValue (false)]
  public bool EnableWordWrap
  {
    get { return _enableWordWrap; }
    set { _enableWordWrap = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to show the connection lines between the nodes. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("If cleared, the tree nodes will not be connected by lines.")]
  [DefaultValue (true)]
  public bool ShowLines
  {
    get { return _showLines; }
    set { _showLines = value; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether the tree node's state information will be saved in the view state.
  /// </summary>
  /// <remarks>
  ///   If cleared, the tree view's owner control will have to save the <see cref="WebTreeNode.IsEvaluated"/> and
  ///   <see cref="WebTreeNode.IsExpanded"/> flags to provide a consistent user expierence.
  /// </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool EnableTreeNodeViewState
  {
    get { return _enableTreeNodeViewState; }
    set { _enableTreeNodeViewState = value; }
  }

  /// <summary> Occurs when a node is clicked. </summary>
  [Category ("Action")]
  [Description ("Occurs when a node is clicked.")]
  public event WebTreeNodeClickEventHandler Click
  {
    add { Events.AddHandler (s_clickEvent, value); }
    remove { Events.RemoveHandler (s_clickEvent, value); }
  }

  /// <summary> Occurs when the selected node is changed. </summary>
  [Category ("Action")]
  [Description ("Occurs when the selected node is changed.")]
  public event WebTreeNodeEventHandler SelectionChanged
  {
    add { Events.AddHandler (s_selectionChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
  }

  /// <summary> Gets the currently selected tree node. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTreeNode SelectedNode
  {
    get 
    { 
      if (_isLoadViewStateCompleted)
        EnsureTreeNodesCreated();
      return _selectedNode; 
    }
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/> node. </summary>
  /// <remarks> Class: <c>treeViewNode</c> </remarks>
  protected virtual string CssClassNode
  {
    get { return "treeViewNode"; }
  }

  /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s node head. </summary>
  /// <remarks> Class: <c>treeViewNodeHead</c> </remarks>
  protected virtual string CssClassNodeHead
  {
    get { return "treeViewNodeHead"; }
  }

  /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s node head if it is selected. </summary>
  /// <remarks> Class: <c>treeViewNodeHeadSelected</c> </remarks>
  protected virtual string CssClassNodeHeadSelected
  {
    get { return "treeViewNodeHeadSelected"; }
  }

  /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s node children. </summary>
  /// <remarks> Class: <c>treeViewNodeChildren</c> </remarks>
  protected virtual string CssClassNodeChildren
  {
    get { return "treeViewNodeChildren"; }
  }

  /// <summary> Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s last node's children. </summary>
  /// <remarks> Class: <c>treeViewNodeChildrenNoLines</c> </remarks>
  protected virtual string CssClassNodeChildrenNoLines
  {
    get { return "treeViewNodeChildrenNoLines"; }
  }

  /// <summary> 
  ///   Gets the CSS-Class applied to the <see cref="WebTreeView"/>'s top level node's children if the expander is 
  ///   hidden.
  /// </summary>
  /// <remarks> Class: <c>treeViewTopLevelNodeChildren</c> </remarks>
  protected virtual string CssClassTopLevelNodeChildren
  {
    get { return "treeViewTopLevelNodeChildren"; }
  }
  #endregion
}

/// <summary>
///   Represents the method called before a <see cref="WebTreeNode"/> with <see cref="WebTreeNode.IsEvaluated"/>
///   set to <see langword="false"/> is expanded.
/// </summary>
public delegate void EvaluateWebTreeNode (WebTreeNode expandingNode);

public delegate void InitializeRootWebTreeNodes();

}