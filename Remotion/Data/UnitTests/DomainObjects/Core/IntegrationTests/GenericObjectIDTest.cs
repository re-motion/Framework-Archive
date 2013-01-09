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

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  [Ignore ("TODO 5339")]
  public class GenericObjectIDTest : ClientTransactionBaseTest
  {
    [Test]
    public void ObjectID_Create_FromType ()
    {
      var value = Guid.NewGuid();

      var result = ObjectID.Create (typeof (Order), value);

      Assert.That (result, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (result, Is.EqualTo (new ObjectID<Order> (value)));
    }

    [Test]
    public void ObjectID_Create_FromClassID ()
    {
      var value = Guid.NewGuid ();

      var result = ObjectID.Create ("Order", value);

      Assert.That (result, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (result, Is.EqualTo (new ObjectID<Order> (value)));
    }

    [Test]
    public void ObjectID_Create_FromClassDefinition ()
    {
      var value = Guid.NewGuid ();

      var result = ObjectID.Create (GetTypeDefinition (typeof (Order)), value);

      Assert.That (result, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (result, Is.EqualTo (new ObjectID<Order> (value)));
    }

    [Test]
    public void ObjectID_CovariantInterface ()
    {
      var result = ObjectID.Create (typeof (Order), Guid.NewGuid());

      Assert.That (result, Is.AssignableTo<IObjectID<Order>> ());
      Assert.That (result, Is.AssignableTo<IObjectID<TestDomainBase>> ());
      Assert.That (result, Is.AssignableTo<IObjectID<DomainObject>> ());
    }

    [Test]
    public void GetTypedID ()
    {
      Order order = Order.NewObject();

      var orderTypedID = order.GetTypedID ();
      var testDomainBaseTypedID = ((TestDomainBase) order).GetTypedID ();
      var domainObjectTypedID = ((DomainObject) order).GetTypedID ();

      Assert.That (orderTypedID, Is.EqualTo (order.ID));
      Assert.That (orderTypedID, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (testDomainBaseTypedID, Is.EqualTo (order.ID));
      Assert.That (testDomainBaseTypedID, Is.TypeOf<ObjectID<TestDomainBase>> ());
      Assert.That (domainObjectTypedID, Is.EqualTo (order.ID));
      Assert.That (domainObjectTypedID, Is.TypeOf<ObjectID<DomainObject>> ());
    }

    [Test]
    public void Serialization ()
    {
      var value = Guid.NewGuid();
      var objectID = ObjectID.Create (typeof (Order), value);

      var deserialized = Serializer.SerializeAndDeserialize (objectID);

      Assert.That (deserialized, Is.TypeOf<IObjectID<Order>>());
      Assert.That (deserialized, Is.EqualTo (objectID));
    }
  }

  public sealed class ObjectID<T> : ObjectID, IObjectID<T>
    where T : DomainObject
  {
    public ObjectID (object value)
      : base (null, value)
    {
    }
  }

  public interface IObjectID<out T>
      where T : DomainObject
  {
  }

  public static class DomainObjectExtensions
  {
    public static IObjectID<T> GetTypedID<T> (this T domainObject) where T : DomainObject
    {
      return null;
    }
  }
}