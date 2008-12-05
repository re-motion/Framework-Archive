// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class AccessControlEntry_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AccessControlEntry>
  {
    public override void ToText (AccessControlEntry ace, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("accessControlEntry", ace);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);

      toTextBuilder.ib<AccessControlEntry> ("").e (ace.Permissions).e ("SelUser", ace.UserCondition);
      toTextBuilder.e ("SelGroup", ace.GroupCondition).e ("SelTenant", ace.TenantCondition).e ("abstr.role", ace.SpecificAbstractRole);
      toTextBuilder.eIfNotNull ("user", ace.SpecificUser).eIfNotNull ("position", ace.SpecificPosition);
      toTextBuilder.eIfNotNull ("group", ace.SpecificGroup).eIfNotNull ("tenant", ace.SpecificTenant);
      toTextBuilder.ie ();
    }
  }
}