using System;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
  public partial class Step22 : WxePage
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

    protected void executeSubFunctionButton_OnClick (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
        ExecuteFunction (new TestSubFunction());
    }

    protected void executeAsyncSubFunction_OnClick (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
        ExecuteFunction(new TestAsyncSubFunction());
    }
  }
}