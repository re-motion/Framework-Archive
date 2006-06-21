using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.Classes;

namespace Rubicon.Kis.Client.Web.UI.AdministrationUI
{
  public partial class NavigationTabs : BaseControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
        UserFullNameTextValue.Text = Page.Request.LogonUserIdentity.Name;
    }
  }
}