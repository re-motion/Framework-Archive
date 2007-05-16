using System;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.SecurityManager.Configuration;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (TenantListControlResources))]
  public partial class TenantListControl : BaseControl
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

    protected new TenantListFormFunction CurrentFunction
    {
      get { return (TenantListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        TenantList.SetSortingOrder (new BocListSortingOrderEntry ((BocColumnDefinition) TenantList.FixedColumns[0], SortingDirection.Ascending));
      TenantList.LoadUnboundValue (Tenant.FindAll (CurrentFunction.CurrentTransaction), IsPostBack);

      if (SecurityConfiguration.Current.SecurityService != null)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        Type tenantType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetTenantType ();
        NewTenantButton.Visible = securityClient.HasConstructorAccess (tenantType);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (HasTenantChanged)
        TenantList.LoadUnboundValue (Tenant.FindAll (CurrentFunction.CurrentTransaction), false);
    }

    protected void TenantList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditTenantFormFunction editTenantFormFunction = new EditTenantFormFunction (((Tenant) e.BusinessObject).ID);
        editTenantFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editTenantFormFunction);
      }
      else
      {
        if (!((EditTenantFormFunction) Page.ReturningFunction).HasUserCancelled)
          TenantList.LoadUnboundValue (Tenant.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }

    protected void NewTenantButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditTenantFormFunction editTenantFormFunction = new EditTenantFormFunction (null);
        editTenantFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editTenantFormFunction);
      }
      else
      {
        if (!((EditTenantFormFunction) Page.ReturningFunction).HasUserCancelled)
          TenantList.LoadUnboundValue (Tenant.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}