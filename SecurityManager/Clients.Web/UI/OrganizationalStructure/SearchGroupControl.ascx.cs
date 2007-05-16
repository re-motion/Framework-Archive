using System;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
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

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      GroupList.LoadUnboundValue (Group.FindByTenantID (CurrentTenantID, CurrentFunction.CurrentTransaction), false);
    }

    protected void GroupList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (e.Column.ItemID == "ApplyItem")
      {
        CurrentFunction.SelectedGroup = (Group) e.BusinessObject;
        Page.ExecuteNextStep ();
      }
    }
  }
}