using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.Transport
{
  public sealed class DefaultImportStrategy : IImportStrategy
  {
    public static readonly DefaultImportStrategy Instance = new DefaultImportStrategy ();

    public DataContainer[] Import (byte[] data)
    {
      using (MemoryStream stream = new MemoryStream (data))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        try
        {
          Tuple<ClientTransaction, ObjectID[]> deserializedData = (Tuple<ClientTransaction, ObjectID[]>) formatter.Deserialize (stream);
          return GetTransportedContainers (deserializedData);
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
  }
}