using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Transport
{
  [Serializable]
  public struct TransportItem
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

    private readonly ObjectID _id;
    private readonly Dictionary<string, object> _properties;

    public TransportItem (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      _id = id;
      _properties = new Dictionary<string, object>();
    }

    internal TransportItem (ObjectID id, Dictionary<string, object> properties)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("properties", properties);

      _id = id;
      _properties = properties;
    }

    public ObjectID ID
    {
      get { return _id; }
    }

    public Dictionary<string, object> Properties
    {
      get { return _properties; }
    }
  }
}