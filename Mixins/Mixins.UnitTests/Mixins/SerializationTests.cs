using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  [Ignore ("TODO")]
  public class SerializationTests : MixinTestBase
  {
    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);
      Assert.IsTrue (bt1 is ISerializable);

      bt1.I = 25;
      BaseType1 bt1a = (BaseType1) Deserialize (Serialize (bt1));
      Assert.AreNotSame (bt1, bt1a);
      Assert.AreEqual (bt1.I, bt1a.I);

      BaseType2 bt2 = ObjectFactory.Create<BaseType2> ();
      Assert.IsTrue (bt2.GetType ().IsSerializable);
      Assert.IsTrue (bt2 is ISerializable);

      bt2.S = "Bla";
      BaseType2 bt2a = (BaseType2) Deserialize (Serialize (bt2));
      Assert.AreNotSame (bt2, bt2a);
      Assert.AreEqual (bt2.S, bt2a.S);
    }

    private byte[] Serialize (object o)
    {
      using (MemoryStream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, o);
        return stream.GetBuffer ();
      }
    }

    private object Deserialize (byte[] bytes)
    {
      using (MemoryStream stream = new MemoryStream (bytes))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        return formatter.Deserialize (stream);
      }
    }
  }
}
