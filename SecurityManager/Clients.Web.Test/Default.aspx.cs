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
using System.Collections.Generic;
using System.Security.Principal;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Clients.Web.Test.Domain;
using Remotion.Utilities;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web.Test
{
  public partial class _Default : System.Web.UI.Page
  {
    private ClientTransaction _clientTransaction;

    protected Global ApplicationInstance
    {
      get { return (Global) Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _clientTransaction.EnterDiscardingScope();
      if (!IsPostBack)
      {
        using (new SecurityFreeSection())
        {
          DomainObjectCollection users = 
              SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000000|System.Guid"));
          users.Combine (SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000001|System.Guid")));
          users.Combine (SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000002|System.Guid")));

          UsersField.SetBusinessObjectList (users);
          UsersField.LoadUnboundValue (ApplicationInstance.LoadUserFromSession(), false);
        }
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      _clientTransaction.EnterDiscardingScope ();
      base.OnPreRender (e);
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);
      ClientTransactionScope.ResetActiveScope();
    }

    protected void EvaluateSecurity_Click (object sender, EventArgs e)
    {
      ISecurityProvider provider = SecurityConfiguration.Current.SecurityProvider;
      SecurityContext context =
          SecurityContext.Create(typeof (File),
                 "1A",
                 "{00000004-1000-0000-0000-000000000007}",
                 string.Empty,
                 new Dictionary<string, Enum>(),
                 new Enum[] {DomainAbstractRoles.Creator});
      GenericPrincipal user = new GenericPrincipal (new GenericIdentity ("1A"), new string[0]);
      AccessType[] accessTypes = provider.GetAccess (context, user);
    }

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      if (StringUtility.IsNullOrEmpty (UsersField.BusinessObjectID))
        ApplicationInstance.SetCurrentUser (null, true);
      else
        ApplicationInstance.SetCurrentUser (SecurityManagerUser.GetObject (ObjectID.Parse (UsersField.BusinessObjectID)), true);
    }
  }
}
