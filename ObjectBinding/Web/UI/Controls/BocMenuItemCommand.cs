using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BocMenuItemCommand: BocCommand
{
  public MenuItemClickEventHandler Click;

	public BocMenuItemCommand()
	{
	}

  /// <summary> Fires the <see cref="Click"/> event. </summary>
  internal virtual void OnClick (BocMenuItem menuItem)
  {
    if (Click != null)
    {
      MenuItemClickEventArgs e = new MenuItemClickEventArgs (menuItem);
      Click (this, e);
    }
  }
}

}
