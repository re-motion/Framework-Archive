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
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.Classes;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditUserControlResources))]
  public partial class EditUserControl : BaseControl
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

    protected new EditUserFormFunction CurrentFunction
    {
      get { return (EditUserFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return UserNameField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        RolesList.SetSortingOrder (
            new BocListSortingOrderEntry ((BocColumnDefinition) RolesList.FixedColumns[0], SortingDirection.Ascending),
            new BocListSortingOrderEntry ((BocColumnDefinition) RolesList.FixedColumns[1], SortingDirection.Ascending));
      }

      FillGroupField ();
    }

    private void FillGroupField ()
    {
      GroupField.SetBusinessObjectList (Group.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction));
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void RolesList_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole (null, CurrentFunction.User, null, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;

          RolesList.LoadValue (!returningFunction.HasUserCancelled);
          if (returningFunction.HasUserCancelled)
            returningFunction.Role.Delete ();
          else
            RolesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole ((Role) RolesList.GetSelectedBusinessObjects ()[0], CurrentFunction.User, null, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            RolesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (Role role in RolesList.GetSelectedBusinessObjects ())
        {
          RolesList.RemoveRow (role);
          role.Delete ();
        }
      }

      RolesList.ClearSelectedRows ();
    }

    private void EditRole (Role role, User user, Group group, Position position)
    {
      EditRoleFormFunction editRoleFormFunction = new EditRoleFormFunction (
          CurrentFunction.ClientID, role == null ? null : role.ID, user, group);

      editRoleFormFunction.TransactionMode = WxeTransactionMode.None;
      Page.ExecuteFunction (editRoleFormFunction);
    }
  }
}