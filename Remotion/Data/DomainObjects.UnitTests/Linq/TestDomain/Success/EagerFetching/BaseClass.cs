﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Success.EagerFetching
{
  [DBTable ("EagerFetching_BaseClass")]
  [ClassID ("EagerFetching_BaseClass")]
  public class BaseClass : DomainObject
  {
  }

  [ClassID ("EagerFetching_DerivedClass1")]
  public class DerivedClass1 : BaseClass
  {
    [DBBidirectionalRelation ("CollectionPropertyOneSide")]
    public virtual ObjectList<RelationTarget> CollectionPropertyManySide { get; set; }

    [DBBidirectionalRelation ("ScalarProperty1RealSide")]
    public virtual RelationTarget ScalarProperty1VirtualSide { get; set; }
  }

  [ClassID ("EagerFetching_DerivedClass2")]
  public class DerivedClass2 : BaseClass
  {
    [DBBidirectionalRelation ("ScalarProperty2VirtualSide", ContainsForeignKey = true)]
    public virtual RelationTarget ScalarProperty2RealSide { get; set; }

    public virtual RelationTarget UnidirectionalProperty { get; set; }
  }


  [DBTable ("EagerFetching_RelationTarget")]
  [ClassID ("EagerFetching_RelationTarget")]
  public class RelationTarget : DomainObject
  {
    [DBBidirectionalRelation ("CollectionPropertyManySide")]
    public virtual DerivedClass1 CollectionPropertyOneSide { get; set; }

    [DBBidirectionalRelation ("ScalarProperty1VirtualSide", ContainsForeignKey = true)]
    public virtual DerivedClass1 ScalarProperty1RealSide { get; set; }

    [DBBidirectionalRelation ("ScalarProperty2RealSide")]
    public virtual DerivedClass2 ScalarProperty2VirtualSide { get; set; }
  }
}