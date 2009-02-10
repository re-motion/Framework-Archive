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

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  public abstract class Partner : Company
  {
    public new static Partner NewObject()
    {
      return DomainObject.NewObject<Partner> ();
    }

    protected Partner()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string Description { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn("PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches")]
    public abstract string PropertyWithIdenticalNameInDifferentInheritanceBranches { get; set; }
  }
}
