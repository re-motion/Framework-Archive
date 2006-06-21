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
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.Classes;
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (EditGroupFormResources))]
  public partial class EditGroupForm : BasePage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected override Control FocusControl
    {
      get { return EditGroupControl.FocusControl; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      ErrorsOnPageLabel.Text = GlobalResources.ErrorMessage;
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      if (EditGroupControl.Validate ())
      {
        SaveData ();
        CurrentFunction.CurrentTransaction.Commit ();

        ExecuteNextStep ();
      }
      else
      {
        ErrorsOnPageLabel.Visible = true;
      }
    }

    private void SaveData ()
    {
      EditGroupControl.SaveValues (false);
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }
  }
}
