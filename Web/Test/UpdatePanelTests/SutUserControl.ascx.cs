using System;
using System.Threading;
using System.Web.UI;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.Test.UpdatePanelTests
{
  public partial class SutUserControl : System.Web.UI.UserControl
  {
    private PostBackEventHandler _postBackEventHandler;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      AsyncPostBackInsideUpdatePanelButton.Click += HandlePostBack;
      AsyncPostBackOutsideUpdatePanelButton.Click += HandlePostBack;
      SyncPostBackInsideUpdatePanelButton.Click += HandlePostBack;
      SyncPostBackOutsideUpdatePanelButton.Click += HandlePostBack;
      AsyncPostBackInsideUpdatePanelLinkButton.Click += HandlePostBack;
      AsyncPostBackOutsideUpdatePanelLinkButton.Click += HandlePostBack;
      SyncPostBackInsideUpdatePanelLinkButton.Click += HandlePostBack;
      SyncPostBackOutsideUpdatePanelLinkButton.Click += HandlePostBack;
      AsyncPostBackInsideUpdatePanelWebButton.Click += HandlePostBack;
      SyncPostBackInsideUpdatePanelWebButton.Click += HandlePostBack;
      DropDownMenuInsideUpdatePanel.EventCommandClick += HandlePostBack;

      _postBackEventHandler = new PostBackEventHandler ();
      _postBackEventHandler.ID = "PostBackEventHandler";
      _postBackEventHandler.PostBack += HandlePostBack;
      Controls.Add (_postBackEventHandler);

      string asyncPostBackCommandInsideUpdatePanelID = "AsyncPostBackCommandInsideUpdatePanel";
      ((ISmartPage) Page).RegisterCommandForSynchronousPostBack (_postBackEventHandler, asyncPostBackCommandInsideUpdatePanelID);
      AsyncCommandInsideUpdatePanelHyperLink.NavigateUrl = "#";
      AsyncCommandInsideUpdatePanelHyperLink.Attributes["onclick"] =
          Page.ClientScript.GetPostBackEventReference (_postBackEventHandler, asyncPostBackCommandInsideUpdatePanelID);

      string syncPostBackCommandInsideUpdatePanelID = "SyncPostBackCommandInsideUpdatePanel";
      ((ISmartPage) Page).RegisterCommandForSynchronousPostBack (_postBackEventHandler, syncPostBackCommandInsideUpdatePanelID);
      SyncCommandInsideUpdatePanelHyperLink.NavigateUrl = "#";
      SyncCommandInsideUpdatePanelHyperLink.Attributes["onclick"] =
          Page.ClientScript.GetPostBackEventReference (_postBackEventHandler, syncPostBackCommandInsideUpdatePanelID);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      UpdateStatus (null);

      WebMenuItem menuItem = new WebMenuItem ();
      menuItem.ItemID = "Item" + PostBackCount;
      menuItem.Text = "Item " + PostBackCount;
      DropDownMenuInsideUpdatePanel.MenuItems.Add (menuItem);
    }

    protected int PostBackCount
    {
      get { return (int?) ViewState["PostBackCount"] ?? 0; }
      set { ViewState["PostBackCount"] = value; }
    }

    private void HandlePostBack (object sender, EventArgs e)
    {
      Thread.Sleep (1000);
      PostBackCount++;
      UpdateStatus (sender);
    }

    private void UpdateStatus (object sender)
    {
      PostBackCountInsideUpdatePanelLabel.Text = PostBackCount.ToString ();
      PostBackCountOutsideUpdatePanelLabel.Text = PostBackCount.ToString ();

      string lastPostBack = "undefined";
      if (sender != null)
        lastPostBack = ((Control) sender).ID;
      LastPostBackInsideUpdatePanelLabel.Text = lastPostBack;
      LastPostBackOutsideUpdatePanelLabel.Text = lastPostBack;
    }
  }
}