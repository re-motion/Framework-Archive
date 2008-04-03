using System;

using Remotion.Web.UI.Controls;

namespace Remotion.Templates.Generic.Web.Classes
{

public class WxeTabControl : TabControl
{
  public WxeTabControl ()
  {
  }

  public BaseWxePage BaseWxePage
  {
    get { return base.Page as BaseWxePage; }
  }

  protected override string SelectedTab
  {
    get
    {
      if (this.BaseWxePage == null)
        return string.Empty;

      return BaseWxePage.CurrentBaseFunction.NavSelectedTab;
    }
  }

  protected override string SelectedMenu
  {
    get
    {
      if (this.BaseWxePage == null)
        return string.Empty;

      return BaseWxePage.CurrentBaseFunction.NavSelectedMenu;
    }
  }
}

}
