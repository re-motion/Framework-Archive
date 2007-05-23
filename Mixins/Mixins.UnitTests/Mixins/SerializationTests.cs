using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Mixins.CodeGeneration;
using Mixins.Definitions;
using Mixins.UnitTests.Mixins;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class SerializationTests : MixinTestBase
  {
    [Serializable]
    [Uses(typeof(NullMixin))]
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
    [Uses (typeof (NullMixin))]
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
      Type t = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      Assert.IsNotNull (t.GetField ("__configuration"));
      Assert.IsTrue (t.GetField ("__configuration").IsStatic);
    }

    [Test]
    public void GeneratedObjectFieldHoldsConfiguration ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();

      Assert.IsNotNull (bt1.GetType ().GetField ("__configuration"));
      Assert.AreSame (TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType1)], bt1.GetType ().GetField ("__configuration").GetValue (bt1));
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      Serializer.Serialize ((object) bt1);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      Serializer.SerializeAndDeserialize (bt1);
    }

    [Test]
    public void DeserializedMembersFit ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsSerializable);

      bt1.I = 25;
      BaseType1 bt1a = Serializer.SerializeAndDeserialize (bt1);
      Assert.AreNotSame (bt1, bt1a);
      Assert.AreEqual (bt1.I, bt1a.I);

      BaseType2 bt2 = CreateMixedObject<BaseType2> (typeof(BT2Mixin1)).With();
      Assert.IsTrue (bt2.GetType ().IsSerializable);

      bt2.S = "Bla";
      BaseType2 bt2a = Serializer.SerializeAndDeserialize (bt2);
      Assert.AreNotSame (bt2, bt2a);
      Assert.AreEqual (bt2.S, bt2a.S);
    }

    [Test]
    public void ExtensionsAndConfigurationSerializedFirstFits ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      IMixinTarget mixinTarget = (IMixinTarget) bt1;

      BaseType1 bt1a = Serializer.SerializeAndDeserialize (bt1);
      IMixinTarget mixinTargetA = (IMixinTarget) bt1a;

      Assert.IsNotNull (mixinTargetA.Configuration);
      Assert.AreEqual (mixinTarget.Configuration.Type, mixinTargetA.Configuration.Type);

      Assert.IsNotNull (mixinTargetA.Mixins);
      Assert.AreEqual (mixinTarget.Mixins.Length, mixinTargetA.Mixins.Length);
      Assert.AreEqual (mixinTarget.Mixins[0].GetType(), mixinTargetA.Mixins[0].GetType());

      Assert.IsNotNull (mixinTargetA.FirstBaseCallProxy);
      Assert.AreNotEqual (mixinTarget.FirstBaseCallProxy.GetType (), mixinTargetA.FirstBaseCallProxy.GetType ());
      Assert.AreEqual (mixinTargetA.GetType ().GetNestedType ("BaseCallProxy"), mixinTargetA.FirstBaseCallProxy.GetType ());
      Assert.AreEqual (0, mixinTargetA.FirstBaseCallProxy.GetType ().GetField ("__depth").GetValue (mixinTargetA.FirstBaseCallProxy));
      Assert.AreSame (mixinTargetA, mixinTargetA.FirstBaseCallProxy.GetType ().GetField ("__this").GetValue(mixinTargetA.FirstBaseCallProxy));
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassImplementingISerializable c = ObjectFactory.Create<ClassImplementingISerializable> ().With();
      Assert.AreNotEqual (typeof (ClassImplementingISerializable), c.GetType ());

      c.I = 15;
      Assert.AreEqual (15, c.I);
      
      ClassImplementingISerializable c2 = Serializer.SerializeAndDeserialize (c);
      Assert.AreNotEqual (c.GetType(), c2.GetType ());
      Assert.AreEqual (28, c2.I);
    }

    [Test]
    public void WorksIfNoDefaultCtor ()
    {
      ClassWithoutDefaultCtor c = ObjectFactory.Create<ClassWithoutDefaultCtor> ().With (35);
      Assert.AreNotEqual (typeof (ClassImplementingISerializable), c.GetType ());

      Assert.AreEqual ("35", c.S);

      ClassWithoutDefaultCtor c2 = Serializer.SerializeAndDeserialize (c);
      Assert.AreNotEqual (c.GetType (), c2.GetType ());
      Assert.AreEqual ("35", c2.S);
    }

    [Test]
    public void ConfigSerializationSpike()
    {
      ApplicationDefinition def = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin2));
      Serializer.SerializeAndDeserialize (def);
    }

    [Test]
    public void SerializationOfMixinThisWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin2)).With();
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (bt3);
      Assert.AreSame (bt3, mixin.This);

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin2 mixinA = Mixin.Get<BT3Mixin2> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.AreSame (bt3A, mixinA.This);
    }

    [Test]
    [Ignore ("TODO: Implement serialization for generated mixin classes")]
    public void SerializationOfMixinBaseWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin1)).With ();
      BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (mixin.Base);
      Assert.AreSame (bt3.GetType().GetField ("__first").FieldType, mixin.Base.GetType());

      Serializer.SerializeAndDeserialize (mixin);

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin1 mixinA = Mixin.Get<BT3Mixin1> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.IsNotNull (mixinA.Base);
      Assert.AreSame (bt3A.GetType ().GetField ("__first").FieldType, mixinA.Base.GetType ());
    }

    [Test]
    [Ignore ("TODO: Implement serialization for generated mixin classes")]
    public void SerializationOfMixinConfigurationWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin1)).With ();
      BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (mixin.Configuration);
      Assert.AreSame (((IMixinTarget) bt3).Configuration.Mixins[typeof(BT3Mixin1)], mixin.Configuration);

      Serializer.SerializeAndDeserialize (mixin);

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin1 mixinA = Mixin.Get<BT3Mixin1> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.IsNotNull (mixinA.Configuration);
      Assert.AreSame (((IMixinTarget) bt3A).Configuration.Mixins[typeof (BT3Mixin1)], mixinA.Configuration);
    }

    [Test]
    [Ignore ("TODO: Implement serialization for generated mixin classes")]
    public void SerializationOfDerivedMixinWorks ()
    {
      Assert.Fail ();
      ClassOverridingMixinMethod com = CreateMixedObject<ClassOverridingMixinMethod> (typeof (MixinOverridingClassMethod)).With ();
      IMixinOverridingClassMethod comAsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> ((object) com));

      Assert.IsNotNull (comAsIfc);
      Assert.AreEqual ("ClassOverridingMixinMethod.AbstractMethod-25", comAsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));

      ClassOverridingMixinMethod com2 = Serializer.SerializeAndDeserialize (com);
      IMixinOverridingClassMethod com2AsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> ((object) com2));
      Assert.AreNotSame(Mixin.Get<MixinOverridingClassMethod> ((object) com),
          Mixin.Get<MixinOverridingClassMethod> ((object) com2));

      Assert.IsNotNull (com2AsIfc);
      Assert.AreEqual ("ClassOverridingMixinMethod.AbstractMethod-25", com2AsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com2.OverridableMethod (13));
    }
  }
}

namespace Mixins
{
  namespace UnitTests
  {
    class Serializer
    {
      public static T SerializeAndDeserialize<T> (T t)
      {
        return (T) Serializer.Deserialize (Serializer.Serialize ((object) t));
      }

      public static byte[] Serialize (object o)
      {
        using (MemoryStream stream = new MemoryStream ())
        {
          BinaryFormatter formatter = new BinaryFormatter ();
          formatter.Serialize (stream, o);
          return stream.GetBuffer ();
        }
      }

      public static object Deserialize (byte[] bytes)
      {
        using (MemoryStream stream = new MemoryStream (bytes))
        {
          BinaryFormatter formatter = new BinaryFormatter ();
          return formatter.Deserialize (stream);
        }
      }
    }
  }
}
