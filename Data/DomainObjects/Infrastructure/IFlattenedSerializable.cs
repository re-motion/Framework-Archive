namespace Rubicon.Data.DomainObjects.Infrastructure
{
  public interface IFlattenedSerializable
  {
    // .ctor (FlattenedDeserializationInfo info)
    void SerializeIntoFlatStructure (FlattenedSerializationInfo info);
  }
}