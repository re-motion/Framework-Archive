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
using Rubicon.SecurityManager.Client.Web.Classes.OrganizationalStructure;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Client.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Client.Web.Globalization.UI.OrganizationalStructure;

namespace Rubicon.SecurityManager.Client.Web.UI.OrganizationalStructure
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

      GroupTypeList.LoadUnboundValue (GroupType.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);
    }

    protected void GroupTypeList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupTypeFormFunction editGroupTypeFormFunction = new EditGroupTypeFormFunction (CurrentFunction.ClientID, ((GroupType) e.BusinessObject).ID);
        editGroupTypeFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupTypeFormFunction);
      }
      else
      {
        if (!((EditGroupTypeFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupTypeList.LoadUnboundValue (GroupType.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);
      }
    }

    protected void NewGroupTypeButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupTypeFormFunction editGroupTypeFormFunction = new EditGroupTypeFormFunction (CurrentFunction.ClientID, null);
        editGroupTypeFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupTypeFormFunction);
      }
      else
      {
        if (!((EditGroupTypeFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupTypeList.LoadUnboundValue (GroupType.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction), false);
      }
    }
  }
}