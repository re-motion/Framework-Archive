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
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (EditConcretePositionFormResources))]
  public partial class EditConcretePositionForm : BasePage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    protected override Control FocusControl
    {
      get { return EditConcretePositionControl.FocusControl; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      ErrorsOnPageLabel.Text = GlobalResources.ErrorMessage;
    }

    protected void CloseButton_Click (object sender, EventArgs e)
    {
      if (EditConcretePositionControl.Validate ())
      {
        SaveData ();
        //CurrentFunction.CurrentTransaction.Commit ();

        ExecuteNextStep ();
      }
      else
      {
        ErrorsOnPageLabel.Visible = true;
      }
    }

    private void SaveData ()
    {
      EditConcretePositionControl.SaveValues (false);
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      ConcretePosition concretePosition = (ConcretePosition) EditConcretePositionControl.DataSource.BusinessObject;
      concretePosition.Delete ();
      throw new WxeUserCancelException ();
    }
  }
}
