namespace Rubicon.Data.DomainObjects.Transport
{
  public interface IExportStrategy
  {
    byte[] Export (TransportItem[] transportedObjects);
  }
}