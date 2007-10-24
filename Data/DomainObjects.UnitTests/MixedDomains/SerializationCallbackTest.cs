using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class SerializationCallbackTest : ClientTransactionBaseTest
  {
    [Serializable]
    public class MixinWithSerializationCallbacks : ClassWithSerializationCallbacksBase
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }
    }

    [Test]
    public void SerializationEvents_OnTarget ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassWithSerializationCallbacks), typeof (MixinWithSerializationCallbacks)))
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) DomainObject.NewObject (typeof (ClassWithSerializationCallbacks));

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (instance, ClassWithSerializationCallbacks.SetReceiver)
            .Test_SerializationCallbacks();
      }
    }

    [Test]
    public void DeserializationEvents_OnTarget ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassWithSerializationCallbacks), typeof (MixinWithSerializationCallbacks)))
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) DomainObject.NewObject (typeof (ClassWithSerializationCallbacks));

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (instance, ClassWithSerializationCallbacks.SetReceiver)
            .Test_DeserializationCallbacks();
      }
    }

    [Test]
    public void SerializationEvents_OnMixin ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassWithSerializationCallbacks), typeof (MixinWithSerializationCallbacks)))
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) DomainObject.NewObject (typeof (ClassWithSerializationCallbacks));

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (instance, MixinWithSerializationCallbacks.SetReceiver)
            .Test_SerializationCallbacks ();
      }
    }

    [Test]
    public void DeserializationEvents_OnMixin ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassWithSerializationCallbacks), typeof (MixinWithSerializationCallbacks)))
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) DomainObject.NewObject (typeof (ClassWithSerializationCallbacks));

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (instance, MixinWithSerializationCallbacks.SetReceiver)
            .Test_DeserializationCallbacks ();
      }
    }
  }
}