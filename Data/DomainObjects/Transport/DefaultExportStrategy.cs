using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.Transport
{
  public sealed class DefaultExportStrategy : IExportStrategy
  {
    public static readonly DefaultExportStrategy Instance = new DefaultExportStrategy();

    public byte[] Export (ObjectID[] transportedObjects, ClientTransaction dataTransaction)
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (dataStream, Tuple.NewTuple (dataTransaction, transportedObjects));
        return dataStream.ToArray ();
      }
    }
  }
}