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
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditPositionFormResources))]
  public partial class EditPositionForm : BaseEditPage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected override IFocusableControl InitialFocusControl
    {
      get { return EditPositionControl.InitialFocusControl; }
    }

    protected override void OnLoad (EventArgs e)
    {
      RegisterDataEditUserControl (EditPositionControl);

      base.OnLoad (e);
    }

    protected override void ShowErrors ()
    {
      ErrorMessageControl.ShowError ();
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      CurrentFunction.CurrentTransaction.Rollback ();
      throw new WxeUserCancelException ();
    }
  }
}
