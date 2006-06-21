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
  [WebMultiLingualResources (typeof (SearchGroupControlResources))]
  public partial class SearchGroupControl : BaseControl
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

    protected new SearchGroupFormFunction CurrentFunction
    {
      get { return (SearchGroupFormFunction) base.CurrentFunction; }
    }

    public override Control FocusControl
    {
      get { return null; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
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
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (CurrentFunction.ClientID);
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