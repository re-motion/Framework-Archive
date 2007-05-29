using System;
using System.Collections.Generic;
using System.Text;
using Mixins.UnitTests.SampleTypes;
using Mixins.Utilities;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using System.Runtime.Serialization;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class MixinSerializationTests : MixinTestBase
  {
    [Test]
    public void SerializationOfMixinThisWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin2), typeof (BT3Mixin2B)).With ();
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (bt3);
      Assert.AreSame (bt3, mixin.This);

      BT3Mixin2B mixin2 = Mixin.Get<BT3Mixin2B> (bt3);
      Assert.AreSame (bt3, mixin2.This);

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin2 mixinA = Mixin.Get<BT3Mixin2> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.AreSame (bt3A, mixinA.This);

      BT3Mixin2B mixin2A = Mixin.Get<BT3Mixin2B> (bt3A);
      Assert.AreNotSame (mixin2, mixin2A);
      Assert.AreSame (bt3A, mixin2A.This);
    }

    [Test]
    public void SerializationOfMixinBaseWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin1), typeof (BT3Mixin1B)).With ();
      BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (mixin.Base);
      Assert.AreSame (bt3.GetType ().GetField ("__first").FieldType, mixin.Base.GetType ());

      BT3Mixin1B mixin2 = Mixin.Get<BT3Mixin1B> (bt3);
      Assert.IsNotNull (mixin2.Base);
      Assert.AreSame (bt3.GetType ().GetField ("__first").FieldType, mixin2.Base.GetType ());

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin1 mixinA = Mixin.Get<BT3Mixin1> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.IsNotNull (mixinA.Base);
      Assert.AreSame (bt3A.GetType ().GetField ("__first").FieldType, mixinA.Base.GetType ());

      BT3Mixin1B mixin2A = Mixin.Get<BT3Mixin1B> (bt3A);
      Assert.AreNotSame (mixin2, mixin2A);
      Assert.IsNotNull (mixin2A.Base);
      Assert.AreSame (bt3A.GetType ().GetField ("__first").FieldType, mixin2A.Base.GetType ());
    }

    [Test]
    public void SerializationOfMixinConfigurationWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin1), typeof (BT3Mixin1B)).With ();
      BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (mixin.Configuration);
      Assert.AreSame (((IMixinTarget) bt3).Configuration.Mixins[typeof (BT3Mixin1)], mixin.Configuration);

      BT3Mixin1B mixin2 = Mixin.Get<BT3Mixin1B> (bt3);
      Assert.IsNotNull (mixin2.Configuration);
      Assert.AreSame (((IMixinTarget) bt3).Configuration.Mixins[typeof (BT3Mixin1B)], mixin2.Configuration);

      Serializer.SerializeAndDeserialize (mixin);

      BaseType3 bt3A = Serializer.SerializeAndDeserialize (bt3);
      BT3Mixin1 mixinA = Mixin.Get<BT3Mixin1> (bt3A);
      Assert.AreNotSame (mixin, mixinA);
      Assert.IsNotNull (mixinA.Configuration);
      Assert.AreSame (((IMixinTarget) bt3A).Configuration.Mixins[typeof (BT3Mixin1)], mixinA.Configuration);

      BT3Mixin1B mixin2A = Mixin.Get<BT3Mixin1B> (bt3A);
      Assert.AreNotSame (mixin2, mixin2A);
      Assert.IsNotNull (mixin2A.Configuration);
      Assert.AreSame (((IMixinTarget) bt3A).Configuration.Mixins[typeof (BT3Mixin1B)], mixin2A.Configuration);
    }

    [Test]
    public void GeneratedTypeHasConfigurationField ()
    {
      ClassOverridingMixinMethod targetInstance = CreateMixedObject<ClassOverridingMixinMethod> (typeof (AbstractMixin)).With ();

      Type t = Mixin.Get<AbstractMixin> (targetInstance).GetType();
      Assert.IsNotNull (t.GetField ("__configuration"));
      Assert.IsTrue (t.GetField ("__configuration").IsStatic);
    }

    [Test]
    public void GeneratedObjectFieldHoldsConfiguration ()
    {
      ClassOverridingMixinMethod targetInstance = CreateMixedObject<ClassOverridingMixinMethod> (typeof (AbstractMixin)).With ();
      AbstractMixin mixin = Mixin.Get<AbstractMixin> (targetInstance);

      Assert.IsNotNull (mixin.GetType ().GetField ("__configuration"));
      Assert.AreSame (((IMixinTarget)targetInstance).Configuration.Mixins[typeof(AbstractMixin)],
          mixin.GetType ().GetField ("__configuration").GetValue (mixin));
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      ClassOverridingMixinMethod targetInstance = CreateMixedObject<ClassOverridingMixinMethod> (typeof (AbstractMixin)).With ();
      AbstractMixin mixin = Mixin.Get<AbstractMixin> (targetInstance);
      Assert.IsTrue (mixin.GetType ().IsSerializable);
      Serializer.Serialize ((object) targetInstance);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      ClassOverridingMixinMethod targetInstance = CreateMixedObject<ClassOverridingMixinMethod> (typeof (AbstractMixin)).With ();
      AbstractMixin mixin = Mixin.Get<AbstractMixin> (targetInstance);

      mixin.I = 13;

      AbstractMixin mixinA = Serializer.SerializeAndDeserialize (mixin);
      Assert.AreEqual (mixin.I, mixinA.I);
      Assert.AreNotSame (mixin, mixinA);
    }

    [Test]
    [Ignore ("TODO: Make caching of configurations by context work correctly")]
    public void GeneratedTypeCorrectlySerializesThisBaseAndConfiguration()
    {
      ClassOverridingMixinMethod targetInstance = CreateMixedObject<ClassOverridingMixinMethod> (typeof (AbstractMixin)).With ();
      AbstractMixin mixin = Mixin.Get<AbstractMixin> (targetInstance);

      Assert.AreEqual (targetInstance, MixinReflector.GetTargetProperty (mixin.GetType()).GetValue (mixin, null));
      Assert.AreEqual (MixinReflector.GetBaseCallProxyType(targetInstance),
          MixinReflector.GetBaseProperty (mixin.GetType ()).GetValue (mixin, null).GetType());
      Assert.AreEqual (((IMixinTarget)targetInstance).Configuration.Mixins[typeof (AbstractMixin)],
          MixinReflector.GetConfigurationProperty (mixin.GetType ()).GetValue (mixin, null));

      ClassOverridingMixinMethod targetInstanceA = Serializer.SerializeAndDeserialize (targetInstance);
      AbstractMixin mixinA = Mixin.Get<AbstractMixin> (targetInstanceA);

      Assert.AreEqual (targetInstanceA, MixinReflector.GetTargetProperty (mixinA.GetType ()).GetValue (mixinA, null));
      Assert.AreEqual (MixinReflector.GetBaseCallProxyType (targetInstanceA),
          MixinReflector.GetBaseProperty (mixinA.GetType ()).GetValue (mixinA, null).GetType ());
      Assert.AreEqual (((IMixinTarget) targetInstanceA).Configuration.Mixins[typeof (AbstractMixin)],
          MixinReflector.GetConfigurationProperty (mixinA.GetType ()).GetValue (mixinA, null));
    }

    [Serializable]
    public abstract class AbstractMixinImplementingISerializable : AbstractMixin, ISerializable
    {
      public AbstractMixinImplementingISerializable ()
      {
      }

      public AbstractMixinImplementingISerializable (SerializationInfo info, StreamingContext context)
      {
        I = info.GetInt32 ("I") + 13;
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        info.AddValue ("I", I + 4);
      }
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassOverridingMixinMethod targetInstance =
          CreateMixedObject<ClassOverridingMixinMethod> (typeof (AbstractMixinImplementingISerializable)).With ();
      AbstractMixinImplementingISerializable mixin = Mixin.Get<AbstractMixinImplementingISerializable> (targetInstance);

      mixin.I = 15;
      Assert.AreEqual (15, mixin.I);

      AbstractMixinImplementingISerializable mixinA = Serializer.SerializeAndDeserialize (mixin);
      Assert.AreEqual (32, mixinA.I);
    }

    public abstract class NotSerializableMixin : AbstractMixin
    {
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "is not marked as serializable", MatchType = MessageMatch.Contains)]
    public void ThrowsIfAbstractMixinTypeNotSerializable()
    {
      ClassOverridingMixinMethod targetInstance =
          CreateMixedObject<ClassOverridingMixinMethod> (typeof (NotSerializableMixin)).With ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    public abstract class NotSerializableMixinWithISerializable : AbstractMixin, ISerializable
    {
      public NotSerializableMixinWithISerializable ()
      {
      }

      public NotSerializableMixinWithISerializable (SerializationInfo info, StreamingContext context)
      {
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
      }
    }

    [Test]
    public void AllowsAbstractMixinTypeNotSerializableWithISerializable ()
    {
      ClassOverridingMixinMethod targetInstance =
          CreateMixedObject<ClassOverridingMixinMethod> (typeof (NotSerializableMixinWithISerializable)).With ();

      Serializer.SerializeAndDeserialize (targetInstance);
    }

    [Test]
    public void SerializationOfGeneratedMixinWorks ()
    {
      ClassOverridingMixinMethod com = CreateMixedObject<ClassOverridingMixinMethod> (typeof (MixinOverridingClassMethod)).With ();
      IMixinOverridingClassMethod comAsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> ((object) com));

      Assert.IsNotNull (comAsIfc);
      Assert.AreEqual ("ClassOverridingMixinMethod.AbstractMethod-25", comAsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));

      ClassOverridingMixinMethod com2 = Serializer.SerializeAndDeserialize (com);
      IMixinOverridingClassMethod com2AsIfc = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (Mixin.Get<MixinOverridingClassMethod> ((object) com2));
      Assert.AreNotSame (Mixin.Get<MixinOverridingClassMethod> ((object) com),
          Mixin.Get<MixinOverridingClassMethod> ((object) com2));

      Assert.IsNotNull (com2AsIfc);
      Assert.AreEqual ("ClassOverridingMixinMethod.AbstractMethod-25", com2AsIfc.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com2.OverridableMethod (13));
    }
  }
}
