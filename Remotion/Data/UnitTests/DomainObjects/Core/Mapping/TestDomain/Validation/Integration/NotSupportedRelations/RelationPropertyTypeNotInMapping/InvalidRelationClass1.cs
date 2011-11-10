// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.
    RelationPropertyTypeNotInMapping
{
  [DBTable]
  [ClassID ("BidirectionalRelation_RelationPropertyTypeNotInMapping_InvalidRelationClass1")]
  public class InvalidRelationClass1 : DomainObject
  {
    [DBBidirectionalRelation("RelationProperty", ContainsForeignKey = true)]
    public ClassNotInMapping RelationProperty { get; set; }
  }
}