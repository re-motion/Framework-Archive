using System;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class XmlImportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Import_DeserializesData ()
    {
      string orderNumberIdentifier = ReflectionUtility.GetPropertyName (typeof (Order), "OrderNumber");

      DataContainer expectedContainer1 = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      DataContainer expectedContainer2 = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;

      byte[] data = Encoding.UTF8.GetBytes (GetXmlString ());

      TransportItem[] items = EnumerableUtility.ToArray (XmlImportStrategy.Instance.Import (data));
      Assert.AreEqual (2, items.Length);

      Assert.AreEqual (expectedContainer1.ID, items[0].ID);
      Assert.AreEqual (expectedContainer1.PropertyValues.Count, items[0].Properties.Count);
      Assert.AreEqual (expectedContainer1.PropertyValues[orderNumberIdentifier].Value, items[0].Properties[orderNumberIdentifier]);

      Assert.AreEqual (expectedContainer2.ID, items[1].ID);
      Assert.AreEqual (expectedContainer2.PropertyValues.Count, items[1].Properties.Count);
      Assert.AreEqual (expectedContainer2.PropertyValues[orderNumberIdentifier].Value, items[1].Properties[orderNumberIdentifier]);
    }

    private string GetXmlString ()
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

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: There is an error in XML document (0, 0).")]
    public void Import_ThrowsOnInvalidFormat ()
    {
      byte[] data = new byte[0];
      XmlImportStrategy.Instance.Import (data);
    }

    [Test]
    public void Import_ExportStrategy_IntegrationTest ()
    {
      TransportItem item1 = new TransportItem (DomainObjectIDs.Order1);
      item1.Properties.Add ("Foo", 12);
      TransportItem item2 = new TransportItem (DomainObjectIDs.Order2);
      item2.Properties.Add ("Bar", "42");

      byte[] package = XmlExportStrategy.Instance.Export (new TransportItem[] { item1, item2 });
      TransportItem[] importedItems = EnumerableUtility.ToArray (XmlImportStrategy.Instance.Import (package));

      Assert.AreEqual (2, importedItems.Length);
      Assert.AreEqual (item1.ID, importedItems[0].ID);
      Assert.AreEqual (1, importedItems[0].Properties.Count);
      Assert.AreEqual (item1.Properties["Foo"], importedItems[0].Properties["Foo"]);

      Assert.AreEqual (item2.ID, importedItems[1].ID);
      Assert.AreEqual (1, importedItems[1].Properties.Count);
      Assert.AreEqual (item2.Properties["Bar"], importedItems[1].Properties["Bar"]);
    }
  }
}