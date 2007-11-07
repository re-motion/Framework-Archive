using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;

using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;

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

    [Test]
    public void FinishTransport_CallsCommit ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionExtension mock = mockRepository.CreateMock<IClientTransactionExtension> ();

      mock.Committing (null, null);
      LastCall.Constraints (Mocks_Is.Same (transportedObjects.DataTransaction), Mocks_List.Equal (GetTransportedObjects (transportedObjects)));
      mock.Committed (null, null);
      LastCall.Constraints (Mocks_Is.Same (transportedObjects.DataTransaction), Mocks_List.Equal (GetTransportedObjects (transportedObjects)));

      mockRepository.ReplayAll ();

      transportedObjects.DataTransaction.Extensions.Add ("mock", mock);
      transportedObjects.FinishTransport ();

      mockRepository.VerifyAll ();
    }

    [Test]
    public void FinishTransport_FilterCalledForEachChangedObject ()
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      transporter.Load (DomainObjectIDs.ClassWithAllDataTypes1);
      transporter.Load (DomainObjectIDs.ClassWithAllDataTypes2);
      transporter.Load (DomainObjectIDs.Order1);

      ModifyDatabase (delegate
      {
        ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
        c1.Delete ();
        c2.Delete ();
      });

      byte[] transportData = transporter.GetBinaryTransportData ();

      TransportedDomainObjects transportedObjects = DomainObjectTransporter.LoadTransportData (transportData);
      List<DomainObject> expectedObjects = new List<DomainObject> ();
      using (transportedObjects.DataTransaction.EnterNonDiscardingScope ())
      {
        expectedObjects.Add (ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1));
        expectedObjects.Add (ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2));
      }

      List<DomainObject> filteredObjects = new List<DomainObject> ();
      transportedObjects.FinishTransport (delegate (DomainObject domainObject) { filteredObjects.Add (domainObject); return true; });
      Assert.That (filteredObjects, Is.EquivalentTo (expectedObjects));
    }

    [Test]
    public void FinishTransport_All ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport ();

      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        ClassWithAllDataTypes c3 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ClassWithAllDataTypes c4 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);

        Assert.AreEqual (2147483647, c3.Int32Property);
        Assert.AreEqual (-2147483647, c4.Int32Property);
      }
    }

    [Test]
    public void FinishTransport_Some ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate (DomainObject transportedObject)
      {
        return ((ClassWithAllDataTypes) transportedObject).Int32Property < 0;
      });

      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        try
        {
          ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
          Assert.Fail ("Expected ObjectNotFoundException");
        }
        catch (ObjectNotFoundException)
        {
          // ok
        }

        ClassWithAllDataTypes c4 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
        Assert.AreEqual (-2147483647, c4.Int32Property);
      }
    }

    [Test]
    public void FinishTransport_ClearsTransportedObjects ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate { return false; });
      Assert.IsNull (transportedObjects.DataTransaction);
      Assert.IsNull (transportedObjects.TransportedObjects);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "FinishTransport can only be called once.")]
    public void FinishTransport_Twice ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate { return false; });
      transportedObjects.FinishTransport ();
    }

    private TransportedDomainObjects TransportAndDeleteObjects (params ObjectID[] objectsToLoadAndDelete)
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      foreach (ObjectID id in objectsToLoadAndDelete)
        transporter.Load (id);

      ModifyDatabase (delegate
      {
        ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);

        Assert.AreEqual (2147483647, c1.Int32Property);
        Assert.AreEqual (-2147483647, c2.Int32Property);

        c1.Delete ();
        c2.Delete ();
      });

      byte[] transportData = transporter.GetBinaryTransportData ();

      return DomainObjectTransporter.LoadTransportData (transportData);
    }

    private List<DomainObject> GetTransportedObjects (TransportedDomainObjects transportedObjects)
    {
      return new List<DomainObject> (transportedObjects.TransportedObjects);
    }

    private void ModifyDatabase (Proc changer)
    {
      SetDatabaseModifyable ();
      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        changer ();
        ClientTransaction.Current.Commit ();
      }
    }
  }
}