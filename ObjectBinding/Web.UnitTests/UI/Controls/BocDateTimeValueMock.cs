using System;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UnitTests.UI;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocDateTimeValue"/> type. </summary>
public class BocDateTimeValueMock: BocDateTimeValue
{
  private WcagHelperMock _wcagHelper = new WcagHelperMock();

	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
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
