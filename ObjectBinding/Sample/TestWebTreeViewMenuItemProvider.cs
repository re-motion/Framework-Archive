using System;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Sample
{
  public class TestWebTreeViewMenuItemProvider : WebTreeViewMenuItemProvider
  {
    public TestWebTreeViewMenuItemProvider ()
    {
    }

    public override WebMenuItem[] InitalizeMenuItems (WebTreeNode node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      WebMenuItem eventMenuItem = new WebMenuItem();
      eventMenuItem.Text = "Event";
      eventMenuItem.Command.Type = CommandType.Event;

      WebMenuItem wxeMenuItem = new WebMenuItem();
      wxeMenuItem.Text = "WXE";
      wxeMenuItem.Command.Type = CommandType.WxeFunction;
      wxeMenuItem.Command.WxeFunctionCommand.TypeName = TypeUtility.GetPartialAssemblyQualifiedName (typeof (TestWxeFunction));

      WebMenuItem[] menuItems = new WebMenuItem[] {eventMenuItem, wxeMenuItem};
      return menuItems;
    }

    public override void PreRenderMenuItems (WebTreeNode node, WebMenuItemCollection menuItems)
    {
      base.PreRenderMenuItems (node, menuItems);
    }

    public override void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      base.OnMenuItemEventCommandClick (menuItem, node);
    }

    public override void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      base.OnMenuItemWxeFunctionCommandClick (menuItem, node);
    }
  }
}