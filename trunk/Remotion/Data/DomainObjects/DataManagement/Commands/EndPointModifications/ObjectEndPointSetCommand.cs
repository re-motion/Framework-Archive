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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Implementations of this class represents the operation of setting the object stored by an <see cref="ObjectEndPoint"/>.
  /// </summary>
  public abstract class ObjectEndPointSetCommand : RelationEndPointModificationCommand
  {
    private readonly Action<ObjectID> _oppositeObjectIDSetter;

    protected ObjectEndPointSetCommand (IObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter)
      : base (modifiedEndPoint, modifiedEndPoint.GetOppositeObject(true), newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _oppositeObjectIDSetter = oppositeObjectIDSetter;
    }

    public override void Perform ()
    {
      var id = NewRelatedObject == null ? null : NewRelatedObject.ID;
      _oppositeObjectIDSetter(id);
    }
  }
}
