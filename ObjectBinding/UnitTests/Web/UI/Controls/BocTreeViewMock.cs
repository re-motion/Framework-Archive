using System;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web.UI.Controls;

namespace Rubicon.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocTreeView"/> type. </summary>
[ToolboxItem (false)]
public class BocTreeViewMock: BocTreeView
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
