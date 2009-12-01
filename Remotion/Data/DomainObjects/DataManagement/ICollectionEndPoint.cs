// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents an <see cref="IEndPoint"/> holding a collection of <see cref="DomainObject"/> instances, i.e. the "many" side of a relation.
  /// </summary>
  public interface ICollectionEndPoint : IEndPoint
  {
    DomainObjectCollection OppositeDomainObjects { get; set; }
    DomainObjectCollection OriginalOppositeDomainObjectsContents { get; }
    DomainObjectCollection OriginalOppositeDomainObjectsReference { get; }

    void Touch ();
    
    IRelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject);
    IRelationEndPointModification CreateInsertModification (DomainObject insertedRelatedObject, int index);
    IRelationEndPointModification CreateAddModification (DomainObject addedRelatedObject);
    IRelationEndPointModification CreateReplaceModification (int index, DomainObject replacementObject);

    IDomainObjectCollectionData CreateDelegatingCollectionData ();
  }
}