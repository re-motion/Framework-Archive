using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Transport
{
  public struct XmlTransportItem : IXmlSerializable
  {
    public static XmlTransportItem[] Wrap (TransportItem[] items)
    {
      ArgumentUtility.CheckNotNull ("items", items);
      return Array.ConvertAll<TransportItem, XmlTransportItem> (items, delegate (TransportItem item) { return new XmlTransportItem (item); });
    }

    public static TransportItem[] Unwrap (XmlTransportItem[] xmlItems)
    {
      ArgumentUtility.CheckNotNull ("xmlItems", xmlItems);
      return Array.ConvertAll<XmlTransportItem, TransportItem> (xmlItems, delegate (XmlTransportItem item) { return item.TransportItem; });
    }

    private TransportItem _transportItem;

    public XmlTransportItem (TransportItem itemToBeSerialized)
    {
      _transportItem = itemToBeSerialized;
    }

    public TransportItem TransportItem
    {
      get { return _transportItem; }
    }

    public void WriteXml (XmlWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.WriteAttributeString ("ID", _transportItem.ID.ToString ());

      writer.WriteStartElement ("Properties");
      writer.WriteAttributeString ("Count", _transportItem.Properties.Count.ToString ());
      foreach (KeyValuePair<string, object> property in _transportItem.Properties)
        SerializeProperty (writer, property);
      writer.WriteEndElement ();
    }

    public XmlSchema GetSchema ()
    {
      return null;
    }

    public void ReadXml (XmlReader reader)
    {
      ArgumentUtility.CheckNotNull ("reader", reader);

      string idString = reader.GetAttribute ("ID");
      ObjectID id = ObjectID.Parse (idString);

      reader.Read ();
      List<KeyValuePair<string, object>> properties = DeserializeProperties (reader);
      reader.ReadEndElement ();

      _transportItem = CreateTransportItem(id, properties);
    }

    private TransportItem CreateTransportItem (ObjectID id, List<KeyValuePair<string, object>> properties)
    {
      Dictionary<string, object> propertyDictionary = new Dictionary<string, object> ();
      for (int i = 0; i < properties.Count; ++i)
      {
        propertyDictionary.Add (properties[i].Key, properties[i].Value);
      }
      return new TransportItem (id, propertyDictionary);
    }

    private void SerializeProperty (XmlWriter writer, KeyValuePair<string, object> property)
    {
      writer.WriteStartElement ("Property");
      writer.WriteAttributeString ("Name", property.Key);
      SerializePropertyValue (writer, property.Value);
      writer.WriteEndElement ();
    }

    private void SerializePropertyValue (XmlWriter writer, object value)
    {
      ObjectID objectID;
      if ((objectID = value as ObjectID) != null)
      {
        writer.WriteAttributeString ("ObjectID", "true");
        writer.WriteString (objectID.ToString ());
      }
      else
      {
        Type valueType = value.GetType ();
        writer.WriteAttributeString ("Type", valueType.AssemblyQualifiedName);
        XmlSerializer valueSerializer = new XmlSerializer (valueType);
        valueSerializer.Serialize (writer, value);
      }
    }

    private List<KeyValuePair<string, object>> DeserializeProperties (XmlReader reader)
    {
      int propertyCount = int.Parse (reader.GetAttribute ("Count"));
      reader.ReadStartElement ("Properties");
      List<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>> (propertyCount);
      for (int i = 0; i < propertyCount; ++i)
        properties.Add (DeserializeProperty (reader));
      reader.ReadEndElement ();
      return properties;
    }

    private KeyValuePair<string, object> DeserializeProperty (XmlReader reader)
    {
      string name = reader.GetAttribute ("Name");
      string objectIDAttribute = reader.GetAttribute ("ObjectID");
      string valueTypeName = reader.GetAttribute ("Type");

      reader.ReadStartElement ("Property");
      object value;
      if (objectIDAttribute != null)
      {
        string objectIDString = reader.ReadContentAsString ();
        value = ObjectID.Parse (objectIDString);
      }
      else
      {
        Type valueType = Type.GetType (valueTypeName, true, false);
        XmlSerializer deserializer = new XmlSerializer (valueType);
        value = deserializer.Deserialize (reader);
      }
      reader.ReadEndElement ();
      return new KeyValuePair<string, object> (name, value);
    }
  }
}