using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Rubicon.Data.DomainObjects.Transport
{
  public class XmlImportStrategy : IImportStrategy
  {
    public static readonly XmlImportStrategy Instance = new XmlImportStrategy();

    public IEnumerable<TransportItem> Import (byte[] data)
    {
      try
      {
        using (MemoryStream dataStream = new MemoryStream (data))
        {
          XmlSerializer formatter = new XmlSerializer (typeof (TransportItem[]));
          return PerformDeserialization(dataStream, formatter);
        }
      }
      catch (Exception ex)
      {
        throw new TransportationException ("Invalid data specified: " + ex.Message, ex);
      }
    }

    protected virtual IEnumerable<TransportItem> PerformDeserialization (MemoryStream dataStream, XmlSerializer formatter)
    {
      return (TransportItem[]) formatter.Deserialize (dataStream);
    }
  }
}