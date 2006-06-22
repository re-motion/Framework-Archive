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
          EditRole (null, null, null, CurrentFunction.Position);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            RolesField.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole ((Role) RolesField.GetSelectedBusinessObjects ()[0], null, null, CurrentFunction.Position);
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

    protected void ConcretePositionsField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditConcretePosition (null, CurrentFunction.Position, null);
        }
        else
        {
          EditConcretePositionFormFunction returningFunction = (EditConcretePositionFormFunction) Page.ReturningFunction;

          if (returningFunction.HasUserCancelled)
            returningFunction.ConcretePosition.Delete ();
          else
            ConcretePositionsField.Value = CurrentFunction.Position.ConcretePositions;
        }
      }
      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditConcretePosition ((ConcretePosition) ConcretePositionsField.GetSelectedBusinessObjects ()[0], CurrentFunction.Position, null);
        }
        else
        {
          EditConcretePositionFormFunction returningFunction = (EditConcretePositionFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            ConcretePositionsField.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (ConcretePosition concretePosition in ConcretePositionsField.GetSelectedBusinessObjects ())
        {
          ConcretePositionsField.RemoveRow (concretePosition);
          concretePosition.Delete ();
        }
      }

      ConcretePositionsField.ClearSelectedRows ();
    }

    private void EditConcretePosition (ConcretePosition concretePosition, Position position, GroupType groupType)
    {
      EditConcretePositionFormFunction editConcretePositionFormFunction =
        new EditConcretePositionFormFunction (CurrentFunction.ClientID, concretePosition == null ? null : concretePosition.ID, position, groupType);

      editConcretePositionFormFunction.TransactionMode = WxeTransactionMode.None;
      Page.ExecuteFunction (editConcretePositionFormFunction);
    }
  }
}