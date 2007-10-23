using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class SerializationCallbackTests
  {
    private MockRepository _mockRepository;
    private ISerializationEventReceiver _receiver;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _receiver = _mockRepository.CreateMock<ISerializationEventReceiver> ();

      TargetTypeWithSerializationCallbacks.StaticReceiver = null;
      MixinWithSerializationCallbacks.StaticReceiver = null;
    }

    public interface ISerializationEventReceiver
    {
      void OnDeserialization (object sender);
      void OnDeserialized (StreamingContext context);
      void OnDeserializing (StreamingContext context);
      void OnSerialized (StreamingContext context);
      void OnSerializing (StreamingContext context);
    }

    [Serializable]
    public abstract class SerializationCallbackBase : IDeserializationCallback
    {
      protected abstract ISerializationEventReceiver Receiver { get; }

      public void OnDeserialization (object sender)
      {
        if (Receiver != null)
          Receiver.OnDeserialization (sender);
      }

      [OnDeserialized]
      public void OnDeserialized (StreamingContext context)
      {
        if (Receiver != null)
          Receiver.OnDeserialized (context);
      }

      [OnDeserializing]
      public void OnDeserializing (StreamingContext context)
      {
        if (Receiver != null)
          Receiver.OnDeserializing (context);
      }

      [OnSerialized]
      public void OnSerialized (StreamingContext context)
      {
        if (Receiver != null)
          Receiver.OnSerialized (context);
      }

      [OnSerializing]
      public void OnSerializing (StreamingContext context)
      {
        if (Receiver != null)
          Receiver.OnSerializing (context);
      }
    }

    [Serializable]
    [Uses (typeof (MixinWithSerializationCallbacks))]
    public class TargetTypeWithSerializationCallbacks : SerializationCallbackBase
    {
      public static ISerializationEventReceiver StaticReceiver;

      protected override ISerializationEventReceiver Receiver
      {
        get { return StaticReceiver; }
      }
    }

    [Serializable]
    public class MixinWithSerializationCallbacks : SerializationCallbackBase
    {
      public static ISerializationEventReceiver StaticReceiver;

      protected override ISerializationEventReceiver Receiver
      {
        get { return StaticReceiver; }
      }
    }

    [Test]
    public void SerializationCallbacks_ViaFormatter ()
    {
      TargetTypeWithSerializationCallbacks instance = new TargetTypeWithSerializationCallbacks ();
      TargetTypeWithSerializationCallbacks.StaticReceiver = _receiver;

      CheckSerialization(instance);
    }

    [Test]
    public void DeserializationCallbacks_ViaFormatter ()
    {
      TargetTypeWithSerializationCallbacks instance = new TargetTypeWithSerializationCallbacks();
      byte[] bytes = Serializer.Serialize (instance);
      TargetTypeWithSerializationCallbacks.StaticReceiver = _receiver;

      CheckDeserialization(bytes);
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnTargetClass ()
    {
      TargetTypeWithSerializationCallbacks instance = ObjectFactory.Create<TargetTypeWithSerializationCallbacks> ().With ();
      TargetTypeWithSerializationCallbacks.StaticReceiver = _receiver;

      CheckSerialization (instance);
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnTargetClass ()
    {
      TargetTypeWithSerializationCallbacks instance = ObjectFactory.Create<TargetTypeWithSerializationCallbacks> ().With ();
      byte[] bytes = Serializer.Serialize (instance);
      TargetTypeWithSerializationCallbacks.StaticReceiver = _receiver;

      CheckDeserialization (bytes);
    }

    [Test]
    public void SerializationCallbacks_AreInvokedOnMixinClass ()
    {
      TargetTypeWithSerializationCallbacks instance = ObjectFactory.Create<TargetTypeWithSerializationCallbacks> ().With ();
      MixinWithSerializationCallbacks.StaticReceiver = _receiver;

      CheckSerialization (instance);
    }

    [Test]
    public void DeserializationCallbacks_AreInvokedOnMixinClass ()
    {
      TargetTypeWithSerializationCallbacks instance = ObjectFactory.Create<TargetTypeWithSerializationCallbacks> ().With ();
      byte[] bytes = Serializer.Serialize (instance);
      MixinWithSerializationCallbacks.StaticReceiver = _receiver;

      CheckDeserialization (bytes);
 }

    private void CheckSerialization (TargetTypeWithSerializationCallbacks instance)
    {
      ExpectSerializationCallbacks ();

      _mockRepository.ReplayAll ();

      Serializer.Serialize (instance);

      _mockRepository.VerifyAll ();
    }

    private void CheckDeserialization (byte[] bytes)
    {
      ExpectDeserializationCallbacks ();

      _mockRepository.ReplayAll ();

      Serializer.Deserialize (bytes);

      _mockRepository.VerifyAll ();
    }

    private void ExpectSerializationCallbacks ()
    {
      using (_mockRepository.Ordered ())
      {
        StreamingContext context = new StreamingContext ();
        _receiver.OnSerializing (context);
        LastCall.IgnoreArguments ();

        _receiver.OnSerialized (context);
        LastCall.IgnoreArguments ();
      }
    }

    private void ExpectDeserializationCallbacks ()
    {
      using (_mockRepository.Ordered ())
      {
        StreamingContext context = new StreamingContext ();
        _receiver.OnDeserializing (context);
        LastCall.IgnoreArguments ();

        _receiver.OnDeserialized (context);
        LastCall.IgnoreArguments ();

        _receiver.OnDeserialization (null);
        LastCall.IgnoreArguments ();
      }
    }
  }
}