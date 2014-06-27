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
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// 2nd-level cache
  /// </summary>
  public class CachingSecurityProviderDecorator : ISecurityProvider
  {
    //TODO RM-6183: Test, change to IoC

    private readonly ISecurityProvider _innerSecurityProvider;
    private readonly IGlobalAccessTypeCache _accessTypeCache;

    public CachingSecurityProviderDecorator (ISecurityProvider innerSecurityProvider, IGlobalAccessTypeCache accessTypeCache)
    {
      ArgumentUtility.CheckNotNull ("innerSecurityProvider", innerSecurityProvider);
      ArgumentUtility.CheckNotNull ("accessTypeCache", accessTypeCache);
      
      _innerSecurityProvider = innerSecurityProvider;
      _accessTypeCache = accessTypeCache;
    }

    public ISecurityProvider InnerSecurityProvider
    {
      get { return _innerSecurityProvider; }
    }

    public IGlobalAccessTypeCache AccessTypeCache
    {
      get { return _accessTypeCache; }
    }

    public bool IsNull
    {
      get { return _innerSecurityProvider.IsNull; }
    }

    public AccessType[] GetAccess (ISecurityContext context, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("principal", principal);

      var key = new GlobalAccessTypeCacheKey (context, principal);
      
      AccessType[] value;
      if (!_accessTypeCache.TryGetValue (key, out value))
        value = _accessTypeCache.GetOrCreateValue (key, delegate { return _innerSecurityProvider.GetAccess (context, principal); });

      return value;
    }
  }
}