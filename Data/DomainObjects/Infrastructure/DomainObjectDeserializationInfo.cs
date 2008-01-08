using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  public class DomainObjectDeserializationInfo
  {
    private readonly object[] _data;
    private readonly Dictionary<int, object> _handleMap = new Dictionary<int, object>();

      private int _readPosition = 0;

    public DomainObjectDeserializationInfo (object[] data)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      _data = data;
    }

    public T GetValue<T> ()
    {
      if (typeof (IFlattenedSerializable).IsAssignableFrom (typeof (T)))
        throw new InvalidOperationException ("This method does not support deserialization of IFlattenedSerializable implementations. Use the "
            + "overload taking a deserializer instead.");

      if (_readPosition >= _data.Length)
        throw new SerializationException (string.Format ("There is no more data in the serialization stream at position {0}.", _readPosition));

      T value = CastValue<T> (_data[_readPosition]);

      ++_readPosition;
      return value;
    }

    public T GetValue<T> (Func<DomainObjectDeserializationInfo, T> deserializer) where T : IFlattenedSerializable
    {
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);
      return deserializer (this);
    }

    private T CastValue<T> (object uncastValue)
    {
      T value;
      try
      {
        value = (T) uncastValue;
      }
      catch (InvalidCastException ex)
      {
        string message = string.Format ("The serialization stream contains an object of type {0} at position {1}, but an object of type {2} was "
            + "expected.", uncastValue.GetType ().FullName, _readPosition, typeof (T).FullName);
        throw new SerializationException (message, ex);
      }
      catch (NullReferenceException ex)
      {
        string message = string.Format ("The serialization stream contains a null value at position {0}, but an object of type {1} was "
            + "expected.", _readPosition, typeof (T).FullName);
        throw new SerializationException (message, ex);
      }
      return value;
    }

    public T[] GetArray<T> ()
    {
      int length = GetValue<int>();
      T[] array = new T[length];
      for (int i = 0; i < length; ++i)
        array[i] = GetValue<T>();
      return array;
    }

    public T[] GetArray<T> (Func<DomainObjectDeserializationInfo, T> deserializer) where T : IFlattenedSerializable
    {
      int length = GetValue<int> ();
      T[] array = new T[length];
      for (int i = 0; i < length; ++i)
        array[i] = GetValue (deserializer);
      return array;
    }

    public T GetValueForHandle<T> ()
    {
      int handle = GetValue<int>();
      object objectValue;
      if (!_handleMap.TryGetValue (handle, out objectValue))
      {
        T value = GetValue<T> ();
        _handleMap.Add (handle, value);
        return value;
      }
      else
        return CastValue<T> (objectValue);
    }

    public T GetValueForHandle<T> (Func<DomainObjectDeserializationInfo, T> deserializer) where T : IFlattenedSerializable
    {
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);
      int handle = GetValue<int> ();
      object objectValue;
      if (!_handleMap.TryGetValue (handle, out objectValue))
      {
        T value = GetValue<T> (deserializer);
        _handleMap.Add (handle, value);
        return value;
      }
      else
        return CastValue<T> (objectValue);
    }
  }
}