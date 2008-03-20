using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class XmlExportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Export_SerializesData ()
    {
      DataContainer container1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer container2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      TransportItem[] items = new TransportItem[] { item1, item2 };
      byte[] actualData = XmlExportStrategy.Instance.Export (items);
      string actualString = Encoding.UTF8.GetString (actualData);
      Assert.AreEqual (GetExpectedString(), actualString);
    }

    private string GetExpectedString ()
    {
      return @"<?xml version=""1.0""?>
<ArrayOfTransportItem xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <TransportItem ID=""Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid"">
    <Properties Count=""4"">
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"" Type=""System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">
        <int>1</int>
      </Property>
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"" Type=""System.DateTime, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">
        <dateTime>2005-01-01T00:00:00</dateTime>
      </Property>
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"" ObjectID=""true"">Official|1|System.Int32</Property>
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"" ObjectID=""true"">Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid</Property>
    </Properties>
  </TransportItem>
  <TransportItem ID=""Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid"">
    <Properties Count=""4"">
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"" Type=""System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">
        <int>3</int>
      </Property>
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"" Type=""System.DateTime, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">
        <dateTime>2005-03-01T00:00:00</dateTime>
      </Property>
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"" ObjectID=""true"">Official|1|System.Int32</Property>
      <Property Name=""Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"" ObjectID=""true"">Customer|dd3e3d55-c16f-497f-a3e1-384d08de0d66|System.Guid</Property>
    </Properties>
  </TransportItem>
</ArrayOfTransportItem>";
    }
  }
}