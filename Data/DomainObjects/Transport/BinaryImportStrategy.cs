using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rubicon.Data.DomainObjects.Transport
{
  public sealed class BinaryImportStrategy : IImportStrategy
  {
    public static readonly BinaryImportStrategy Instance = new BinaryImportStrategy ();

    public IEnumerable<TransportItem> Import (byte[] data)
    {
      using (MemoryStream stream = new MemoryStream (data))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        try
        {
          KeyValuePair<string, Dictionary<string, object>>[] deserializedData = 
              (KeyValuePair<string, Dictionary<string, object>>[]) formatter.Deserialize (stream);
          TransportItem[] transportedObjects = GetTransportItems (deserializedData);
          return transportedObjects;
        }
        catch (Exception ex)
        {
          throw new TransportationException ("Invalid data specified: " + ex.Message, ex);
        }
      }
    }

    private TransportItem[] GetTransportItems (KeyValuePair<string, Dictionary<string, object>>[] deserializedData)
    {
      return Array.ConvertAll<KeyValuePair<string, Dictionary<string, object>>, TransportItem> (deserializedData,
          delegate (KeyValuePair<string, Dictionary<string, object>> pair)
          {
            ObjectID objectID = ObjectID.Parse (pair.Key);
            return new TransportItem (objectID, pair.Value);
          });
    }
  }
}