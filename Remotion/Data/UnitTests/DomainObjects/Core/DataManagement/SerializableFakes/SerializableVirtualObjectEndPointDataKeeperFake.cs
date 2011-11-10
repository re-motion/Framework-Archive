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
using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes
{
  public class SerializableVirtualObjectEndPointDataKeeperFake : IVirtualObjectEndPointDataKeeper
  {
    public SerializableVirtualObjectEndPointDataKeeperFake ()
    {
    }

    public SerializableVirtualObjectEndPointDataKeeperFake (FlattenedDeserializationInfo info)
    {
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }

    public RelationEndPointID EndPointID
    {
      get { throw new NotImplementedException(); }
    }

    public bool ContainsOriginalObjectID (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public bool HasDataChanged ()
    {
      return false;
    }

    public void Commit ()
    {
      throw new NotImplementedException();
    }

    public void Rollback ()
    {
      throw new NotImplementedException();
    }

    public DomainObject CurrentOppositeObject
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public DomainObject OriginalOppositeObject
    {
      get { throw new NotImplementedException(); }
    }

    public IRealObjectEndPoint CurrentOppositeEndPoint
    {
      get { throw new NotImplementedException(); }
    }

    public IRealObjectEndPoint OriginalOppositeEndPoint
    {
      get { return null; }
    }

    public DomainObject OriginalItemWithoutEndPoint
    {
      get { throw new NotImplementedException(); }
    }

    public void SetDataFromSubTransaction (IVirtualObjectEndPointDataKeeper sourceDataKeeper, IRelationEndPointProvider endPointProvider)
    {
      throw new NotImplementedException();
    }
  }
}