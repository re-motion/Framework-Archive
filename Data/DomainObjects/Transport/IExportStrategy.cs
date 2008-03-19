using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.Transport
{
  public interface IExportStrategy
  {
    byte[] Export (ObjectID[] transportedObjects, ClientTransaction dataTransaction);
  }
}