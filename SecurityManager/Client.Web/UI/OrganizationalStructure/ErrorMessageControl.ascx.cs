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
using Rubicon.SecurityManager.Client.Web.Globalization.UI.OrganizationalStructure;

namespace Rubicon.SecurityManager.Client.Web.UI.OrganizationalStructure
{
  public partial class ErrorMessageControl : UserControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      ErrorsOnPageLabel.Text = GlobalResources.ErrorMessage;
    }

    public void ShowError ()
    {
      ErrorsOnPageLabel.Visible = true;
    }
  }
}