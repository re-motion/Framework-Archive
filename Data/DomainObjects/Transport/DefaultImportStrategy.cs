using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
          TransportItem[] deserializedData = (TransportItem[]) formatter.Deserialize (stream);
          return deserializedData;
        }
        catch (Exception ex)
        {
          throw new TransportationException ("Invalid data specified: " + ex.Message, ex);
        }
      }
    }
  }
}