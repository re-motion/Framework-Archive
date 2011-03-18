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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the operation of setting the object stored by an <see cref="ObjectEndPoint"/> that is part of a one-to-many relation.
  /// </summary>
  public class ObjectEndPointSetOneManyCommand : ObjectEndPointSetCommand
  {
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ICollectionEndPoint _newRelatedEndPoint;
    private readonly ICollectionEndPoint _oldRelatedEndPoint;

    public ObjectEndPointSetOneManyCommand (
        IObjectEndPoint modifiedEndPoint,
        DomainObject newRelatedObject,
        Action<ObjectID> oppositeObjectIDSetter,
        IRelationEndPointProvider endPointProvider)
        : base (modifiedEndPoint, newRelatedObject, oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      if (modifiedEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous)
      {
        var message = string.Format (
            "EndPoint '{0}' is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalCommand instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "modifiedEndPoint");
      }

      if (modifiedEndPoint.Definition.GetOppositeEndPointDefinition().Cardinality == CardinalityType.One)
      {
        var message = string.Format (
            "EndPoint '{0}' is from a 1:1 relation - use a ObjectEndPointSetOneOneCommand instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "modifiedEndPoint");
      }

      if (newRelatedObject == modifiedEndPoint.GetOppositeObject (true))
      {
        var message =
            string.Format (
                "New related object for EndPoint '{0}' is the same as its old value - use a ObjectEndPointSetSameCommand instead.",
                modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException (message, "newRelatedObject");
      }

      _endPointProvider = endPointProvider;

      _newRelatedEndPoint = (ICollectionEndPoint) GetOppositeEndPoint (ModifiedEndPoint, NewRelatedObject, _endPointProvider);
      _oldRelatedEndPoint = (ICollectionEndPoint) GetOppositeEndPoint (ModifiedEndPoint, OldRelatedObject, _endPointProvider);
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional 1:n set operation on this <see cref="ObjectEndPoint"/>. One of the steps is 
    /// this command, the other steps are the opposite commands on the new/old related objects.
    /// </summary>
    /// <remarks>
    /// A 1:n set operation of the form "order.Customer = newCustomer" needs three steps:
    /// <list type="bullet">
    ///   <item>order.Customer = newCustomer,</item>
    ///   <item>newCustomer.Orders.Add (order), and</item>
    ///   <item>oldCustomer.Orders.Remove (order).</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var bidirectionalModification = new ExpandedCommand (
          // => order.Customer = newCustomer
          this,
          // => newCustomer.Orders.Add (order)
          _newRelatedEndPoint.CreateAddCommand (ModifiedEndPoint.GetDomainObject()),
          // => oldCustomer.Orders.Remove (order) (remove)
          _oldRelatedEndPoint.CreateRemoveCommand (ModifiedEndPoint.GetDomainObject()));
      return bidirectionalModification;
    }
  }
}