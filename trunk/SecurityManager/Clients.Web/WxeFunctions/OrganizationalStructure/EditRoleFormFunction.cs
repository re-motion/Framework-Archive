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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditRoleFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditRoleFormFunction ()
    {
    }

    protected EditRoleFormFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }

    public EditRoleFormFunction (ITransactionMode transactionMode, ObjectID organizationalStructureObjectID, User user, Group group)
      : base (transactionMode, organizationalStructureObjectID)
    {
      User = user;
      Group = group;
    }

    // methods and properties
    public User User
    {
      get { return (User) Variables["User"]; }
      private set { Variables["User"] = value; }
    }

    public Group Group
    {
      get { return (Group) Variables["Group"]; }
      private set { Variables["Group"] = value; }
    }

    public Role Role
    {
      get { return (Role) CurrentObject; }
      private set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Role = Role.NewObject ();
        Role.User = User;
        Role.Group = Group;
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditRoleForm), "UI/OrganizationalStructure/EditRoleForm.aspx");

  }
}
