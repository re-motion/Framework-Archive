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
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (EditUserControlResources))]
  public partial class EditUserControl : BaseControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditUserFormFunction CurrentFunction
    {
      get { return (EditUserFormFunction) base.CurrentFunction; }
    }

    public override Control FocusControl
    {
      get { return FirstnameField; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      CurrentObject.BusinessObject = CurrentFunction.User;
      CurrentObject.LoadValues (IsPostBack);

      FillGroupField ();
    }

    private void FillGroupField ()
    {
      GroupField.SetBusinessObjectList (Group.GetByClientID (CurrentFunction.ClientID));
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}