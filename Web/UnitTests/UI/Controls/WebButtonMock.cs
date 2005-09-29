using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="WebButton"/> type. </summary>
public class WebButtonMock: WebButton
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
