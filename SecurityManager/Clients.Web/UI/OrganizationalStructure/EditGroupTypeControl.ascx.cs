/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
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
      base.OnLoad (e);

      if (!IsPostBack)
      {
        GroupsList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) GroupsList.FixedColumns[0], SortingDirection.Ascending));
        PositionsList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) PositionsList.FixedColumns[0], SortingDirection.Ascending));
      }

      if (GroupsList.IsReadOnly)
        GroupsList.Selection = RowSelection.Disabled;

      if (PositionsList.IsReadOnly)
        PositionsList.Selection = RowSelection.Disabled;
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void PositionsList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditGroupTypePosition (null, null, CurrentFunction.GroupType);
        }
        else
        {
          EditGroupTypePositionFormFunction returningFunction = (EditGroupTypePositionFormFunction) Page.ReturningFunction;

          PositionsList.LoadValue (!returningFunction.HasUserCancelled);
          if (returningFunction.HasUserCancelled)
            returningFunction.GroupTypePosition.Delete ();
          else
            PositionsList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditGroupTypePosition ((GroupTypePosition) PositionsList.GetSelectedBusinessObjects ()[0], null, CurrentFunction.GroupType);
        }
        else
        {
          EditGroupTypePositionFormFunction returningFunction = (EditGroupTypePositionFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            PositionsList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (GroupTypePosition groupTypePosition in PositionsList.GetSelectedBusinessObjects ())
        {
          PositionsList.RemoveRow (groupTypePosition);
          groupTypePosition.Delete ();
        }
      }

      PositionsList.ClearSelectedRows ();
    }

    private void EditGroupTypePosition (GroupTypePosition groupTypePosition, Position position, GroupType groupType)
    {
      EditGroupTypePositionFormFunction editGroupTypePositionFormFunction =
        new EditGroupTypePositionFormFunction (WxeTransactionMode.None, (groupTypePosition != null) ? groupTypePosition.ID : null, position, groupType);

      Page.ExecuteFunction (editGroupTypePositionFormFunction, WxeCallArguments.Default);
    }

    protected void GroupsList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "AddItem")
      {
        if (!Page.IsReturningPostBack)
        {
          SearchGroupFormFunction searchGroupFormFunction = new SearchGroupFormFunction (WxeTransactionMode.None);

          Page.ExecuteFunction (searchGroupFormFunction, WxeCallArguments.Default);
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
