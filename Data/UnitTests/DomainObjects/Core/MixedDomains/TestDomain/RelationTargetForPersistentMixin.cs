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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  [DBTable ("MixedDomains_RelationTarget")]
  [Instantiable]
  [TestDomain]
  public abstract class RelationTargetForPersistentMixin : SimpleDomainObject<RelationTargetForPersistentMixin>
  {
    [DBBidirectionalRelation ("RelationProperty")]
    public abstract TargetClassForPersistentMixin RelationProperty1 { get; set; }

    [DBBidirectionalRelation ("VirtualRelationProperty", ContainsForeignKey = true)]
    public abstract TargetClassForPersistentMixin RelationProperty2 { get; set; }

    [DBBidirectionalRelation ("CollectionProperty1Side")]
    public abstract TargetClassForPersistentMixin RelationProperty3 { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyNSide")]
    public abstract ObjectList<TargetClassForPersistentMixin> RelationProperty4 { get; }

    [DBBidirectionalRelation ("PrivateBaseRelationProperty", ContainsForeignKey = false)]
    public abstract TargetClassForPersistentMixin RelationProperty5 { get; set; }
  }
}
