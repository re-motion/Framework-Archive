// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Remotion.SecurityManager.Globalization.Domain.AccessControl.TenantHierarchyCondition")]
  [UndefinedEnumValue (Undefined)]
  //[Flags] //Must not be official flags enum since business objects interface does not support this.
  [DisableEnumValues (Parent)]
  public enum TenantHierarchyCondition
  {
    Undefined = 0,
    This = 1,
    Parent = 2,
    ThisAndParent = This | Parent,
  }
}
