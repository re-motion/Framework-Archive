using System;
using NUnit.Framework;
using System.Collections.Generic;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Transport;
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
      Assert.AreEqual (0, _transporter.ObjectIDs.Count);
      Assert.IsEmpty (_transporter.ObjectIDs);
    }

    [Test]
    public void Load ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EqualTo (new ObjectID[] { DomainObjectIDs.Order1 }));
    }

    [Test]
    public void Load_Twice ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EqualTo (new ObjectID[] { DomainObjectIDs.Order1 }));
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
      Assert.AreEqual (1, _transporter.ObjectIDs.Count);
      _transporter.Load (DomainObjectIDs.Order2);
      Assert.AreEqual (2, _transporter.ObjectIDs.Count);
      _transporter.Load (DomainObjectIDs.OrderItem1);
      Assert.AreEqual (3, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EqualTo (new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 }));
    }

    [Test]
    public void LoadWithRelatedObjects ()
    {
      _transporter.LoadWithRelatedObjects (DomainObjectIDs.Order1);
      Assert.AreEqual (6, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderTicket1, DomainObjectIDs.Customer1, DomainObjectIDs.Official1 }));
    }

    [Test]
    public void LoadRecursive ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Employee1);
      Assert.AreEqual (5, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee4, DomainObjectIDs.Computer2,
          DomainObjectIDs.Employee5, DomainObjectIDs.Computer3 }));
    }

    [Test]
    public void LoadRecursive_WithStrategy_ShouldFollow ()
    {
      FollowOnlyOneLevelStrategy strategy = new FollowOnlyOneLevelStrategy();
      _transporter.LoadRecursive (DomainObjectIDs.Employee1, strategy);
      Assert.That (_transporter.ObjectIDs, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5 }));
    }

    [Test]
    public void LoadRecursive_WithStrategy_ShouldProcess ()
    {
      OnlyProcessComputersStrategy strategy = new OnlyProcessComputersStrategy ();
      _transporter.LoadRecursive (DomainObjectIDs.Employee1, strategy);
      Assert.That (_transporter.ObjectIDs, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Computer2, DomainObjectIDs.Computer3 }));
    }

    [Test]
    public void LoadTransportData ()
    {
      _transporter.Load (DomainObjectIDs.Employee1);
      _transporter.Load (DomainObjectIDs.Employee2);
      TransportedDomainObjects transportedObjects = DomainObjectTransporter.LoadTransportData (_transporter.GetBinaryTransportData ());
      Assert.IsNotNull (transportedObjects);
      List<DomainObject> domainObjects = new List<DomainObject> (transportedObjects.TransportedObjects);
      Assert.AreEqual (2, domainObjects.Count);
      Assert.That (domainObjects.ConvertAll<ObjectID> (delegate (DomainObject obj) { return obj.ID; }),
          Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Invalid data specified: End of Stream encountered before parsing was completed."
        + "\r\nParameter name: data")]
    public void LoadTransportData_InvalidData ()
    {
      byte[] data = new byte[] { 1, 2, 3 };
      DomainObjectTransporter.LoadTransportData (data);
    }

    [Test]
    public void TransactionContainsMoreObjects_ThanAreTransported ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Employee1, new FollowAllProcessNoneStrategy());
      Assert.AreEqual (0, _transporter.ObjectIDs.Count);
      Assert.IsEmpty (new List<ObjectID> (_transporter.ObjectIDs));

      TransportedDomainObjects transportedObjects = DomainObjectTransporter.LoadTransportData (_transporter.GetBinaryTransportData());
      Assert.IsEmpty (transportedObjects.TransportedObjects);
    }
    
    [Test]
    public void GetBinaryTransportData ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      byte[] data = _transporter.GetBinaryTransportData ();
      Assert.IsNotNull (data);
      Assert.IsNotEmpty (data);
    }
  }
}