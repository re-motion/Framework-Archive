using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Utilities;
using Rubicon.Web.Test.MultiplePostBackCatching;
using Rubicon.Web.UI;

namespace Rubicon.Web.Test.MultiplePostBackCatching
{
  public partial class SutForm : SmartPage, IPostBackEventHandler
  {
    private TestControlGenerator _testControlGenerator;
    private int _serverDelay = 2000;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      CreateTestMatrix();
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      Assertion.IsFalse (WcagHelper.Instance.IsWaiConformanceLevelARequired());
      string serverDelayString = Request.QueryString["ServerDelay"];
      if (serverDelayString != null)
        _serverDelay = int.Parse (serverDelayString);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Rubicon.Web.ResourceType.Html, "Style.css"));
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
      SetTestResultLabelText (eventArgument);
    }

    private void SetTestResultLabelText (string eventArgument)
    {
      TestResultLabel.Text = eventArgument;
      System.Threading.Thread.Sleep (_serverDelay);
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