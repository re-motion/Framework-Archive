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
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (SearchUserFormResources))]
  public partial class SearchUserForm : BasePage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected void Page_Load (object sender, EventArgs e)
    {
      ErrorsOnPageLabel.Text = GlobalResources.ErrorMessage;
    }
  }
}
