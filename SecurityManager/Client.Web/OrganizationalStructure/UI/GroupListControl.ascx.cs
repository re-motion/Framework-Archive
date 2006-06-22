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
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
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

      GroupList.LoadUnboundValue (Group.FindByClientID (CurrentFunction.ClientID), false);
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
          GroupList.LoadUnboundValue (Group.FindByClientID (CurrentFunction.ClientID), false);
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
          GroupList.LoadUnboundValue (Group.FindByClientID (CurrentFunction.ClientID), false);
      }
    }
  }
}