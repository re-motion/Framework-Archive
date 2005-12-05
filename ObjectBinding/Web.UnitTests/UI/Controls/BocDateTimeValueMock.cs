using System;
using System.ComponentModel;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UnitTests.UI;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocDateTimeValue"/> type. </summary>
[ToolboxItem (false)]
public class BocDateTimeValueMock: BocDateTimeValue
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
