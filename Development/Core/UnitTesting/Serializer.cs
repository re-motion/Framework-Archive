using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Utilities;

namespace Rubicon.Development.UnitTesting
{
  /// <summary>
  /// Provides quick serialization and deserialization functionality for unit tests.
  /// </summary>
  /// <remarks>The methods of this class use a <see cref="BinaryFormatter"/> for serialization.</remarks>
  public static class Serializer
  {
    public static T SerializeAndDeserialize<T> (T t)
    {
      ArgumentUtility.CheckNotNull ("t", t);
      return (T) Serializer.Deserialize (Serializer.Serialize ((object) t));
    }

    public static byte[] Serialize (object o)
    {
      ArgumentUtility.CheckNotNull ("o", o);

      using (MemoryStream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, o);
        return stream.ToArray();
      }
    }

    public static object Deserialize (byte[] bytes)
    {
      ArgumentUtility.CheckNotNull ("bytes", bytes);

      using (MemoryStream stream = new MemoryStream (bytes))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        return formatter.Deserialize (stream);
      }
    }
  }
}
