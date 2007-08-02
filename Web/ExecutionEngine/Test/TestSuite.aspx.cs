using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI;

namespace Rubicon.PageTransition
{
  public partial class TestSuite : SmartPage
  {
    private TestControlGenerator _testControlGenerator;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      EnableViewState = false;
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      CreateTests ();
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Rubicon.Web.ResourceType.Html, "Style.css"));
    }

    private void CreateTests ()
    {
      _testControlGenerator = new TestControlGenerator (this);

      foreach (Control initialControl in _testControlGenerator.GetTestControls (null))
      {
        if (_testControlGenerator.IsEnabled (initialControl) && !_testControlGenerator.IsAlertHyperLink (initialControl))
        {
          TestSuiteTable.Rows.Add (
              CreateTest (initialControl.ID, ResolveUrl ("MultiplePostbackCatcherTest.aspx?Test=" + initialControl.ID)));
        }
      }
    }

    private TableRow CreateTest (string title, string url)
    {
      TableRow row = new TableRow();
      TableCell cell = new TableCell();
      
      HyperLink hyperLink = new HyperLink();
      hyperLink.NavigateUrl = url;
      hyperLink.Text = title;

      cell.Controls.Add (hyperLink);
      row.Cells.Add (cell);

      return row;
    }
  }
}
