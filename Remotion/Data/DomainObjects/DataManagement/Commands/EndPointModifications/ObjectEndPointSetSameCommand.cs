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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the operation of setting the object stored by an <see cref="ObjectEndPoint"/> to the same value as before. Calling 
  /// <see cref="ExtendToAllRelatedObjects"/> results in a <see cref="IDataManagementCommand"/> that does not raise any events.
  /// </summary>
  public class ObjectEndPointSetSameCommand : ObjectEndPointSetCommand
  {
    public ObjectEndPointSetSameCommand (IObjectEndPoint modifiedEndPoint)
        : base (modifiedEndPoint, modifiedEndPoint.GetOppositeObject (true))
    {
    }

    public override void Begin ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override void End ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override void NotifyClientTransactionOfBegin ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override void NotifyClientTransactionOfEnd ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional set-same operation on this <see cref="ObjectEndPoint"/>. One of the steps is 
    /// this command, the other steps are the opposite commands on the new/old related objects.
    /// </summary>
    /// <remarks>
    /// A same-set operation of the form "order.OrderTicket = order.OrderTicket" needs two steps:
    /// <list type="bullet">
    ///   <item>order.Touch() and</item>
    ///   <item>order.OrderTicket.Touch.</item>
    /// </list>
    /// No change notifications are sent for this operation.
    /// </remarks>
    public override IDataManagementCommand ExtendToAllRelatedObjects ()
    {
      var oppositeEndPointDefinition = ModifiedEndPoint.Definition.GetOppositeEndPointDefinition ();
      if (oppositeEndPointDefinition.IsAnonymous)
      {
        return this;
      }
      else
      {
        var oppositeEndPoint = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IEndPoint> (NewRelatedObject);
        return new CompositeCommand (this, new RelationEndPointTouchCommand (oppositeEndPoint));
      }
    }
  }
}
