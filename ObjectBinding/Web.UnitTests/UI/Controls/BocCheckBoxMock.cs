using System;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocCheckBox"/> type. </summary>
[ToolboxItem (false)]
public class BocCheckBoxMock: BocCheckBox
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
