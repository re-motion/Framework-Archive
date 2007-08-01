using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rubicon.PageTransition
{
  public partial class MultiplePostbackCatcherTests : System.Web.UI.Page
  {
    private TestControlGenerator _testControlGenerator;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      CreateTests();
    }

    private void CreateTests ()
    {
      _testControlGenerator = new TestControlGenerator (this);

      TestFixture.Rows.Add (Expect ("open", ResolveUrl ("MultiplePostbackCatcherForm.aspx"), null));

      foreach (Control initialControl in _testControlGenerator.GetTestControls (null))
      {
        foreach (Control followUpControl in _testControlGenerator.GetTestControls (initialControl.ID))
          TestFixture.Rows.AddRange (ExpectForControl (initialControl, followUpControl));
      }
    }

    private TableRow[] ExpectForControl (Control initialControl, Control followUpControl)
    {
      List<TableRow> rows = new List<TableRow>();

      rows.Add (ExpectControlClick (initialControl));
      rows.Add (ExpectControlClick (followUpControl));
      rows.Add (Expect ("waitForVisible", "SmartPageStatusIsSubmittingMessage", null));
      rows.Add (Expect ("waitForPageToLoad", "750", null));
      rows.Add (Expect ("verifyText", "TestResultLabel", initialControl.ID));

      return rows.ToArray();
    }

    private TableRow ExpectControlClick (Control control)
    {
      Control targetControl = (control.Controls.Count == 0) ? control : control.Controls[0];
      return Expect ("click", targetControl.ID, null);
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