using System;
using System.ComponentModel;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UnitTests.UI;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocDropDownMenu"/> type. </summary>
[ToolboxItem (false)]
public class BocDropDownMenuMock: BocDropDownMenu
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
