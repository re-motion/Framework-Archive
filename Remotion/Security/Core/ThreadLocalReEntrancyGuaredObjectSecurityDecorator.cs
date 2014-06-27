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
  [Serializable]
  public class ThreadLocalReEntrancyGuaredObjectSecurityDecorator : IObjectSecurityStrategy
  {
    //TODO RM-6183: Test, replace argument checks with Debug-Checks

    [ThreadStatic]
    private static bool s_isEvaluatingAccess;

    private readonly IObjectSecurityStrategy _objectSecurityStrategy;

    public ThreadLocalReEntrancyGuaredObjectSecurityDecorator (IObjectSecurityStrategy objectSecurityStrategy)
    {
      ArgumentUtility.CheckNotNull ("objectSecurityStrategy", objectSecurityStrategy);
      
      _objectSecurityStrategy = objectSecurityStrategy;
    }

    public bool HasAccess (
        ISecurityProvider securityProvider,
        ISecurityPrincipal principal,
        params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);

      if (s_isEvaluatingAccess)
      {
        throw new InvalidOperationException (
            "Multiple reentrancies on SecurityStrategy.HasAccess(...) are not allowed as they can indicate a possible infinite recursion. "
            + "Use SecurityFreeSection.IsActive to guard the computation of the SecurityContext returned by ISecurityContextFactory.CreateSecurityContext().");
      }

      try
      {
        s_isEvaluatingAccess = true;
        return _objectSecurityStrategy.HasAccess (securityProvider, principal, requiredAccessTypes);
      }
      finally
      {
        s_isEvaluatingAccess = false;
      }
    }
  }
}