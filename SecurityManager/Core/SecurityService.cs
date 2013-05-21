// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Specialized;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager
{
  public class SecurityService : ExtendedProviderBase, IRevisionBasedSecurityProvider
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (SecurityService));

    private readonly IAccessControlListFinder _accessControlListFinder;
    private readonly ISecurityTokenBuilder _securityTokenBuilder;
    private readonly IAccessResolver _accessResolver;

    public SecurityService (string name, NameValueCollection config)
        : this (name,
                config,
                SafeServiceLocator.Current.GetInstance<IAccessControlListFinder>(),
                SafeServiceLocator.Current.GetInstance<ISecurityTokenBuilder>(),
                SafeServiceLocator.Current.GetInstance<IAccessResolver>())
    {
    }

    public SecurityService (
        string name,
        NameValueCollection config,
        IAccessControlListFinder accessControlListFinder,
        ISecurityTokenBuilder securityTokenBuilder,
        IAccessResolver accessResolver)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNull ("accessControlListFinder", accessControlListFinder);
      ArgumentUtility.CheckNotNull ("securityTokenBuilder", securityTokenBuilder);
      ArgumentUtility.CheckNotNull ("accessResolver", accessResolver);

      _accessControlListFinder = accessControlListFinder;
      _securityTokenBuilder = securityTokenBuilder;
      _accessResolver = accessResolver;
    }

    public AccessType[] GetAccess (ISecurityContext context, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("principal", principal);

      using (new SecurityFreeSection())
      {
        IDomainObjectHandle<AccessControlList> acl;
        SecurityToken token;
        try
        {
          acl = _accessControlListFinder.Find (context);
          token = _securityTokenBuilder.CreateToken (principal, context);
        }
        catch (AccessControlException e)
        {
          s_log.Error ("Error during evaluation of security query.", e);
          return new AccessType[0];
        }

        return _accessResolver.GetAccessTypes (acl, token);
      }
    }

    public int GetRevision ()
    {
      return (int) ClientTransaction.CreateRootTransaction().QueryManager.GetScalar (Revision.GetGetRevisionQuery());
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
