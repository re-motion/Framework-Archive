using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Web.UI;
using Rubicon.Web;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

[ToolboxData("<{0}:WebTreeView runat=server></{0}:WebTreeView>")]
public class WebTreeView : WebControl, IControl, IPostBackEventHandler
{
  // constants
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

  private const char c_pathSeparator = '|';
  private const string c_expansionCommandPrefix = "Expand=";
  private const string c_clickCommandPrefix = "Click=";

  // statics
  private static readonly object s_clickEvent = new object();

  // types

  // fields
  private WebTreeNodeCollection _nodes;
  private string _nodeIconF;
  private string _nodeIconFMinus;
  private string _nodeIconFPlus;
  private string _nodeIconI;
  private string _nodeIconL;
  private string _nodeIconLMinus;
  private string _nodeIconLPlus;
  private string _nodeIconMinus;
  private string _nodeIconPlus;
  private string _nodeIconR;
  private string _nodeIconRMinus;
  private string _nodeIconRPlus;
  private string _nodeIconT;
  private string _nodeIconTMinus;
  private string _nodeIconTPlus;
  private string _nodeIconWhite;

  public EvaluateWebTreeNode EvaluateTreeNode;

  //  construction and destruction
  public WebTreeView (Control ownerControl)
  {
    _nodes = new WebTreeNodeCollection (ownerControl);
    _nodes.SetParent (this, null);
  }

  public WebTreeView()
    :this (null)
  {
  }

  //  methods and properties

  void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    eventArgument = eventArgument.Trim();

    if (eventArgument.StartsWith (c_expansionCommandPrefix))
      HandleEventExpansionCommand (eventArgument.Substring (c_expansionCommandPrefix.Length));
    else if (eventArgument.StartsWith (c_clickCommandPrefix))
      HandleEventClickCommand (eventArgument.Substring (c_clickCommandPrefix.Length));
    else
      throw new ArgumentException ("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
  }

  private void HandleEventExpansionCommand (string eventArgument)
  {
    string[] pathSegments;
    WebTreeNode clickedNode = ParseNodePath (eventArgument, out pathSegments);
    if (clickedNode != null)
    {
      if (clickedNode.IsEvaluated)
      {
        clickedNode.IsExpanded = ! clickedNode.IsExpanded;
      }
      else
      {
        if (EvaluateTreeNode == null) throw new InvalidOperationException ("EvaluateTreeNode has no method registered but tree node '" + clickedNode.NodeID + "' is not evaluated.");
        EvaluateTreeNode (clickedNode);
        if (! clickedNode.IsEvaluated) throw new InvalidOperationException ("EvaluateTreeNode called for tree node '" + clickedNode.NodeID + "' but did not evaluate the tree node.");
        clickedNode.IsExpanded = true;
      }
    }
  }

  private void HandleEventClickCommand (string eventArgument)
  {
    string[] pathSegments;
    WebTreeNode clickedNode = ParseNodePath (eventArgument, out pathSegments);
    if (clickedNode != null)
      OnClick (clickedNode, pathSegments);
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

  public void CollapseAll()
  {
    _nodes.SetExpandsion (false);
  }
    
  public void ExpandAll()
  {
    _nodes.SetExpandsion (true);
  }

  protected override void OnPreRender(EventArgs e)
  {
    string key = typeof (DropDownMenu).FullName + "_Style";
    string styleSheetUrl = null;
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DropDownMenu), ResourceType.Html, "TreeView.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl);
    }
    base.OnPreRender (e);
  }


  /// <summary> Calls the parent's <c>LoadViewState</c> method and restores this control's specific data. </summary>
  /// <param name="savedState"> An <see cref="Object"/> that represents the control state to be restored. </param>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    
    base.LoadViewState (values[0]);
    LoadNodeViewStateRecursive (values[1], _nodes);
  }

  /// <summary> Calls the parent's <c>SaveViewState</c> method and saves this control's specific data. </summary>
  /// <returns> Returns the server control's current view state. </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[2];

    values[0] = base.SaveViewState();
    values[1] = SaveNodeViewStateRecursive (_nodes);

    return values;
  }

  private void LoadNodeViewStateRecursive (object viewState, WebTreeNodeCollection nodes)
  {
    Triplet[] nodeViewStates = (Triplet[]) viewState;
    foreach (Triplet nodeViewState in nodeViewStates)
    {
      string nodeID = (string) nodeViewState.First;
      WebTreeNode node = nodes.Find (nodeID);
      if (node != null)
      {
        object[] values = (object[]) nodeViewState.Second;
        node.IsExpanded = (bool) values[0];
        node.IsEvaluated = (bool) values[1];
        LoadNodeViewStateRecursive (nodeViewState.Third, node.Children);
      }
    }
  }

  private object SaveNodeViewStateRecursive (WebTreeNodeCollection nodes)
  {
    Triplet[] nodeViewStates = new Triplet[nodes.Count];
    for (int i = 0; i < nodes.Count; i++)
    {
      WebTreeNode node = nodes[i];    
      Triplet nodeViewState = new Triplet();
      nodeViewState.First = node.NodeID;
      object[] values = new object[2];
      values[0] = node.IsExpanded;
      values[1] = node.IsEvaluated;
      nodeViewState.Second = values;
      nodeViewState.Third = SaveNodeViewStateRecursive (node.Children);
      nodeViewStates[i] = nodeViewState;
    }
    return nodeViewStates;
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    InitalizeNodeIcons();
    RenderNodes (writer, _nodes);
    if (ControlHelper.IsDesignMode (this, Context) && _nodes.Count == 0)
      RenderDesignModeContents (writer);
  }

  private void RenderNodes (HtmlTextWriter writer, WebTreeNodeCollection nodes)
  {
    for (int i = 0; i < nodes.Count; i++)
    {
      WebTreeNode node = (WebTreeNode) nodes[i];
      bool isFirstNode = i == 0;
      bool isLastNode = i + 1 == nodes.Count;

      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin node block

      RenderParentNodeSection (writer, node, isFirstNode, isLastNode); 
      bool hasChildren = node.Children.Count > 0;
      if (hasChildren && node.IsExpanded)
        RenderChildNodeSection (writer, node, isLastNode);       

      writer.RenderEndTag(); // End node block
    }
  }

  private void RenderParentNodeSection (HtmlTextWriter writer, WebTreeNode node, bool isFirstNode, bool isLastNode)
  {
    writer.AddStyleAttribute ("white-space", "nowrap");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassParentNode);  
    writer.RenderBeginTag (HtmlTextWriterTag.Div);

    string nodeIcon = GetNodeIcon (node, isFirstNode, isLastNode);

    string nodePath = FormatNodePath (node);

    string argument;
    string postBackLink;

    bool hasChildren = node.Children.Count > 0;
    bool isEvaluated = node.IsEvaluated;
    bool hasExpansionLink = hasChildren || ! isEvaluated;
    if (hasExpansionLink)
    {
      argument = c_expansionCommandPrefix + nodePath;
      postBackLink = Page.GetPostBackClientHyperlink (this, argument);
      writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
    }
    writer.AddAttribute (HtmlTextWriterAttribute.Src, nodeIcon);
    writer.AddStyleAttribute ("vertical-align", "middle");
    writer.AddStyleAttribute ("border", "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Img);
    writer.RenderEndTag();
    if (hasExpansionLink)
      writer.RenderEndTag();

    argument = c_clickCommandPrefix + nodePath;
    postBackLink = Page.GetPostBackClientHyperlink (this, argument);
    writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
    if (! StringUtility.IsNullOrEmpty (node.Icon))
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Src, nodeIcon);
      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute ("border", "0");
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }
    if (! StringUtility.IsNullOrEmpty (node.Text))
      writer.Write (node.Text);
    writer.RenderEndTag();

    writer.RenderEndTag();
  }

  private void RenderChildNodeSection (HtmlTextWriter writer, WebTreeNode node, bool isLastNode)
  {
    if (isLastNode)
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassLastNodeGroup);  
    else
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassNodeGroup);  
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin child nodes

    RenderNodes (writer, node.Children);

    writer.RenderEndTag(); // End child nodes
  }

  private void RenderDesignModeContents (HtmlTextWriter writer)
  {
    WebTreeNodeCollection designModeNodes = new WebTreeNodeCollection (null);
    designModeNodes.SetParent (this, null);
    WebTreeNodeCollection nodes= designModeNodes;
    nodes.Add (new WebTreeNode ("node0", "Node 0"));
    nodes.Add (new WebTreeNode ("node1", "Node 1"));
    nodes.Add (new WebTreeNode ("node2", "Node 2"));

    nodes = ((WebTreeNode) designModeNodes[0]).Children;
    nodes.Add (new WebTreeNode ("node00", "Node 0-0"));
    nodes.Add (new WebTreeNode ("node01", "Node 0-1"));

    nodes = ((WebTreeNode) ((WebTreeNode) designModeNodes[0]).Children[0]).Children;
    nodes.Add (new WebTreeNode ("node000", "Node 0-0-0"));
    nodes.Add (new WebTreeNode ("node001", "Node 0-0-1"));

    nodes = ((WebTreeNode) designModeNodes[2]).Children;
    nodes.Add (new WebTreeNode ("node20", "Node 2-0"));

    designModeNodes.SetExpandsion (false);
    RenderNodes (writer, designModeNodes);
  }

  public string FormatNodePath (WebTreeNode node)
  {    
    string parentPath = string.Empty;
    if (node.ParentNode != null)
    {
      parentPath = FormatNodePath (node.ParentNode);
      parentPath += c_pathSeparator;
    }
    return parentPath + node.NodeID;
  }

  public WebTreeNode ParseNodePath (string path, out string[] pathSegments)
  {
    pathSegments = path.Split (c_pathSeparator);
    WebTreeNode currentNode = null;
    WebTreeNodeCollection currentNodes = Nodes;
    foreach (string nodeID in pathSegments)
    {
      WebTreeNode node = currentNodes.Find (nodeID);
      if (node == null)
        return currentNode;
      currentNode = node;
      currentNodes = currentNode.Children;
    }
    return currentNode;
  }

  private string GetNodeIcon (WebTreeNode node, bool isFirstNode, bool isLastNode)
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

    if (expander == ' ' && type == 'F')
      return _nodeIconF;
    else if (expander == '-' && type == 'F')
      return _nodeIconFMinus;
    else if (expander == '+' && type == 'F')
      return _nodeIconFPlus;
    else if (expander == ' ' && type == 'L')
      return _nodeIconL;
    else if (expander == '-' && type == 'L')
      return _nodeIconLMinus;
    else if (expander == '+' && type == 'L')
      return _nodeIconLPlus;
    else if (expander == ' ' && type == 'r')
      return _nodeIconR;
    else if (expander == '-' && type == 'r')
      return _nodeIconRMinus;
    else if (expander == '+' && type == 'r')
      return _nodeIconRPlus;
    else if (expander == ' ' && type == 'T')
      return _nodeIconT;
    else if (expander == '-' && type == 'T')
      return _nodeIconTMinus;
    else if (expander == '+' && type == 'T')
      return _nodeIconTPlus;
    
    return _nodeIconR;
  }


  private void InitalizeNodeIcons()
  {
    _nodeIconF = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconF);
    _nodeIconFMinus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconFMinus);
    _nodeIconFPlus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconFPlus);
    _nodeIconI = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconI);
    _nodeIconL = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconL);
    _nodeIconLMinus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconLMinus);
    _nodeIconLPlus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconLPlus);
    _nodeIconMinus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconMinus);
    _nodeIconPlus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconPlus);
    _nodeIconR = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconR);
    _nodeIconRMinus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconRMinus);
    _nodeIconRPlus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconRPlus);
    _nodeIconT = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconT);
    _nodeIconTMinus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconTMinus);
    _nodeIconTPlus = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconTPlus);
    _nodeIconWhite = 
        ResourceUrlResolver.GetResourceUrl (this, Context, typeof (WebTreeView), ResourceType.Image, c_nodeIconWhite);
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

  /// <summary> Occurs when a node is clicked. </summary>
  [Category ("Action")]
  [Description ("Occurs when a node is clicked.")]
  public event WebTreeNodeClickEventHandler Click
  {
    add { Events.AddHandler (s_clickEvent, value); }
    remove { Events.RemoveHandler (s_clickEvent, value); }
  }

  public virtual string CssClassParentNode
  {
    get { return "treeViewParentNode"; }
  }

  public virtual string CssClassNodeGroup
  {
    get { return "treeViewNodeGroup"; }
  }

  public virtual string CssClassLastNodeGroup
  {
    get { return "treeViewLastNodeGroup"; }
  }
}

public delegate void EvaluateWebTreeNode (WebTreeNode expandingNode);

/// <summary> Represents the method that handles the <c>Click</c> event raised when clicking on a tree node. </summary>
public delegate void WebTreeNodeClickEventHandler (object sender, WebTreeNodeClickEventArgs e);

/// <summary> Provides data for the <c>Click</c> event. </summary>
public class WebTreeNodeClickEventArgs: EventArgs
{
  private WebTreeNode _node;
  private string[] _path;

  /// <summary> Initializes an instance. </summary>
  public WebTreeNodeClickEventArgs (WebTreeNode node, string[] path)
  {
    _node = node;
    _path = path;
  }

  /// <summary> The <see cref="WebTreeNode"/> that was clicked. </summary>
  public WebTreeNode Node
  {
    get { return _node; }
  }

  /// <summary> The ID path for the clicked node. </summary>
  public string[] Path
  {
    get { return _path; }
  }
}

}