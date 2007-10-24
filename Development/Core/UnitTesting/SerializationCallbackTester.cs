using System;
using System.Runtime.Serialization;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Development.UnitTesting
{
  public class SerializationCallbackTester<T>
  {
      private readonly MockRepository _mockRepository;
      private readonly Proc<ISerializationEventReceiver> _receiverSetter;
      private readonly T _instance;

      public SerializationCallbackTester (T instance, Proc<ISerializationEventReceiver> receiverSetter)
      {
        _mockRepository = new MockRepository ();
        _instance = instance;
        _receiverSetter = receiverSetter;

        _receiverSetter (null);
      }

    public void Test_SerializationCallbacks ()
      {
        _receiverSetter (null);
        try
        {
          ISerializationEventReceiver receiver = _mockRepository.CreateMock<ISerializationEventReceiver> ();

          _receiverSetter (receiver);
          CheckSerialization (receiver);
        }
        finally
        {
          _receiverSetter (null);
        }
      }

      public void Test_DeserializationCallbacks ()
      {
        _receiverSetter (null);
        try
        {
          byte[] bytes = Serializer.Serialize (_instance);
          ISerializationEventReceiver receiver = _mockRepository.CreateMock<ISerializationEventReceiver>();
          _receiverSetter (receiver);

          CheckDeserialization (bytes, receiver);
        }
        finally
        {
          _receiverSetter (null);
        }
      }

      private void CheckSerialization (ISerializationEventReceiver receiver)
      {
        ExpectSerializationCallbacks (receiver);

        _mockRepository.ReplayAll ();

        Serializer.Serialize (_instance);

        _mockRepository.VerifyAll ();
      }

      private void CheckDeserialization (byte[] bytes, ISerializationEventReceiver receiver)
      {
        ExpectDeserializationCallbacks (receiver);

        _mockRepository.ReplayAll ();

        Serializer.Deserialize (bytes);

        _mockRepository.VerifyAll ();
      }

      private void ExpectSerializationCallbacks (ISerializationEventReceiver receiver)
      {
        using (_mockRepository.Ordered ())
        {
          StreamingContext context = new StreamingContext ();
          receiver.OnSerializing (context);
          LastCall.IgnoreArguments ();

          receiver.OnSerialized (context);
          LastCall.IgnoreArguments ();
        }
      }

      private void ExpectDeserializationCallbacks (ISerializationEventReceiver receiver)
      {
        using (_mockRepository.Ordered ())
        {
          StreamingContext context = new StreamingContext ();
          receiver.OnDeserializing (context);
          LastCall.IgnoreArguments ();

          receiver.OnDeserialized (context);
          LastCall.IgnoreArguments ();

          receiver.OnDeserialization (null);
          LastCall.IgnoreArguments ();
        }
      }
  }
}