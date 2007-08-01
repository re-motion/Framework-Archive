using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.PageTransition
{
  public partial class MultiplePostbackCatcherForm : SmartPage, IPostBackEventHandler
  {
    private TestControlGenerator _testControlGenerator;

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      Assertion.IsFalse (WcagHelper.Instance.IsWaiConformanceLevelARequired());
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (WxePage), Rubicon.Web.ResourceType.Html, "Style.css"));
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      CreateTestMatrix();
    }

    
    private void CreateTestMatrix ()
    {
      _testControlGenerator = new TestControlGenerator (this);
      _testControlGenerator.Click += HandlePostBack;
      foreach (Control initialControl in _testControlGenerator.GetTestControls (null))
        TestMatrix.Rows.Add (CreateTestMatrixRow (initialControl));
    }

    private void HandlePostBack (object sender, IDEventArgs e)
    {
      SetTestResultLabelText (e.ID);
    }

    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      SetTestResultLabelText(eventArgument);
    }

    private void SetTestResultLabelText (string eventArgument)
    {
      TestResultLabel.Text = eventArgument;
      System.Threading.Thread.Sleep (500);
    }

    private TableRow CreateTestMatrixRow (Control initialControl)
    {
      TableRow row = new TableRow();
      row.Cells.Add (CreateTestMatrixCell (initialControl));
      foreach (Control followUpControl in _testControlGenerator.GetTestControls (initialControl.ID))
        row.Cells.Add (CreateTestMatrixCell (followUpControl)); 

      return row;
    }

    private TableCell CreateTestMatrixCell (Control control)
    {
      TableCell cell = new TableCell();
      cell.Controls.Add (control);
      return cell;
    }
  }
}