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

namespace OBWTest
{
public class SingleTestTreeView : System.Web.UI.Page
{
  protected System.Web.UI.WebControls.Label TreeViewLabel;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.Web.UI.Controls.WebTreeView WebTreeView;

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

    WebTreeView.EvaluateTreeNode += new EvaluateWebTreeNode (EvaluateWebTreeNode);
	}

  private void EvaluateWebTreeNode (WebTreeNode node)
  {
    node.IsEvaluated = true;
  }
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.WebTreeView.Click += new Rubicon.Web.UI.Controls.WebTreeNodeClickEventHandler(this.WebTreeView_Click);

  }

  private void WebTreeView_Click(object sender, Rubicon.Web.UI.Controls.WebTreeNodeClickEventArgs e)
  {
    TreeViewLabel.Text = "Node = " + e.Node.Text;
  }
}

}
