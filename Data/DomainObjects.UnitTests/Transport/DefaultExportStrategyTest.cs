using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class DefaultExportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Export_Serialized ()
    {
      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] expectedData = Serialize (expectedContainer1.ID, expectedContainer2.ID);
      byte[] actualData = DefaultExportStrategy.Instance.Export (new ObjectID[] { expectedContainer1.ID, expectedContainer2.ID }, ClientTransactionMock);
      Assert.That (actualData, Is.EqualTo (expectedData));
    }

    private byte[] Serialize (params ObjectID[] loadedIDs)
    {
      byte[] data;
      using (MemoryStream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, new Tuple<ClientTransaction, ObjectID[]> (ClientTransactionMock, loadedIDs));
        data = stream.ToArray ();
      }
      return data;
    }
  }
}