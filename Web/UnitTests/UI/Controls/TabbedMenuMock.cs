using System;
using System.ComponentModel;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UnitTests.UI.Controls
{
[ToolboxItem (false)]
public class TabbedMenuMock: TabbedMenu
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}
}
