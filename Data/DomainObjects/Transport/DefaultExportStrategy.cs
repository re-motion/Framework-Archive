using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rubicon.Data.DomainObjects.Transport
{
  public sealed class DefaultExportStrategy : IExportStrategy
  {
    public static readonly DefaultExportStrategy Instance = new DefaultExportStrategy();

    public byte[] Export (TransportItem[] transportedItems)
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (dataStream, transportedItems);
        return dataStream.ToArray ();
      }
    }
  }
}