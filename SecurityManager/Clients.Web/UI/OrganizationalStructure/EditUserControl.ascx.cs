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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditUserControlResources))]
  public partial class EditUserControl : BaseControl
  {
    private BocAutoCompleteReferenceValue _owningGroupField;

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

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      _owningGroupField = GetControl<BocAutoCompleteReferenceValue> ("OwningGroupField", "OwningGroup");

      if (string.IsNullOrEmpty (_owningGroupField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_owningGroupField);

      var bocListInlineEditingConfigurator = new BocListInlineEditingConfigurator (ResourceUrlFactory);

      SubstitutedByList.EditModeControlFactory = EditableRowAutoCompleteControlFactory.Create();
      bocListInlineEditingConfigurator.Configure (SubstitutedByList, Substitution.NewObject);

      RolesList.EditModeControlFactory = UserRolesListEditableRowControlFactory.Create();
      bocListInlineEditingConfigurator.Configure (RolesList, Role.NewObject);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        RolesList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) RolesList.FixedColumns.Find ("Group"), SortingDirection.Ascending),
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) RolesList.FixedColumns.Find ("Position"), SortingDirection.Ascending));
      }

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
       
      if (CurrentFunction.TenantID == null)
        throw new InvalidOperationException ("No current tenant has been set. Possible reason: session timeout");

      _owningGroupField.Args = CurrentFunction.TenantID.ToString();
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}
