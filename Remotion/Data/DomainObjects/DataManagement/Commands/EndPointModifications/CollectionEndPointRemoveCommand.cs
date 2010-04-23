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
using System;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the removal of an element from a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public class CollectionEndPointRemoveCommand : RelationEndPointModificationCommand
  {
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;
    private readonly int _index;

    public CollectionEndPointRemoveCommand (
        ICollectionEndPoint modifiedEndPoint, 
        DomainObject removedObject, 
        IDomainObjectCollectionData collectionData)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            ArgumentUtility.CheckNotNull ("removedObject", removedObject),
            null)
    {
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _index = modifiedEndPoint.OppositeDomainObjects.IndexOf (removedObject);
      _modifiedCollectionData = collectionData;
      _modifiedCollection = modifiedEndPoint.OppositeDomainObjects;
    }

    public DomainObjectCollection ModifiedCollection
    {
      get { return _modifiedCollection; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    protected override void ScopedBegin ()
    {
      ((IDomainObjectCollectionEventRaiser) ModifiedCollection).BeginRemove (_index, OldRelatedObject);
      base.ScopedBegin ();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Remove (OldRelatedObject);
      ModifiedEndPoint.Touch ();
    }

    protected override void ScopedEnd ()
    {
      base.ScopedEnd ();
      ((IDomainObjectCollectionEventRaiser) ModifiedCollection).EndRemove (_index, OldRelatedObject);
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional remove operation from this collection end point.
    /// </summary>
    /// <remarks>
    /// A remove operation of the form "customer.Orders.Remove (RemovedOrder)" needs two steps:
    /// <list type="bullet">
    ///   <item>RemovedOrder.Customer = null and</item>
    ///   <item>customer.Orders.Remove (removedOrder).</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var removedEndPoint = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint> (OldRelatedObject);
      return new ExpandedCommand (
          removedEndPoint.CreateRemoveCommand (ModifiedEndPoint.GetDomainObject ()), 
          this);
    }
  }
}
