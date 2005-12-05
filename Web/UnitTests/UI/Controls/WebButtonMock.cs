using System;
using System.ComponentModel;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="WebButton"/> type. </summary>
[ToolboxItem (false)]
public class WebButtonMock: WebButton
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }

  public new bool IsLegacyButtonEnabled
  {
    get { return base.IsLegacyButtonEnabled; }
  }
}

}
