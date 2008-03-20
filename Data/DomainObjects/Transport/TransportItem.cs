using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Transport
{
  [Serializable]
  public struct TransportItem : IXmlSerializable
  {
    public static TransportItem PackageDataContainer (DataContainer container)
    {
      ArgumentUtility.CheckNotNull ("container", container);

      TransportItem item = new TransportItem (container.ID);
      foreach (PropertyValue propertyValue in container.PropertyValues)
        item.Properties.Add (propertyValue.Name, propertyValue.Value);
      return item;
    }

    public static IEnumerable<TransportItem> PackageDataContainers (IEnumerable<DataContainer> containers)
    {
      ArgumentUtility.CheckNotNull ("containers", containers);

      foreach (DataContainer container in containers)
        yield return PackageDataContainer (container);
    }

    private ObjectID _id;
    private Dictionary<string, object> _properties;

    public TransportItem (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      _id = id;
      _properties = new Dictionary<string, object>();
    }

    public ObjectID ID
    {
      get { return _id; }
    }

    public Dictionary<string, object> Properties
    {
      get { return _properties; }
    }

    XmlSchema IXmlSerializable.GetSchema ()
    {
      return null;
    }

    void IXmlSerializable.WriteXml (XmlWriter writer)
    {
      writer.WriteAttributeString ("ID", ID.ToString ());

      writer.WriteStartElement ("Properties");
      writer.WriteAttributeString ("Count", Properties.Count.ToString());
      foreach (KeyValuePair<string, object> property in Properties)
        SerializeProperty (writer, property);
      writer.WriteEndElement ();
    }

    void IXmlSerializable.ReadXml (XmlReader reader)
    {
      string idString = reader.GetAttribute ("ID");
      _id = ObjectID.Parse (idString);

      reader.Read ();

      List<KeyValuePair<string, object>> properties = DeserializeProperties (reader);

      _properties = new Dictionary<string, object> ();
      for (int i = 0; i < properties.Count; ++i)
      {
        Properties.Add (properties[i].Key, properties[i].Value);
      }
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
        writer.WriteValue (value);
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

      reader.ReadStartElement ("Property");
      object value;
      if (objectIDAttribute != null)
      {
        string objectIDString = reader.ReadContentAsString ();
        value = ObjectID.Parse (objectIDString);
      }
      else
        value = reader.ReadContentAsObject ();
      reader.ReadEndElement ();
      return new KeyValuePair<string, object> (name, value);
    }
  }
}