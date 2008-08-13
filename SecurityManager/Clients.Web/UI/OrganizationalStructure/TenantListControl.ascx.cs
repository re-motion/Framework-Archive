/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
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
        TenantList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) TenantList.FixedColumns[0], SortingDirection.Ascending));
      TenantList.LoadUnboundValue (Tenant.FindAll (), IsPostBack);

      if (SecurityConfiguration.Current.SecurityProvider != null)
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
        TenantList.LoadUnboundValue (Tenant.FindAll (), false);
    }

    protected void TenantList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditTenantFormFunction editTenantFormFunction = new EditTenantFormFunction (((Tenant) e.BusinessObject).ID);
        editTenantFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editTenantFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((EditTenantFormFunction) Page.ReturningFunction).HasUserCancelled)
          TenantList.LoadUnboundValue (Tenant.FindAll (), false);
      }
    }

    protected void NewTenantButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditTenantFormFunction editTenantFormFunction = new EditTenantFormFunction (null);
        editTenantFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editTenantFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((EditTenantFormFunction) Page.ReturningFunction).HasUserCancelled)
          TenantList.LoadUnboundValue (Tenant.FindAll (), false);
      }
    }
  }
}
