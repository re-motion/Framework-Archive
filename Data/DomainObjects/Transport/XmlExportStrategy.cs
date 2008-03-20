using System;
using System.IO;
using System.Xml.Serialization;

namespace Rubicon.Data.DomainObjects.Transport
{
  public class XmlExportStrategy : IExportStrategy
  {
    public static readonly XmlExportStrategy Instance = new XmlExportStrategy();

    public byte[] Export (TransportItem[] transportedObjects)
    {
      using (MemoryStream dataStream = new MemoryStream ())
      {
        XmlSerializer formatter = new XmlSerializer (typeof (XmlTransportItem[]));
        PerformSerialization(XmlTransportItem.Wrap (transportedObjects), dataStream, formatter);
        return dataStream.ToArray ();
      }
    }

    protected virtual void PerformSerialization (XmlTransportItem[] transportedObjects, MemoryStream dataStream, XmlSerializer formatter)
    {
      formatter.Serialize (dataStream, transportedObjects);
    }
  }
}