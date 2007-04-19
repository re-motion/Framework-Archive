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
  public class SerializationTests : MixinTestBase
  {
    [Serializable]
    [ApplyMixin(typeof(NullMixin))]
    public class ClassImplementingISerializable : ISerializable
    {
      public int I;

      public ClassImplementingISerializable ()
      {
      }

      public ClassImplementingISerializable (SerializationInfo info, StreamingContext context)
      {
        I = 13 + info.GetInt32("I");
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        info.AddValue ("I", I);
      }
    }

    [Serializable]
    [ApplyMixin (typeof (NullMixin))]
    public class ClassWithoutDefaultCtor
    {
      public string S;

      public ClassWithoutDefaultCtor (int i)
      {
        S = i.ToString();
      }
    }

    [Test]
    public void GeneratedTypeHasConfigurationField ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.IsNotNull (t.GetField ("__configuration"));
      Assert.IsTrue (t.GetField ("__configuration").IsStatic);
    }

    [Test]
    public void GeneratedObjectFieldHoldsConfiguration ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();

      Assert.IsNotNull (bt1.GetType ().GetField ("__configuration"));
      Assert.AreSame (Configuration.BaseClasses[typeof (BaseType1)], bt1.GetType ().GetField ("__configuration").GetValue (bt1));
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      Serialize (bt1);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      SerializeAndDeserialize (bt1);
    }

    [Test]
    public void DeserializedMembersFit ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      BaseType1 bt1a = SerializeAndDeserialize (bt1);
      Assert.AreNotSame (bt1, bt1a);
      Assert.AreEqual (bt1.I, bt1a.I);

      BaseType2 bt2 = ObjectFactory.Create<BaseType2> ().With ();
      Assert.IsTrue (bt2.GetType ().IsSerializable);

      bt2.S = "Bla";
      BaseType2 bt2a = SerializeAndDeserialize (bt2);
      Assert.AreNotSame (bt2, bt2a);
      Assert.AreEqual (bt2.S, bt2a.S);
    }

    [Test]
    public void ExtensionsAndConfigurationSerialized ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      IMixinTarget mixinTarget = (IMixinTarget) bt1;

      BaseType1 bt1a = SerializeAndDeserialize (bt1);
      IMixinTarget mixinTargeta = (IMixinTarget) bt1a;

      Assert.IsNotNull (mixinTargeta.Configuration);
      Assert.AreEqual (mixinTarget.Configuration.Type, mixinTargeta.Configuration.Type);

      Assert.IsNotNull (mixinTargeta.Mixins);
      Assert.AreEqual (mixinTarget.Mixins.Length, mixinTargeta.Mixins.Length);
      Assert.AreEqual (mixinTarget.Mixins[0].GetType(), mixinTargeta.Mixins[0].GetType());
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassImplementingISerializable c = ObjectFactory.Create<ClassImplementingISerializable> ().With();
      Assert.AreNotEqual (typeof (ClassImplementingISerializable), c.GetType ());

      c.I = 15;
      Assert.AreEqual (15, c.I);
      
      ClassImplementingISerializable c2 = SerializeAndDeserialize (c);
      Assert.AreNotEqual (c.GetType(), c2.GetType ());
      Assert.AreEqual (28, c2.I);
    }

    [Test]
    public void WorksIfNoDefaultCtor ()
    {
      ClassWithoutDefaultCtor c = ObjectFactory.Create<ClassWithoutDefaultCtor> ().With (35);
      Assert.AreNotEqual (typeof (ClassImplementingISerializable), c.GetType ());

      Assert.AreEqual ("35", c.S);

      ClassWithoutDefaultCtor c2 = SerializeAndDeserialize (c);
      Assert.AreNotEqual (c.GetType (), c2.GetType ());
      Assert.AreEqual ("35", c2.S);
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

    private T SerializeAndDeserialize<T> (T t)
    {
      return (T) Deserialize (Serialize (t));
    }
  }
}
