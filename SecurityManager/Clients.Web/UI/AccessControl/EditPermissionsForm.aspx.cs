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
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.AccessControl;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (EditPermissionsFormResources))]
  public partial class EditPermissionsForm : BaseEditPage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected override IFocusableControl InitialFocusControl
    {
      get { return EditPermissionsControl.InitialFocusControl; }
    }

    protected override void OnLoad (EventArgs e)
    {
      RegisterDataEditUserControl (EditPermissionsControl);

      base.OnLoad (e);
    }

    protected void ApplyButton_Click (object sender, EventArgs e)
    {
      if (EditPermissionsControl.Validate ())
      {
        SaveData ();
        ExecuteNextStep ();
      }
      else
      {
        ErrorMessageControl.ShowError ();
      }
    }

    private void SaveData ()
    {
      EditPermissionsControl.SaveValues (false);
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      CurrentFunction.CurrentTransaction.Rollback ();
      throw new WxeUserCancelException ();
    }
  }
}
