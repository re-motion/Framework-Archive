using System;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="WebButton"/> type. </summary>
public class WebButtonMock: WebButton
{
  private WcagHelperMock _wcagHelper = new WcagHelperMock();

	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }

  public new bool IsLegacyButtonEnabled
  {
    get { return base.IsLegacyButtonEnabled; }
  }

  protected override WcagHelper WcagHelper
  {
    get { return _wcagHelper; }
  }

  public WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelper; }
  }
}

}
