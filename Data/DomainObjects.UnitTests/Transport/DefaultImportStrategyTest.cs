using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class DefaultImportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Import_DeserializesData ()
    {
      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] data = Serialize(expectedContainer1.ID, expectedContainer2.ID);
      DataContainer[] containers = DefaultImportStrategy.Instance.Import (data);
      Assert.AreEqual (2, containers.Length);
      Assert.AreEqual (expectedContainer1.ID, containers[0].ID);
      Assert.AreEqual (expectedContainer2.ID, containers[1].ID);
    }

    [Test]
    public void Import_OnlyImportsLoadedObjects ()
    {
      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      Dev.Null = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] data = Serialize (expectedContainer1.ID );
      DataContainer[] containers = DefaultImportStrategy.Instance.Import (data);
      Assert.AreEqual (1, containers.Length);
      Assert.AreEqual (expectedContainer1.ID, containers[0].ID);
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: Attempting to deserialize an empty stream.")]
    public void Import_InvalidFormat ()
    {
      byte[] data = new byte[0];
      DefaultImportStrategy.Instance.Import (data);
    }

    private byte[] Serialize (params ObjectID[] loadedIDs)
    {
      byte[] data;
      using (MemoryStream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, new Tuple<ClientTransaction, ObjectID[]> (ClientTransactionMock,
            loadedIDs));
        data = stream.ToArray ();
      }
      return data;
    }
  }
}