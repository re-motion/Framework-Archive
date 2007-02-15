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
  [WebMultiLingualResources (typeof (GroupListControlResources))]
  public partial class GroupListControl : BaseControl
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

    protected new GroupListFormFunction CurrentFunction
    {
      get { return (GroupListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        GroupList.SetSortingOrder (new BocListSortingOrderEntry ((BocColumnDefinition) GroupList.FixedColumns[0], SortingDirection.Ascending));
      GroupList.LoadUnboundValue (Group.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      NewGroupButton.Visible = securityClient.HasConstructorAccess (typeof (Group));
    }

    protected void GroupList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (CurrentFunction.ClientID, ((Group) e.BusinessObject).ID);
        editGroupFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupFormFunction);
      }
      else
      {
        if (!((EditGroupFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupList.LoadUnboundValue (Group.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);
      }
    }

    protected void NewGroupButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (CurrentFunction.ClientID, null);
        editGroupFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupFormFunction);
      }
      else
      {
        if (!((EditGroupFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupList.LoadUnboundValue (Group.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}