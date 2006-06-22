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
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;
using Rubicon.Web.UI.Globalization;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (UserListControlResources))]
  public partial class UserListControl : BaseControl
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

    protected new UserListFormFunction CurrentFunction
    {
      get { return (UserListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      UserList.LoadUnboundValue (User.FindByClientID (CurrentFunction.ClientID), false);
    }

    protected void UserList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (CurrentFunction.ClientID, ((User) e.BusinessObject).ID);
        editUserFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editUserFormFunction);
      }
      else
      {
        if (!((EditUserFormFunction) Page.ReturningFunction).HasUserCancelled)
          UserList.LoadUnboundValue (User.FindByClientID (CurrentFunction.ClientID), false);
      }
    }

    protected void NewUserButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (CurrentFunction.ClientID, null);
        editUserFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editUserFormFunction);
      }
      else
      {
        if (!((EditUserFormFunction) Page.ReturningFunction).HasUserCancelled)
          UserList.LoadUnboundValue (User.FindByClientID (CurrentFunction.ClientID), false);
      }
    }
  }
}