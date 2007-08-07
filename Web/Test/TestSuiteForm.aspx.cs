using System;
using Rubicon.Web.Test.MultiplePostBackCatching;

namespace Rubicon.Web.Test
{
  public partial class TestSuiteForm : BasePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      TestSuiteGenerator.GenerateTestCases (this, TestSuiteTable.Rows, "~/MultiplePostbackCatching/TestForm.aspx", "Standard");
      TestSuiteGenerator.GenerateTestCases (this, TestSuiteTable.Rows, "~/MultiplePostbackCatching/UpdatePanelTestForm.aspx", "UpdatePanel");
    }
  }
}