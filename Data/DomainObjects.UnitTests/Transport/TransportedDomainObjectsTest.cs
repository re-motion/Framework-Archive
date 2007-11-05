using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class TransportedDomainObjectsTest : StandardMappingTest
  {
    [Test]
    public void EmptyTransport ()
    {
      ClientTransaction dataTransaction = ClientTransaction.NewTransaction ();
      TransportedDomainObjects transportedObjects = new TransportedDomainObjects (dataTransaction, new List<DomainObject>());

      Assert.IsNotNull (transportedObjects.DataTransaction);
      Assert.AreSame (dataTransaction, transportedObjects.DataTransaction);
      Assert.IsEmpty (GetTransportedObjects (transportedObjects));
    }

    [Test]
    public void TransportedObjectsStayConstant_WhenTransactionIsManipulated ()
    {
      TransportedDomainObjects transportedObjects = new TransportedDomainObjects (ClientTransaction.NewTransaction (), new List<DomainObject> ());

      Assert.IsEmpty (GetTransportedObjects (transportedObjects));

      using (transportedObjects.DataTransaction.EnterNonDiscardingScope ())
      {
        Order.GetObject (DomainObjectIDs.Order1);
      }

      Assert.IsEmpty (GetTransportedObjects (transportedObjects));
    }

    [Test]
    public void NonEmptyTransport ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();
      List<DomainObject> transportedObjectList = new List<DomainObject>  ();
      using (newTransaction.EnterNonDiscardingScope ())
      {
        transportedObjectList.Add (Order.GetObject (DomainObjectIDs.Order1));
        transportedObjectList.Add (Order.GetObject (DomainObjectIDs.Order2));
        transportedObjectList.Add (Company.GetObject (DomainObjectIDs.Company1));
      }

      TransportedDomainObjects transportedObjects = new TransportedDomainObjects (newTransaction, transportedObjectList);

      Assert.IsNotNull (transportedObjects.DataTransaction);
      Assert.IsNotEmpty (GetTransportedObjects (transportedObjects));
      List<ObjectID> ids = GetTransportedObjects (transportedObjects).ConvertAll<ObjectID> (delegate (DomainObject obj) { return obj.ID; });
      Assert.That (ids, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Company1 }));
    }

    private List<DomainObject> GetTransportedObjects (TransportedDomainObjects transportedObjects)
    {
      return new List<DomainObject> (transportedObjects.TransportedObjects);
    }
  }
}