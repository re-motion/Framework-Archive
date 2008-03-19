namespace Rubicon.Data.DomainObjects.Transport
{
  public interface IImportStrategy
  {
    DataContainer[] Import (byte[] data);
  }
}