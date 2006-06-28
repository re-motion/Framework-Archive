using System;
using System.Web.UI;

namespace Rubicon.SecurityManager.Client.Web.UI
{
  public partial class SecurityManagerNavigationTabs : UserControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      UserNameLabel.InnerText = Page.Request.LogonUserIdentity.Name;
    }
  }
}