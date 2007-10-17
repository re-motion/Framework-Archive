using System;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class SerializationTest : ClientTransactionBaseTest
  {
    [Test]
    public void MixedTypesAreSerializable ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinAddingInterface)))
      {
        Order order = Order.NewObject ();
        Assert.IsTrue (((object) order).GetType ().IsSerializable);
      }
    }

    [Test]
    public void MixedObjectsCanBeSerializedWithoutException ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinAddingInterface)))
      {
        Order order = Order.NewObject();
        Serializer.Serialize (order);
      }
    }

    [Test]
    public void MixedObjectsCanBeDeserializedWithoutException ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinAddingInterface)))
      {
        Order order = Order.NewObject ();
        Serializer.SerializeAndDeserialize (order);
      }
    }

    [Test]
    public void DomainObjectStateIsRestored ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinAddingInterface)))
      {
        Order order = Order.NewObject ();
        order.OrderNumber = 5;
        order.OrderItems.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem4));
        Tuple<ClientTransaction, Order> deserializedObjects =
            Serializer.SerializeAndDeserialize (new Tuple<ClientTransaction, Order> (ClientTransactionScope.CurrentTransaction, order));

        using (deserializedObjects.A.EnterDiscardingScope ())
        {
          Assert.AreEqual (5, deserializedObjects.B.OrderNumber);
          Assert.IsTrue (deserializedObjects.B.OrderItems.Contains (DomainObjectIDs.OrderItem4));
        }
      }
    }

    [Test]
    public void MixinStateIsRestored ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (Order), typeof (MixinWithState)))
      {
        Order order = Order.NewObject ();
        Mixin.Get<MixinWithState> (order).State = "Sto stas stat stamus statis stant";
        Tuple<ClientTransaction, Order> deserializedObjects =
            Serializer.SerializeAndDeserialize (new Tuple<ClientTransaction, Order> (ClientTransactionScope.CurrentTransaction, order));

        Assert.AreNotSame (Mixin.Get<MixinWithState> (order), Mixin.Get<MixinWithState> (deserializedObjects.B));
        Assert.AreEqual ("Sto stas stat stamus statis stant", Mixin.Get<MixinWithState> (deserializedObjects.B).State);
      }
    }
  }
}