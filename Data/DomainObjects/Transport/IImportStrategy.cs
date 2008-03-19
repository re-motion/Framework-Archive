using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects.Transport
{
  public interface IImportStrategy
  {
    IEnumerable<TransportItem> Import (byte[] data);
  }
}