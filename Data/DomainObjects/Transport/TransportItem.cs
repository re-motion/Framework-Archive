using System;
using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects.Transport
{
  [Serializable]
  public struct TransportItem
  {
    public static TransportItem PackageDataContainer (DataContainer container)
    {
      TransportItem item = new TransportItem (container.ID);
      foreach (PropertyValue propertyValue in container.PropertyValues)
        item.Properties.Add (propertyValue.Name, propertyValue.Value);
      return item;
    }

    public readonly ObjectID ID;
    public readonly Dictionary<string, object> Properties;


    public TransportItem (ObjectID id)
    {
      ID = id;
      Properties = new Dictionary<string, object>();
    }
  }
}