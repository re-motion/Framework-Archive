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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  [Uses (typeof (MixinAddingPersistentProperties))]
  [Uses (typeof (NullMixin))]
  [DBTable ("MixedDomains_Target")]
  [TestDomain]
  public class TargetClassForPersistentMixin : DomainObject, ISupportsGetObject
  {
    public static TargetClassForPersistentMixin NewObject ()
    {
      return NewObject<TargetClassForPersistentMixin> ();
    }

    [StorageClassNone]
    public int RedirectedPersistentProperty
    {
      [LinqPropertyRedirection (typeof (IMixinAddingPersistentProperties), "PersistentProperty")]
      get { return ((IMixinAddingPersistentProperties) this).PersistentProperty; }
    }

    [StorageClassNone]
    public ObjectList<RelationTargetForPersistentMixin> RedirectedCollectionProperty1Side
    {
      [LinqPropertyRedirection (typeof (IMixinAddingPersistentProperties), "CollectionProperty1Side")]
      get { return ((IMixinAddingPersistentProperties) this).CollectionProperty1Side; }
    }

    [StorageClassNone]
    public IMixinAddingPersistentProperties MixedMembers
    {
      [LinqCastMethod]
      get { return (IMixinAddingPersistentProperties) this; }
    }
  }

  public static class TargetClassForPersistentMixinExtensions
  {
    [LinqCastMethod]
    public static IMixinAddingPersistentProperties GetMixedMembers (this TargetClassForPersistentMixin that)
    {
      return (IMixinAddingPersistentProperties) that;
    }
  }
}
