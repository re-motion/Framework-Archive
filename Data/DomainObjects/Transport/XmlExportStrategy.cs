using System;
using System.IO;
using System.Xml.Serialization;

namespace Rubicon.Data.DomainObjects.Transport
{
  public sealed class XmlExportStrategy : IExportStrategy
  {
    public static readonly XmlExportStrategy Instance = new XmlExportStrategy();

    public byte[] Export (TransportItem[] transportedObjects)
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        XmlSerializer formatter = new XmlSerializer (typeof (TransportItem[]));
        formatter.Serialize (dataStream, transportedObjects);
        return dataStream.ToArray ();
      }
    }
  }
}