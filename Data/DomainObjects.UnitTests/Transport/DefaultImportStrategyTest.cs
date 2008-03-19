using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class DefaultImportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Import_DeserializesData ()
    {
      string orderNumberIdentifier = ReflectionUtility.GetPropertyName (typeof (Order), "OrderNumber");

      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] data = Serialize(expectedContainer1.ID, expectedContainer2.ID);
      TransportItem[] items = EnumerableUtility.ToArray (DefaultImportStrategy.Instance.Import (data));
      Assert.AreEqual (2, items.Length);

      Assert.AreEqual (expectedContainer1.ID, items[0].ID);
      Assert.AreEqual (expectedContainer1.PropertyValues.Count, items[0].Properties.Count);
      Assert.AreEqual (expectedContainer1.PropertyValues[orderNumberIdentifier].Value, items[0].Properties[orderNumberIdentifier]);

      Assert.AreEqual (expectedContainer2.ID, items[1].ID);
      Assert.AreEqual (expectedContainer2.PropertyValues.Count, items[1].Properties.Count);
      Assert.AreEqual (expectedContainer2.PropertyValues[orderNumberIdentifier].Value, items[1].Properties[orderNumberIdentifier]);
    }

    [Test]
    public void Import_OnlyImportsLoadedObjects ()
    {
      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      Dev.Null = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] data = Serialize (expectedContainer1.ID );
      TransportItem[] containers = EnumerableUtility.ToArray (DefaultImportStrategy.Instance.Import (data));
      Assert.AreEqual (1, containers.Length);
      Assert.AreEqual (expectedContainer1.ID, containers[0].ID);
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: Attempting to deserialize an empty stream.")]
    public void Import_ThrowsOnInvalidFormat ()
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