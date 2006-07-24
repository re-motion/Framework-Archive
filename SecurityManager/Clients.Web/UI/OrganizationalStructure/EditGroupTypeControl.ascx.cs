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
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditGroupTypeControlResources))]
  public partial class EditGroupTypeControl : BaseControl
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

    protected new EditGroupTypeFormFunction CurrentFunction
    {
      get { return (EditGroupTypeFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      if (!IsPostBack)
      {
        GroupsList.SetSortingOrder (new BocListSortingOrderEntry ((BocColumnDefinition) GroupsList.FixedColumns[0], SortingDirection.Ascending));
        ConcretePositionsList.SetSortingOrder (
            new BocListSortingOrderEntry ((BocColumnDefinition) ConcretePositionsList.FixedColumns[0], SortingDirection.Ascending),
            new BocListSortingOrderEntry ((BocColumnDefinition) ConcretePositionsList.FixedColumns[1], SortingDirection.Ascending));
      }
      base.OnLoad (e);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void ConcretePositionsList_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditConcretePosition (null, null, CurrentFunction.GroupType);
        }
        else
        {
          EditConcretePositionFormFunction returningFunction = (EditConcretePositionFormFunction) Page.ReturningFunction;

          if (returningFunction.HasUserCancelled)
            returningFunction.ConcretePosition.Delete ();
          else
            ConcretePositionsList.Value = CurrentFunction.GroupType.ConcretePositions;
        }
      }

      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditConcretePosition ((ConcretePosition) ConcretePositionsList.GetSelectedBusinessObjects ()[0], null, CurrentFunction.GroupType);
        }
        else
        {
          EditConcretePositionFormFunction returningFunction = (EditConcretePositionFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            ConcretePositionsList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (ConcretePosition concretePosition in ConcretePositionsList.GetSelectedBusinessObjects ())
        {
          ConcretePositionsList.RemoveRow (concretePosition);
          concretePosition.Delete ();
        }
      }

      ConcretePositionsList.ClearSelectedRows ();
    }

    private void EditConcretePosition (ConcretePosition concretePosition, Position position, GroupType groupType)
    {
      EditConcretePositionFormFunction editConcretePositionFormFunction =
        new EditConcretePositionFormFunction (CurrentFunction.ClientID, concretePosition == null ? null : concretePosition.ID, position, groupType);

      editConcretePositionFormFunction.TransactionMode = WxeTransactionMode.None;
      Page.ExecuteFunction (editConcretePositionFormFunction);
    }

    protected void GroupsList_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "AddItem")
      {
        if (!Page.IsReturningPostBack)
        {
          SearchGroupFormFunction searchGroupFormFunction = new SearchGroupFormFunction (CurrentFunction.ClientID);
          searchGroupFormFunction.TransactionMode = WxeTransactionMode.None;

          Page.ExecuteFunction (searchGroupFormFunction);
        }
        else
        {
          SearchGroupFormFunction returningFunction = (SearchGroupFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
          {
            if (!GroupsList.Value.Contains (returningFunction.SelectedGroup))
              GroupsList.AddRow (returningFunction.SelectedGroup);
          }
        }
      }

      if (e.Item.ItemID == "RemoveItem")
        GroupsList.RemoveRows (GroupsList.GetSelectedBusinessObjects ());

      GroupsList.ClearSelectedRows ();
    }
  }
}