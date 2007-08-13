using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test
{
  public partial class ImmediatelyReturningPage : WxePage
  {
    protected override void OnLoadComplete (EventArgs e)
    {
      base.OnLoadComplete (e);
    }

    protected void ReturnButton_Click (object sender, EventArgs e)
    {
      ExecuteNextStep ();
    }
  }
}
