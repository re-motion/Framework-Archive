using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BocMenuItemCommand: BocCommand
{
  public MenuItemClickEventHandler Click;

  /// <summary> Initializes an instance. </summary>
  public BocMenuItemCommand()
    : this (CommandType.Event)
  {
  }

  /// <summary> Initializes an instance. </summary>
  public BocMenuItemCommand (CommandType defaultType)
    : base (defaultType)
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
