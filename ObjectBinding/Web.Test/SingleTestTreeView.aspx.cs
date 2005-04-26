using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.Controls;
using OBRTest;

namespace OBWTest
{
public class SingleTestTreeView : SingleBocTestBasePage
{
  protected System.Web.UI.WebControls.Label TreeViewLabel;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.WebTreeView WebTreeView;
  protected OBRTest.PersonTreeView PersonTreeView;
  protected System.Web.UI.WebControls.Button RefreshPesonTreeViewButton;
  protected System.Web.UI.WebControls.Button Button1;
  protected Rubicon.Web.UI.Controls.WebButton WebButton1;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);

    ReflectionBusinessObjectDataSourceControl.BusinessObject = person;
    

    this.DataBind();

    if (person.Children == null)
    {
      person.Children = null;
      Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectStorage.Reset();
    }
    ReflectionBusinessObjectDataSourceControl.LoadValues (IsPostBack);
    BocTreeNode node = PersonTreeView.SelectedNode;
  }

  protected override void OnPreRender(EventArgs e)
  {
    BocTreeNode node = PersonTreeView.SelectedNode;
    base.OnPreRender (e);
  }

	override protected void OnInit(EventArgs e)
	{
		InitializeComponent();
		base.OnInit(e);

    WebTreeNodeCollection nodes;

    nodes = WebTreeView.Nodes;
    nodes.Add (new WebTreeNode ("node0", "Node 0", "Images/OBRTest.Job.gif"));
    nodes.Add (new WebTreeNode ("node1", "Node 1"));
    nodes.Add (new WebTreeNode ("node2", "Node 2"));
    nodes.Add (new WebTreeNode ("node3", "Node 3"));
    nodes.Add (new WebTreeNode ("node4", "Node 4"));

    nodes = ((WebTreeNode) WebTreeView.Nodes[0]).Children;
    nodes.Add (new WebTreeNode ("node00", "Node 0-0", "Images/OBRTest.Job.gif"));
    nodes.Add (new WebTreeNode ("node01", "Node 0-1"));
    nodes.Add (new WebTreeNode ("node02", "Node 0-2"));
    nodes.Add (new WebTreeNode ("node03", "Node 0-3"));
    ((WebTreeNode) WebTreeView.Nodes[0]).IsEvaluated = true;

    nodes = ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[0]).Children[0]).Children;
    nodes.Add (new WebTreeNode ("node000", "Node 0-0-0"));
    nodes.Add (new WebTreeNode ("node001", "Node 0-0-1", "Images/OBRTest.Job.gif"));
    nodes.Add (new WebTreeNode ("node002", "Node 0-0-2", "Images/OBRTest.Job.gif"));
    nodes.Add (new WebTreeNode ("node003", "Node 0-0-3", "Images/OBRTest.Job.gif"));
    ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[0]).Children[0]).IsEvaluated = true;

    nodes = ((WebTreeNode) WebTreeView.Nodes[3]).Children;
    nodes.Add (new WebTreeNode ("node30", "Node 3-0"));
    nodes.Add (new WebTreeNode ("node31", "Node 3-1"));
    nodes.Add (new WebTreeNode ("node32", "Node 3-2"));
    nodes.Add (new WebTreeNode ("node33", "Node 3-3"));
    ((WebTreeNode) WebTreeView.Nodes[3]).IsEvaluated = true;

    nodes = ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[3]).Children[3]).Children;
    nodes.Add (new WebTreeNode ("node330", "Node 3-3-0"));
    nodes.Add (new WebTreeNode ("node331", "Node 3-3-1"));
    nodes.Add (new WebTreeNode ("node332", "Node 3-3-2"));
    nodes.Add (new WebTreeNode ("node333", "Node 3-3-3"));
    ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[3]).Children[3]).IsEvaluated = true;

    nodes = ((WebTreeNode) WebTreeView.Nodes[2]).Children;
    nodes.Add (new WebTreeNode ("node20", "Node 2-0"));
    nodes.Add (new WebTreeNode ("node21", "Node 2-1"));
    nodes.Add (new WebTreeNode ("node22", "Node 2-2"));
    nodes.Add (new WebTreeNode ("node23", "Node 2-3"));
    ((WebTreeNode) WebTreeView.Nodes[2]).IsEvaluated = true;

    nodes = ((WebTreeNode) WebTreeView.Nodes[4]).Children;
    nodes.Add (new WebTreeNode ("node40", "Node 4-0"));
    nodes.Add (new WebTreeNode ("node41", "Node 4-1"));
    nodes.Add (new WebTreeNode ("node42", "Node 4-2"));
    nodes.Add (new WebTreeNode ("node43", "Node 4-3"));
    ((WebTreeNode) WebTreeView.Nodes[4]).IsEvaluated = true;

    WebTreeView.SetEvaluateTreeNodeDelegate (new EvaluateWebTreeNode (EvaluateTreeNode));
	}
	
  private void EvaluateTreeNode (WebTreeNode node)
  {
    node.IsEvaluated = true;
  }

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.PersonTreeView.Click += new Rubicon.ObjectBinding.Web.Controls.BocTreeNodeClickEventHandler(this.PersonTreeView_Click);
    this.WebTreeView.Click += new Rubicon.Web.UI.Controls.WebTreeNodeClickEventHandler(this.TreeView_Click);
    this.WebButton1.Click += new System.EventHandler(this.WebButton1_Click);
    this.Load += new System.EventHandler(this.Page_Load);
    this.RefreshPesonTreeViewButton.Click += new EventHandler(RefreshPesonTreeViewButton_Click);
  }

  private void TreeView_Click(object sender, Rubicon.Web.UI.Controls.WebTreeNodeClickEventArgs e)
  {
    TreeViewLabel.Text = "Node = " + e.Node.Text;
  }

  private void PersonTreeView_Click(object sender, Rubicon.ObjectBinding.Web.Controls.BocTreeNodeClickEventArgs e)
  {
    TreeViewLabel.Text = "Node = " + e.Node.Text;
  }

  private void RefreshPesonTreeViewButton_Click(object sender, System.EventArgs e)
  {
    PersonTreeView.RefreshTreeNodes();
  }

  private void WebButton1_Click(object sender, System.EventArgs e)
  {
    WebTreeNode node0 = (WebTreeNode)PersonTreeView.Nodes[0];
    node0.EvaluateExpand();
    WebTreeNode node01 = (WebTreeNode)node0.Children[1];
    node01.EvaluateExpand();
    WebTreeNode node010 = (WebTreeNode)node01.Children[0];
    node010.EvaluateExpand();
    node010.IsSelected = true;
  }
}

}
