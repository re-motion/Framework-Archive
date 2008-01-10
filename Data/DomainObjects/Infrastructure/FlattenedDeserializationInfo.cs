using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  public class FlattenedDeserializationInfo
  {
    private readonly object[] _data;
    private readonly Dictionary<int, object> _handleMap = new Dictionary<int, object>();

      private int _readPosition = 0;

    public FlattenedDeserializationInfo (object[] data)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      _data = data;
    }

    public T GetValue<T> ()
    {
      int originalPosition = _readPosition;
      object o = ReadValue();
      if (o is FlattenedSerializableMarker)
        return GetFlattenedSerializable<T> (originalPosition);
      else
        return CastValue<T> (o, originalPosition);
    }

    private T GetFlattenedSerializable<T> (int originalPosition)
    {
      Type type = GetValue<Type>();
      object instance = TypesafeActivator.CreateInstance (type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With (this);
      return CastValue<T>(instance, originalPosition);
    }

    private object ReadValue ()
    {
      if (_readPosition >= _data.Length)
        throw new SerializationException (string.Format ("There is no more data in the serialization stream at position {0}.", _readPosition));

      object value = _data[_readPosition];
      ++_readPosition;
      return value;
    }

    private T CastValue<T> (object uncastValue, int originalPosition)
    {
      T value;
      try
      {
        value = (T) uncastValue;
      }
      catch (InvalidCastException ex)
      {
        string message = string.Format ("The serialization stream contains an object of type {0} at position {1}, but an object of type {2} was "
            + "expected.", uncastValue.GetType ().FullName, originalPosition, typeof (T).FullName);
        throw new SerializationException (message, ex);
      }
      catch (NullReferenceException ex)
      {
        string message = string.Format ("The serialization stream contains a null value at position {0}, but an object of type {1} was "
            + "expected.", originalPosition, typeof (T).FullName);
        throw new SerializationException (message, ex);
      }
      return value;
    }

    public T[] GetArray<T> ()
    {
      int length = GetValue<int> ();
      T[] array = new T[length];
      for (int i = 0; i < length; ++i)
        array[i] = GetValue<T> ();
      return array;
    }

    public void FillCollection<T> (ICollection<T> targetCollection)
    {
      int length = GetValue<int> ();
      for (int i = 0; i < length; ++i)
        targetCollection.Add (GetValue<T> ());
    }

    public T GetValueForHandle<T> ()
    {
      int handle = GetValue<int> ();
      if (handle == -1)
        return (T) (object) null;
      else
      {
        object objectValue;
        if (!_handleMap.TryGetValue (handle, out objectValue))
        {
          T value = GetValue<T> ();
          _handleMap.Add (handle, value);
          return value;
        }
        else
          return CastValue<T> (objectValue, _readPosition);
      }
    }
  }
}