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
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Proxy for the <see cref="Tenant"/> domain object class.
  /// </summary>
  /// <remarks>
  /// Used when a threadsafe representation of the domain object is required.
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [Serializable]
  public sealed class TenantProxy : OrganizationalStructureObjectProxy
  {
    public static TenantProxy Create (Tenant tenant)
    {
      ArgumentUtility.CheckNotNull ("tenant", tenant);

      return new TenantProxy (
          tenant.ID,
          ((IBusinessObjectWithIdentity) tenant).UniqueIdentifier,
          ((IBusinessObjectWithIdentity) tenant).DisplayName);
    }

    private TenantProxy (ObjectID id, string uniqueIdentifier, string displayName)
        : base (id, uniqueIdentifier, displayName)
    {
    }
  }
}