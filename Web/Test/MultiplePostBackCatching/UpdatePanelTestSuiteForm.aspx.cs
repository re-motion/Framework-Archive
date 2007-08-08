using System;
using Rubicon.Web.Test.MultiplePostBackCatching;

namespace Rubicon.Web.Test.MultiplePostBackCatching
{
  public partial class UpdatePanelTestSuiteForm : TestBasePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      TestSuiteGenerator.GenerateTestCases (this, TestSuiteTable.Rows, "~/MultiplePostbackCatching/UpdatePanelTestForm.aspx", "UpdatePanel");
    }
  }
}