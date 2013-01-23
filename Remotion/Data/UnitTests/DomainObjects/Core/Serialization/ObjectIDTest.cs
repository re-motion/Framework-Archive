// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class ObjectIDTest : StandardMappingTest
  {
    [Test]
    public void ObjectIDIsSerializable ()
    {
      IObjectID<DomainObject> id = Serializer.SerializeAndDeserialize (DomainObjectIDs.Order1);
      Assert.That (id, Is.TypeOf<ObjectID<Order>>());
      Assert.That (id, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void DeserializedContent_Value ()
    {
      IObjectID<DomainObject> id = DomainObjectIDs.Order1;
      IObjectID<DomainObject> deserializedID = Serializer.SerializeAndDeserialize (id);

      Assert.That (deserializedID.Value, Is.EqualTo (id.Value));
    }

    [Test]
    public void DeserializedContent_ClassDefinition ()
    {
      IObjectID<DomainObject> id = DomainObjectIDs.Order1;
      IObjectID<DomainObject> deserializedID = Serializer.SerializeAndDeserialize (id);

      Assert.That (deserializedID.ClassDefinition, Is.EqualTo (id.ClassDefinition));
    }

    [Test]
    public void DeserializedContent_HashCode ()
    {
      IObjectID<DomainObject> id = DomainObjectIDs.Order1;
      IObjectID<DomainObject> deserializedID = Serializer.SerializeAndDeserialize (id);

      Assert.That (deserializedID.GetHashCode(), Is.EqualTo (id.GetHashCode()));
    }
  }
}
