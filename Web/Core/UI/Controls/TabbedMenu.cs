using System;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{
public class TabbedMenu: WebControl
{
  // constants
  // statics
  //private static readonly object s_clickEvent = new object();
  //private static readonly object s_selectionChangedEvent = new object();

  // types

  // fields
  private WebTabStrip _mainMenuTabStrip;
  private WebTabStrip _subMenuTabStrip;
  private WcagHelper _wcagHelper;

  // construction and destruction
  public TabbedMenu()
  {
    CreateControls();
  }

  // methods and properties
  private void CreateControls()
  {
    _mainMenuTabStrip = new WebTabStrip (this);
    _subMenuTabStrip = new WebTabStrip (this);
  }

  protected override void CreateChildControls()
  {
    _mainMenuTabStrip.ID = ID + "_MainMenuTabStrip";
    Controls.Add (_mainMenuTabStrip);

    _subMenuTabStrip.ID = ID + "_SubMenuTabStrip";
    Controls.Add (_subMenuTabStrip);
  }
}
}
