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
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (EditGroupControlResources))]
  public partial class EditGroupControl : BaseControl
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

    protected new EditGroupFormFunction CurrentFunction
    {
      get { return (EditGroupFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      FillParentField ();
      FillGroupTypeField ();
    }

    private void FillGroupTypeField ()
    {
      GroupTypeField.SetBusinessObjectList (GroupType.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction));
    }

    private void FillParentField ()
    {
      ParentField.SetBusinessObjectList (CurrentFunction.Group.GetPossibleParentGroups (CurrentFunction.ClientID));
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void RolesField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole (null, null, CurrentFunction.Group, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;

          if (returningFunction.HasUserCancelled)
            returningFunction.Role.Delete ();
          else
            RolesField.Value = CurrentFunction.Group.Roles;
        }
      }

      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole ((Role) RolesField.GetSelectedBusinessObjects ()[0], null, CurrentFunction.Group, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;
          if (!returningFunction.HasUserCancelled)
            RolesField.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (Role role in RolesField.GetSelectedBusinessObjects ())
        {
          RolesField.RemoveRow (role);
          role.Delete ();
        }
      }

      RolesField.ClearSelectedRows ();
    }

    private void EditRole (Role role, User user, Group group, Position position)
    {
      EditRoleFormFunction editRoleFormFunction = new EditRoleFormFunction (
          CurrentFunction.ClientID, role == null ? null : role.ID, user, group, position);

      editRoleFormFunction.TransactionMode = WxeTransactionMode.None;
      Page.ExecuteFunction (editRoleFormFunction);
    }

    protected void ChildrenField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
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
            ChildrenField.AddRow (returningFunction.SelectedGroup);
        }
      }

      if (e.Item.ItemID == "RemoveItem")
        ChildrenField.RemoveRows (ChildrenField.GetSelectedBusinessObjects ());

      ChildrenField.ClearSelectedRows ();
    }
  }
}