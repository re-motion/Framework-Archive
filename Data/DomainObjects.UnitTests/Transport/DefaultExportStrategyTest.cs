using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class DefaultExportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Export_SerializesData ()
    {
      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      TransportItem item1 = TransportItem.PackageDataContainer (expectedContainer1);
      TransportItem item2 = TransportItem.PackageDataContainer (expectedContainer2);

      TransportItem[] items = new TransportItem[] { item1, item2 };
      byte[] expectedData = Serializer.Serialize (items);
      byte[] actualData = DefaultExportStrategy.Instance.Export (items);
      Assert.That (actualData, Is.EqualTo (expectedData));
    }
  }
}