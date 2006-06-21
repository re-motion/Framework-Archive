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
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (SearchGroupTypeControlResources))]
  public partial class SearchGroupTypeControl : BaseControl
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

    protected new SearchGroupTypeFormFunction CurrentFunction
    {
      get { return (SearchGroupTypeFormFunction) base.CurrentFunction; }
    }

    public override Control FocusControl
    {
      get { return null; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      GroupTypeList.LoadUnboundValue (GroupType.GetByClientID (CurrentFunction.ClientID), false);
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
          GroupTypeList.LoadUnboundValue (GroupType.GetByClientID (CurrentFunction.ClientID), false);
      }
    }

    protected void NewGroupTypeButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupTypeFormFunction editGroupTypeFormFunction = new EditGroupTypeFormFunction (CurrentFunction.ClientID);
        editGroupTypeFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupTypeFormFunction);
      }
      else
      {
        if (!((EditGroupTypeFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupTypeList.LoadUnboundValue (GroupType.GetByClientID (CurrentFunction.ClientID), false);
      }
    }
  }
}