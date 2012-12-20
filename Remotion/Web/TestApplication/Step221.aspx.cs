using System;
using System.Web.UI;
using Remotion.Web.ExecutionEngine;

namespace TestApplication
{
  public partial class Step221 : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      submitButton.Click += submitButton_OnClick;
      base.OnInit (e);
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
        nameLabel.Text = DateTime.Now.ToString();
    }

    protected void submitButton_OnClick (object sender, EventArgs e)
    {
      nameLabel.Text = DateTime.Now.ToString();
    }

    protected void nextPageButton_OnClick (object sender, EventArgs e)
    {
      ExecuteNextStep();
    }

    protected void executeAsyncSubFunction_OnClick (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
        ExecuteFunction(new TestAsyncSubFunction(), new WxeCallArguments ((Control) sender, new WxeCallOptions()));
    }
  }
}