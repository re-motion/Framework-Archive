using System;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Configuration;

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
      GroupList.LoadUnboundValue (Group.FindByClientID (CurrentClientID, CurrentFunction.CurrentTransaction), IsPostBack);

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        Type groupType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetGroupType ();
        NewGroupButton.Visible = securityClient.HasConstructorAccess (groupType);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (HasClientChanged)
        GroupList.LoadUnboundValue (Group.FindByClientID (CurrentClientID, CurrentFunction.CurrentTransaction), false);
    }

    protected void GroupList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (((Group) e.BusinessObject).ID);
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
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (null);
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
