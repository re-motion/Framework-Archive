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
using Rubicon.Security.Configuration;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (PositionListControlResources))]
  public partial class PositionListControl : BaseControl
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

    protected new PositionListFormFunction CurrentFunction
    {
      get { return (PositionListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        PositionList.SetSortingOrder (new BocListSortingOrderEntry ((BocColumnDefinition) PositionList.FixedColumns[0], SortingDirection.Ascending));
      PositionList.LoadUnboundValue (Position.FindAll (CurrentFunction.CurrentTransaction), false);

      if (!SecurityConfiguration.Current.SecurityService.IsNull)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        NewPositionButton.Visible = securityClient.HasConstructorAccess (typeof (Position));
      }
    }

    protected void PositionList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditPositionFormFunction editPositionFormFunction = new EditPositionFormFunction (CurrentFunction.ClientID, ((Position) e.BusinessObject).ID);
        editPositionFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editPositionFormFunction);
      }
      else
      {
        if (!((EditPositionFormFunction) Page.ReturningFunction).HasUserCancelled)
          PositionList.LoadUnboundValue (Position.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }

    protected void NewPositionButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditPositionFormFunction editPositionFormFunction = new EditPositionFormFunction (CurrentFunction.ClientID,null);
        editPositionFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editPositionFormFunction);
      }
      else
      {
        if (!((EditPositionFormFunction) Page.ReturningFunction).HasUserCancelled)
          PositionList.LoadUnboundValue (Position.FindAll (CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}