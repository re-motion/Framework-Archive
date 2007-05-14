using System;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Security;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
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

      if (!IsPostBack)
        UserList.SetSortingOrder (new BocListSortingOrderEntry ((BocColumnDefinition) UserList.FixedColumns[0], SortingDirection.Ascending));
      UserList.LoadUnboundValue (User.FindByClientID (ClientID, CurrentFunction.CurrentTransaction), IsPostBack);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      NewUserButton.Visible = securityClient.HasConstructorAccess (typeof (User));
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (HasClientChanged)
        UserList.LoadUnboundValue (User.FindByClientID (ClientID, CurrentFunction.CurrentTransaction), false);
    }

    protected void UserList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (((User) e.BusinessObject).ID);
        editUserFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editUserFormFunction);
      }
      else
      {
        if (!((EditUserFormFunction) Page.ReturningFunction).HasUserCancelled)
          UserList.LoadUnboundValue (User.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);
      }
    }

    protected void NewUserButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (null);
        editUserFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editUserFormFunction);
      }
      else
      {
        if (!((EditUserFormFunction) Page.ReturningFunction).HasUserCancelled)
          UserList.LoadUnboundValue (User.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}