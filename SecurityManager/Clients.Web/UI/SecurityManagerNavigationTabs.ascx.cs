using System;
using System.Web;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Clients.Web.UI
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

      if (StringUtility.IsNullOrEmpty ( HttpContext.Current.User.Identity.Name))
        UserNameLabel.InnerText = "Anonymous";
      else
        UserNameLabel.InnerText = HttpContext.Current.User.Identity.Name;
    }
  }
}