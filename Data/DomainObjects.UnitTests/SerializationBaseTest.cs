using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
public class SerializationBaseTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  protected SerializationBaseTest ()
  {
  }

  // methods and properties

  protected object SerializeAndDeserialize  (object graph)
  {
    using (MemoryStream stream = new MemoryStream ())
    {
      Serialize (stream, graph);
      return Deserialize (stream);
    }
  }

  protected void Serialize (Stream stream, object graph)
  {
    BinaryFormatter serializationFormatter = new BinaryFormatter ();
    serializationFormatter.Serialize (stream, graph);
  }

  protected object Deserialize (Stream stream)
  {
    stream.Position = 0;

    BinaryFormatter deserializationFormatter = new BinaryFormatter ();
    return deserializationFormatter.Deserialize (stream);
  }
}
}
