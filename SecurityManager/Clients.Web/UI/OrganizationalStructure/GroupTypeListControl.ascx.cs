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

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (GroupTypeListControlResources))]
  public partial class GroupTypeListControl : BaseControl
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

    protected new GroupTypeListFormFunction CurrentFunction
    {
      get { return (GroupTypeListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        GroupTypeList.SetSortingOrder (new BocListSortingOrderEntry ((BocColumnDefinition) GroupTypeList.FixedColumns[0], SortingDirection.Ascending));
      GroupTypeList.LoadUnboundValue (GroupType.FindAll (CurrentFunction.CurrentTransaction), false);

      if (SecurityConfiguration.Current.SecurityProvider != null)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        NewGroupTypeButton.Visible = securityClient.HasConstructorAccess (typeof (GroupType));
      }
    }

    protected void GroupTypeList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupTypeFormFunction editGroupTypeFormFunction = new EditGroupTypeFormFunction (((GroupType) e.BusinessObject).ID);
        editGroupTypeFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupTypeFormFunction);
      }
      else
      {
        if (!((EditGroupTypeFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupTypeList.LoadUnboundValue (GroupType.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }

    protected void NewGroupTypeButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupTypeFormFunction editGroupTypeFormFunction = new EditGroupTypeFormFunction (null);
        editGroupTypeFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupTypeFormFunction);
      }
      else
      {
        if (!((EditGroupTypeFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupTypeList.LoadUnboundValue (GroupType.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}