using System;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditRoleFormResources))]
  public partial class EditRoleForm : BaseEditPage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected override IFocusableControl InitialFocusControl
    {
      get { return EditRoleControl.InitialFocusControl; }
    }

    protected override void OnLoad (EventArgs e)
    {
      RegisterDataEditUserControl (EditRoleControl);

      base.OnLoad (e);
    }

    protected void ApplyButton_Click (object sender, EventArgs e)
    {
      if (EditRoleControl.Validate ())
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
      EditRoleControl.SaveValues (false);
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }
  }
}
