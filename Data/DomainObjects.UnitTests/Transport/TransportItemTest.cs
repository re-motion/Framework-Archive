using System;
using System.Xml.Serialization;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using System.IO;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Transport
{
  [TestFixture]
  public class TransportItemTest : ClientTransactionBaseTest
  {
    [Test]
    public void Initialization ()
    {
      TransportItem item = new TransportItem(DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, item.ID);
    }

    [Test]
    public void PackageDataContainer ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);

      CheckEqualData(container, item);
    }

    [Test]
    public void PackageDataContainers()
    {
      DataContainer container1 = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      DataContainer container2 = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem[] items = EnumerableUtility.ToArray (TransportItem.PackageDataContainers (new DataContainer[] { container1, container2 }));

      CheckEqualData (container1, items[0]);
      CheckEqualData (container2, items[1]);
    }

    [Test]
    public void Serializable ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      TransportItem deserializedItem = Serializer.SerializeAndDeserialize (item);

      CheckEqualData (container, deserializedItem);
    }

    [Test]
    public void XmlSerializable_ID ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      TransportItem deserializedItem = Serializer.XmlSerializeAndDeserialize (item);
      
      Assert.AreEqual (container.ID, deserializedItem.ID);
    }

    [Test]
    public void XmlSerializable_Properties ()
    {
      DataContainer container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      TransportItem deserializedItem = Serializer.XmlSerializeAndDeserialize (item);

      CheckEqualData (container, deserializedItem);
    }

    [Test]
    public void XmlSerializable_Properties_IntVsString ()
    {
      TransportItem item = new TransportItem(DomainObjectIDs.Order2);
      item.Properties.Add ("Int", 1);
      item.Properties.Add ("String", "1");
      TransportItem deserializedItem = Serializer.XmlSerializeAndDeserialize (item);

      Assert.AreEqual (1, deserializedItem.Properties["Int"]);
      Assert.AreEqual ("1", deserializedItem.Properties["String"]);
    }

    [Test]
    public void XmlSerializable_Multiple ()
    {
      DataContainer container1 = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      DataContainer container2 = Computer.GetObject (DomainObjectIDs.Computer2).InternalDataContainer;
      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      TransportItem[] deserializedItems = Serializer.XmlSerializeAndDeserialize (new TransportItem[] {item1, item2});

      CheckEqualData (container1, deserializedItems[0]);
      CheckEqualData (container2, deserializedItems[1]);
    }

    private void CheckEqualData (DataContainer expectedData, TransportItem item)
    {
      Assert.AreEqual (expectedData.ID, item.ID);
      Assert.AreEqual (expectedData.PropertyValues.Count, item.Properties.Count);
      foreach (PropertyValue propertyValue in expectedData.PropertyValues)
      {
        Assert.IsTrue (item.Properties.ContainsKey (propertyValue.Name));
        Assert.AreEqual (propertyValue.Value, item.Properties[propertyValue.Name]);
      }
    }

  }
}