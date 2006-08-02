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
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditPositionControlResources))]
  public partial class EditPositionControl : BaseControl
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

    protected new EditPositionFormFunction CurrentFunction
    {
      get { return (EditPositionFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        GroupTypesList.SetSortingOrder (
            new BocListSortingOrderEntry ((BocColumnDefinition) GroupTypesList.FixedColumns[0], SortingDirection.Ascending),
            new BocListSortingOrderEntry ((BocColumnDefinition) GroupTypesList.FixedColumns[1], SortingDirection.Ascending));
      }
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void GroupTypesList_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditGroupTypePosition (null, CurrentFunction.Position, null);
        }
        else
        {
          EditGroupTypePositionFormFunction returningFunction = (EditGroupTypePositionFormFunction) Page.ReturningFunction;

          GroupTypesList.LoadValue (!returningFunction.HasUserCancelled);
          if (returningFunction.HasUserCancelled)
            returningFunction.GroupTypePosition.Delete ();
          else
            GroupTypesList.IsDirty = true;
        }
      }
      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditGroupTypePosition ((GroupTypePosition) GroupTypesList.GetSelectedBusinessObjects ()[0], CurrentFunction.Position, null);
        }
        else
        {
          EditGroupTypePositionFormFunction returningFunction = (EditGroupTypePositionFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            GroupTypesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (GroupTypePosition groupTypePosition in GroupTypesList.GetSelectedBusinessObjects ())
        {
          GroupTypesList.RemoveRow (groupTypePosition);
          groupTypePosition.Delete ();
        }
      }

      GroupTypesList.ClearSelectedRows ();
    }

    private void EditGroupTypePosition (GroupTypePosition groupTypePosition, Position position, GroupType groupType)
    {
      EditGroupTypePositionFormFunction editGroupTypePositionFormFunction =
        new EditGroupTypePositionFormFunction (CurrentFunction.ClientID, groupTypePosition == null ? null : groupTypePosition.ID, position, groupType);

      editGroupTypePositionFormFunction.TransactionMode = WxeTransactionMode.None;
      Page.ExecuteFunction (editGroupTypePositionFormFunction);
    }
  }
}