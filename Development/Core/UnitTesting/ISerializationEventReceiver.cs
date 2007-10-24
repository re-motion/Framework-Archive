using System.Runtime.Serialization;

namespace Rubicon.Development.UnitTesting
{
  public interface ISerializationEventReceiver
  {
    void OnDeserialization (object sender);
    void OnDeserialized (StreamingContext context);
    void OnDeserializing (StreamingContext context);
    void OnSerialized (StreamingContext context);
    void OnSerializing (StreamingContext context);
  }
}