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
  /// <see cref="INullObject"/>-implementation of the <see cref="IFunctionalSecurityStrategy"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class NullFunctionalSecurityStrategy : IFunctionalSecurityStrategy
  {
    public bool HasAccess (Type type, ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      return true;
    }

    public bool IsNull
    {
      get { return true; }
    }
  }
}