using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.Transport
{
  public sealed class DefaultImportStrategy : IImportStrategy
  {
    public static readonly DefaultImportStrategy Instance = new DefaultImportStrategy ();

    public IEnumerable<TransportItem> Import (byte[] data)
    {
      using (MemoryStream stream = new MemoryStream (data))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        try
        {
          Tuple<ClientTransaction, ObjectID[]> deserializedData = (Tuple<ClientTransaction, ObjectID[]>) formatter.Deserialize (stream);
          DataContainer[] dataContainers = GetTransportedContainers (deserializedData);
          return UnwrapData (dataContainers);
        }
        catch (SerializationException ex)
        {
          throw new TransportationException ("Invalid data specified: " + ex.Message, ex);
        }
      }
    }

    private DataContainer[] GetTransportedContainers (Tuple<ClientTransaction, ObjectID[]> deserializedData)
    {
      DataContainer[] dataContainers = new DataContainer[deserializedData.B.Length];
      for (int i = 0; i < dataContainers.Length; i++)
        dataContainers[i] = deserializedData.A.DataManager.DataContainerMap[deserializedData.B[i]];
      return dataContainers;
    }

    private IEnumerable<TransportItem> UnwrapData (IEnumerable<DataContainer> containers)
    {
      foreach (DataContainer container in containers)
      {
        TransportItem item = TransportItem.PackageDataContainer(container);
        yield return item;
      }
    }
  }
}