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
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.AccessControl;

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


    protected new EditPermissionsFormFunction CurrentFunction
    {
      get { return (EditPermissionsFormFunction) base.CurrentFunction; }
    }

    protected override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      CurrentObject.BusinessObject = CurrentFunction.SecurableClassDefinition;
      CurrentObject.LoadValues (interim);
    }

    protected override bool ValidatePage ()
    {
      bool isValid = true;
      isValid &= base.ValidatePage ();
      isValid &= CurrentObject.Validate ();

      return isValid;
    }

    protected override void SaveValues (bool interim)
    {
      base.SaveValues (interim);
      CurrentObject.SaveValues (interim);
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      CurrentFunction.CurrentTransaction.Rollback ();
      throw new WxeUserCancelException ();
    }
  }
}
