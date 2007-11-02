using System;
using NUnit.Framework;
using System.Collections.Generic;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class DomainObjectTransporterTest : StandardMappingTest
  {
    DomainObjectTransporter _transporter;

    public override void SetUp ()
    {
      base.SetUp ();
      _transporter = new DomainObjectTransporter ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreEqual (0, _transporter.ObjectCount);
      Assert.IsEmpty (GetIDList ());
    }

    [Test]
    public void Load ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectCount);
      Assert.That (GetIDList (), Is.EqualTo (new ObjectID[] { DomainObjectIDs.Order1 }));
    }

    [Test]
    public void Load_Twice ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectCount);
      Assert.That (GetIDList (), Is.EqualTo (new ObjectID[] { DomainObjectIDs.Order1 }));
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException), ExpectedMessage = "Object 'Order|.*|System.Guid' could not be found.",
        MatchType = MessageMatch.Regex)]
    public void Load_Inexistent ()
    {
      _transporter.Load (new ObjectID (typeof (Order), Guid.NewGuid()));
    }

    [Test]
    public void Load_Multiple ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectCount);
      _transporter.Load (DomainObjectIDs.Order2);
      Assert.AreEqual (2, _transporter.ObjectCount);
      _transporter.Load (DomainObjectIDs.OrderItem1);
      Assert.AreEqual (3, _transporter.ObjectCount);
      Assert.That (GetIDList (), Is.EqualTo (new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 }));
    }

    [Test]
    public void LoadWithRelatedObjects ()
    {
      _transporter.LoadWithRelatedObjects (DomainObjectIDs.Order1);
      Assert.AreEqual (6, _transporter.ObjectCount);
      Assert.That (GetIDList (), Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderTicket1, DomainObjectIDs.Customer1, DomainObjectIDs.Official1 }));
    }

    [Test]
    public void LoadRecursive ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Employee1);
      Assert.AreEqual (5, _transporter.ObjectCount);
      Assert.That (GetIDList (), Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee4, DomainObjectIDs.Computer2,
          DomainObjectIDs.Employee5, DomainObjectIDs.Computer3 }));
    }

    /*[Test]
    public void LoadRecursive_WithFilter_DontStopRecursion ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Order1,
          delegate (ObjectID id, ref bool continueRecursion) { return typeof (OrderItem).IsAssignableFrom (id.ClassDefinition.ClassType); });
      
      List<ObjectID> flattenedGraphIDs;

      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        Order order1 = Order.GetObject (DomainObjectIDs.Order1);
        Set<DomainObject> flattenedGraph = order1.GetFlattenedRelatedObjectGraph ();
        flattenedGraphIDs = new List<ObjectID> ();
        foreach (DomainObject domainObject in flattenedGraph)
        {
          if (domainObject is OrderItem)
            flattenedGraphIDs.Add (domainObject.ID);
        }
      }

      Assert.AreEqual (flattenedGraphIDs.Count, _transporter.ObjectCount);
      Assert.That (GetIDList (), Is.EquivalentTo (flattenedGraphIDs), "Recursion stops where filter returns false.");
    }

    [Test]
    public void LoadRecursive_WithFilter_StopRecursion ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Order1,
          delegate (ObjectID id, ref bool continueRecursion)
          {
            // recurse on order items or original objects, include only order items
            continueRecursion = typeof (OrderItem).IsAssignableFrom (id.ClassDefinition.ClassType) || id == DomainObjectIDs.Order1;
            return typeof (OrderItem).IsAssignableFrom (id.ClassDefinition.ClassType);
          });

      List<ObjectID> flattenedGraphIDs;

      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        Order order1 = Order.GetObject (DomainObjectIDs.Order1);
        Set<DomainObject> flattenedGraph = order1.GetFlattenedRelatedObjectGraph ();
        flattenedGraphIDs = new List<ObjectID> ();
        foreach (DomainObject domainObject in flattenedGraph)
        {
          if (domainObject is OrderItem)
            flattenedGraphIDs.Add (domainObject.ID);
        }
      }

      Assert.AreNotEqual (flattenedGraphIDs.Count, _transporter.ObjectCount);
      Assert.AreEqual (3, _transporter.ObjectCount);
      Assert.That (GetIDList (), Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));
    }

    [Test]
    public void LoadRecursive_WithFilter_StopRecursionImmediately ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Order1,
          delegate (ObjectID id, ref bool continueRecursion)
          {
            // recurse on order items, include only order items
            continueRecursion = typeof (OrderItem).IsAssignableFrom (id.ClassDefinition.ClassType);
            return typeof (OrderItem).IsAssignableFrom (id.ClassDefinition.ClassType);
          });

      // recursion stopped immediately, no object was taken
      Assert.AreEqual (0, _transporter.ObjectCount);
    }

    [Test]
    public void LoadRecursive_WithFilter_StopAtOneBranchContinueAtAnother ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Order1,
          delegate (ObjectID id, ref bool continueRecursion)
          {
            // recurse on the original object, ?? , include only order items
            continueRecursion = id == DomainObjectIDs.Order1 || typeof (OrderTicket).IsAssignableFrom (id.ClassDefinition.ClassType);
            return typeof (OrderTicket).IsAssignableFrom (id.ClassDefinition.ClassType);
          });

      // recursion stopped immediately, no object was taken
      Assert.AreEqual (0, _transporter.ObjectCount);
    }*/

    [Test]
    public void GetBinaryTransportData ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      byte[] data = _transporter.GetBinaryTransportData ();
      Assert.IsNotNull (data);
      Assert.IsNotEmpty (data);
    }

    private List<ObjectID> GetIDList ()
    {
      return new List<ObjectID> (_transporter.ObjectIDs);
    }
  }
}