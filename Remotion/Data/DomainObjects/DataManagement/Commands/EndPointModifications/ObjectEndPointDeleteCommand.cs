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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the deletion of an object owning a <see cref="ObjectEndPoint"/> from the end-point's point of view.
  /// </summary>
  public class ObjectEndPointDeleteCommand : RelationEndPointModificationCommand
  {
    private readonly Action<DomainObject> _oppositeObjectSetter;

    public ObjectEndPointDeleteCommand (IObjectEndPoint modifiedEndPoint, Action<DomainObject> oppositeObjectSetter)
        : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            null,
            null)
    {
      _oppositeObjectSetter = oppositeObjectSetter;
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");
    }

    protected override void ScopedNotifyClientTransactionOfBegin ()
    {
      // no notification
    }

    protected override void ScopedBegin ()
    {
      // no notification
    }

    public override void Perform ()
    {
      _oppositeObjectSetter (null);
      ModifiedEndPoint.Touch();
    }

    protected override void ScopedEnd ()
    {
      // no notification
    }

    protected override void ScopedNotifyClientTransactionOfEnd ()
    {
      // no notification
    }

    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}
