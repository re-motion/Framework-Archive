// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditGroupControlResources))]
  public partial class EditGroupControl : BaseControl
  {
    private BocAutoCompleteReferenceValue _parentGroupField;

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

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      _parentGroupField = GetControl<BocAutoCompleteReferenceValue> ("ParentField", "Parent");

      if (string.IsNullOrEmpty (_parentGroupField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_parentGroupField);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      FillGroupTypeField ();

      if (ChildrenList.IsReadOnly)
        ChildrenList.Selection = RowSelection.Disabled;

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
       
      if (CurrentFunction.TenantID == null)
        throw new InvalidOperationException ("No current tenant has been set. Possible reason: session timeout");

      _parentGroupField.Args = CurrentFunction.TenantID.ToString();
    }

    private void FillGroupTypeField ()
    {
      GroupTypeField.SetBusinessObjectList (GroupType.FindAll ());
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void RolesList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
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
          EditRole ((Role) RolesList.GetSelectedBusinessObjects ()[0], null, CurrentFunction.Group, null);
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
      EditRoleFormFunction editRoleFormFunction = new EditRoleFormFunction (WxeTransactionMode.None, (role != null) ? role.ID : null, user, group);
      Page.ExecuteFunction (editRoleFormFunction, WxeCallArguments.Default);
    }

    protected void ParentValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = IsParentHierarchyValid ((Group) ParentField.Value);
    }

    private bool IsParentHierarchyValid (Group group)
    {
      var groups = group.CreateSequence (g => g.Parent, g => g != null && g != CurrentFunction.Group && g.Parent != group).ToArray();
      if (groups.Length == 0)
        return false;
      if (groups.Last().Parent != null)
        return false;
      return true;
    }
  }
}
