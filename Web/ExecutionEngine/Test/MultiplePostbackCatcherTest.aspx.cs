using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.PageTransition
{
  public partial class MultiplePostbackCatcherTest : SmartPage
  {
    private TestControlGenerator _testControlGenerator;
    private const string c_allTests = "All";

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      EnableViewState = false;
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      string testFixture = Request.QueryString["Test"];
      CreateTests (testFixture ?? c_allTests);
      HtmlHeadAppender.Current.SetTitle (testFixture ?? "All Multiple Postback Catcher Tests");
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Rubicon.Web.ResourceType.Html, "Style.css"));
    }

    private void CreateTests (string testFixture)
    {
      _testControlGenerator = new TestControlGenerator (this);

      TestTable.Rows.Add (Expect ("open", ResolveUrl ("MultiplePostbackCatcherForm.aspx?ServerDelay=150"), null));

      foreach (Control control in _testControlGenerator.GetTestControls (null))
        TestTable.Rows.AddRange (ExpectControlAttributes (control));

      foreach (Control initialControl in _testControlGenerator.GetTestControls (null))
      {
        if (testFixture == c_allTests || testFixture == initialControl.ID)
        {
          foreach (Control followUpControl in _testControlGenerator.GetTestControls (initialControl.ID))
            TestTable.Rows.AddRange (ExpectPostbackForControl (initialControl, followUpControl));
        }
      }
    }

    private TableRow[] ExpectPostbackForControl (Control initialControl, Control followUpControl)
    {
      List<TableRow> rows = new List<TableRow>();

      if (_testControlGenerator.IsEnabled (initialControl) && _testControlGenerator.IsEnabled (followUpControl))
      {
        rows.Add (ExpectControlClick (initialControl));
        rows.Add (ExpectControlClick (followUpControl));
        if (_testControlGenerator.IsAlertHyperLink (followUpControl))
        {
          rows.Add (Expect ("waitForAlert", "*", null));
          rows.Add (Expect ("assertElementNotPresent", "SmartPageStatusIsSubmittingMessage", null));
        }
        else
        {
          rows.Add (Expect ("waitForVisible", "SmartPageStatusIsSubmittingMessage", null));
        }
        rows.Add (Expect ("waitForPageToLoad", "500", null));
        rows.Add (Expect ("assertText", "TestResultLabel", initialControl.ID));
      }
      return rows.ToArray();
    }

    private TableRow[] ExpectControlAttributes (Control control)
    {
      if (_testControlGenerator.IsEnabled (control))
      {
        if (control.GetType() == typeof (Button))
          return ExpectButtonAttributes ((Button) control);
        if (control.GetType() == typeof (WebButton))
          return ExpectWebButtonAttributes ((WebButton) control);
        if (control.GetType() == typeof (LinkButton))
          return ExpectLinkButtonAttributes ((LinkButton) control);
        if (control.GetType() == typeof (HyperLink))
          return ExpectHyperLinkAttributes ((HyperLink) control);
      }
      return new TableRow[0];
    }

    private TableRow[] ExpectButtonAttributes (Button button)
    {
      List<TableRow> rows = new List<TableRow>();

      rows.Add (ExpectAttribute (button, "tagname", "INPUT"));
      rows.Add (ExpectAttribute (button, "type", button.UseSubmitBehavior ? "submit" : "button"));

      return rows.ToArray();
    }

    private TableRow[] ExpectWebButtonAttributes (WebButton button)
    {
      List<TableRow> rows = new List<TableRow>();

      rows.Add (ExpectAttribute (button, "tagname", "BUTTON"));
      rows.Add (ExpectAttribute (button, "type", button.UseSubmitBehavior ? "submit" : "button"));

      return rows.ToArray();
    }

    private TableRow[] ExpectLinkButtonAttributes (LinkButton linkButton)
    {
      List<TableRow> rows = new List<TableRow>();

      rows.Add (ExpectAttribute (linkButton, "href", "javascript:__doPostBack*"));

      return rows.ToArray();
    }

    private TableRow[] ExpectHyperLinkAttributes (HyperLink hyperLink)
    {
      List<TableRow> rows = new List<TableRow>();

      if (!_testControlGenerator.IsAlertHyperLink (hyperLink))
      {
        rows.Add (ExpectAttribute (hyperLink, "href", "*#"));
        rows.Add (ExpectAttribute (hyperLink, "onclick", "*__doPostBack*"));
      }

      return rows.ToArray();
    }

    private TableRow ExpectControlClick (Control control)
    {
      Control targetControl = (control.Controls.Count == 0) ? control : control.Controls[0];
      return Expect ("click", targetControl.ID, null);
    }

    private TableRow ExpectAttribute (Control control, string name, string value)
    {
      return Expect ("assertAttribute", control.ID + "@" + name, value);
    }

    private TableRow Expect (string command, string target, string value)
    {
      TableRow row = new TableRow();

      TableCell commandCell = new TableCell();
      commandCell.Text = command;
      row.Cells.Add (commandCell);

      TableCell targetCell = new TableCell();
      targetCell.Text = target;
      row.Cells.Add (targetCell);

      TableCell valueCell = new TableCell();
      valueCell.Text = value;
      row.Cells.Add (valueCell);

      return row;
    }
  }
}