// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  public class ObjectEndPointSetOneOneModification : ObjectEndPointSetModificationBase
  {
    public ObjectEndPointSetOneOneModification (ObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject)
        : base(modifiedEndPoint, newRelatedObject)
    {
      if (modifiedEndPoint.OppositeEndPointDefinition.IsAnonymous)
      {
        var message = string.Format ("EndPoint '{0}' is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalModification instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "modifiedEndPoint");
      }

      if (modifiedEndPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.Many)
      {
        var message = string.Format ("EndPoint '{0}' is from a 1:n relation - use a ObjectEndPointSetOneManyModification instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "modifiedEndPoint");
      }

      if (newRelatedObject == modifiedEndPoint.GetOppositeObject (true))
      {
        var message = string.Format ("New related object for EndPoint '{0}' is the same as its old value - use a ObjectEndPointSetSameModification instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "newRelatedObject");
      }
    }

    /// <summary>
    /// Creates all modification steps needed to perform a bidirectional 1:1 set operation on this <see cref="ObjectEndPoint"/>. One of the steps is 
    /// this modification, the other steps are the opposite modifications on the new/old related objects.
    /// </summary>
    /// <remarks>
    /// A 1:1 set operation of the form "order.OrderTicket = newTicket" needs four steps:
    /// <list type="bullet">
    ///   <item>order.OrderTicket = newTicket,</item>
    ///   <item>oldTicket.Order = null, </item>
    ///   <item>newTicket.Order = order, and</item>
    ///   <item>oldOrderOfNewTicket.OrderTicket = null.</item>
    /// </list>
    /// </remarks>
    public override BidirectionalRelationModificationBase CreateBidirectionalModification ()
    {
      var relationEndPointMap = ModifiedEndPoint.ClientTransaction.DataManager.RelationEndPointMap;
      var newRelatedEndPoint = (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (NewRelatedObject, ModifiedEndPoint.OppositeEndPointDefinition);
      var oldRelatedEndPoint = (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (OldRelatedObject, newRelatedEndPoint.Definition);

      var oldRelatedObjectOfNewRelatedObject = NewRelatedObject == null ? null : relationEndPointMap.GetRelatedObject (newRelatedEndPoint.ID, true);
      var oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) relationEndPointMap.GetRelationEndPointWithLazyLoad (oldRelatedObjectOfNewRelatedObject, ModifiedEndPoint.Definition);

      var bidirectionalModification = new NotifyingBidirectionalRelationModification (
        // => order.OrderTicket = newTicket
        this,
        // => oldTicket.Order = null (remove)
        oldRelatedEndPoint.CreateRemoveModification (ModifiedEndPoint.GetDomainObject ()),
        // => newTicket.Order = order
        newRelatedEndPoint.CreateSetModification (ModifiedEndPoint.GetDomainObject ()),
        // => oldOrderOfNewTicket.OrderTicket = null (remove)
        oldRelatedEndPointOfNewRelatedEndPoint.CreateRemoveModification (NewRelatedObject));

      return bidirectionalModification;
    }
  }
}