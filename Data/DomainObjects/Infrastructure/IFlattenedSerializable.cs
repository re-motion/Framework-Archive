namespace Rubicon.Data.DomainObjects.Infrastructure
{
  public interface IFlattenedSerializable
  {
    void SerializeIntoFlatStructure (DomainObjectSerializationInfo info);
  }
}