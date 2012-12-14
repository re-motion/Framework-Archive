using System;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
  public partial class SubFunction : WxePage
  {
    protected void Page_Load(object sender, EventArgs e)
    {

      if (!IsPostBack)
        dateLabel.Text = DateTime.Now.ToString();
    }

    protected void exitSubFunctionButton_OnClick (object sender, EventArgs e)
    {
      ExecuteNextStep();
    }

    protected void updateButton_OnClick (object sender, EventArgs e)
    {
      dateLabel.Text = DateTime.Now.ToString();
    }
  }
}