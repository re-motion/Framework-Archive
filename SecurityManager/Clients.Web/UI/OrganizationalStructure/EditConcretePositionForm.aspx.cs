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
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditConcretePositionFormResources))]
  public partial class EditConcretePositionForm : BaseEditPage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected override IFocusableControl InitialFocusControl
    {
      get { return EditConcretePositionControl.InitialFocusControl; }
    }

    protected override void OnLoad (EventArgs e)
    {
      RegisterDataEditUserControl (EditConcretePositionControl);

      base.OnLoad (e);
    }

    protected void ApplyButton_Click (object sender, EventArgs e)
    {
      if (EditConcretePositionControl.Validate ())
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
      EditConcretePositionControl.SaveValues (false);
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }
  }
}
