using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rubicon.Data.DomainObjects.Transport
{
  public sealed class BinaryExportStrategy : IExportStrategy
  {
    public static readonly BinaryExportStrategy Instance = new BinaryExportStrategy();

    public byte[] Export (TransportItem[] transportedItems)
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        KeyValuePair<string, Dictionary<string, object>>[] versionIndependentItems = GetVersionIndependentItems (transportedItems);
        formatter.Serialize (dataStream, versionIndependentItems);
        return dataStream.ToArray ();
      }
    }

    private KeyValuePair<string, Dictionary<string, object>>[] GetVersionIndependentItems (TransportItem[] transportItems)
    {
      return Array.ConvertAll<TransportItem, KeyValuePair<string, Dictionary<string, object>>> (transportItems, 
          delegate (TransportItem item) { return new KeyValuePair<string, Dictionary<string, object>> (item.ID.ToString (), item.Properties); });
    }
  }
}