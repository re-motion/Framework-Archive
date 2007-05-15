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
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Security.Web.UI;
using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.SecurityManager.Configuration;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (ClientListControlResources))]
  public partial class ClientListControl : BaseControl
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

    protected new ClientListFormFunction CurrentFunction
    {
      get { return (ClientListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        ClientList.SetSortingOrder (new BocListSortingOrderEntry ((BocColumnDefinition) ClientList.FixedColumns[0], SortingDirection.Ascending));
      ClientList.LoadUnboundValue (Client.FindAll (CurrentFunction.CurrentTransaction), IsPostBack);

      if (SecurityConfiguration.Current.SecurityService != null)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        Type clientType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetClientType ();
        NewClientButton.Visible = securityClient.HasConstructorAccess (clientType);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (HasClientChanged)
        ClientList.LoadUnboundValue (Client.FindAll (CurrentFunction.CurrentTransaction), false);
    }

    protected void ClientList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditClientFormFunction editClientFormFunction = new EditClientFormFunction (((Client) e.BusinessObject).ID);
        editClientFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editClientFormFunction);
      }
      else
      {
        if (!((EditClientFormFunction) Page.ReturningFunction).HasUserCancelled)
          ClientList.LoadUnboundValue (Client.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }

    protected void NewClientButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditClientFormFunction editClientFormFunction = new EditClientFormFunction (null);
        editClientFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editClientFormFunction);
      }
      else
      {
        if (!((EditClientFormFunction) Page.ReturningFunction).HasUserCancelled)
          ClientList.LoadUnboundValue (Client.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}