// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Linq;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Default implementation of the <see cref="IObjectSecurityStrategy"/> interface. A new instance of the <see cref="ObjectSecurityStrategy2"/> type
  /// is typically created and held for each <see cref="ISecurableObject"/> implementation.
  /// </summary>
  /// <remarks>
  /// The <see cref="ObjectSecurityStrategy2"/> supports the use of an <see cref="ISecurityContextFactory"/> for creating the relevant <see cref="ISecurityContext"/>, 
  /// an <see cref="IAccessTypeFilter"/> for filtering the allowed access types returned by the <see cref="ISecurityProvider"/>, 
  /// and caches the result.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [Serializable]
  public sealed class ObjectSecurityStrategy2 : IObjectSecurityStrategy
  {
    //TODO RM-6183: Test cache, test filter, test serialization, replace argument checks with Debug-Checks
    //TODO RM-6183: Refactor AccessType[] to IReadOnlyList<AccessType> and implement a Singleton-Version to allow for non-allocating checks
    // Implement CacheInvalidationToken, that can return and check a Revision (struct, holds Int64) and exposes an Invalidate() method.
    // The CacheInvalidationToken is passed via ctor. The ICachingObjectSecurityStrategy will be dropped. 

    private readonly ICache<ISecurityPrincipal, AccessType[]> _cache = CacheFactory.Create<ISecurityPrincipal, AccessType[]>();
    private readonly ISecurityContextFactory _securityContextFactory;
    private readonly IAccessTypeFilter _accessTypeFilter;

    public ObjectSecurityStrategy2 (ISecurityContextFactory securityContextFactory, IAccessTypeFilter accessTypeFilter)
    {
      ArgumentUtility.CheckNotNull ("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull ("accessTypeFilter", accessTypeFilter);

      _securityContextFactory = securityContextFactory;
      _accessTypeFilter = accessTypeFilter;
    }

    public ISecurityContextFactory SecurityContextFactory
    {
      get { return _securityContextFactory; }
    }

    public bool HasAccess (ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);

      var actualAccessTypes = GetAccessTypes (securityProvider, principal);
      return actualAccessTypes.HasAccess (requiredAccessTypes);
    }

    private AccessType[] GetAccessTypesFromCache (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      AccessType[] value;

      if (!_cache.TryGetValue (principal, out value))
        value = _cache.GetOrCreateValue (principal, delegate { return GetAccessTypes (securityProvider, principal); });

      return value;
    }

    private AccessType[] GetAccessTypes (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      var context = CreateSecurityContext();

      var accessTypes = securityProvider.GetAccess (context, principal);
      Assertion.IsNotNull (accessTypes, "GetAccess evaluated and returned null.");

      return _accessTypeFilter.Filter (accessTypes, context, principal).ToArray();
      //TODO RM-6183: check, that no new access types have been introduced.
    }

    private ISecurityContext CreateSecurityContext ()
    {
      using (new SecurityFreeSection())
      {
        var context = _securityContextFactory.CreateSecurityContext();
        Assertion.IsNotNull (context, "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

        return context;
      }
    }
  }
}