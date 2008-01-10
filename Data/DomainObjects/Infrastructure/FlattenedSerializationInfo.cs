using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  public class FlattenedSerializationInfo
  {
    private readonly List<object> _data = new List<object>();
    private readonly Dictionary<object, int> _handleMap = new Dictionary<object, int>();

    public FlattenedSerializationInfo ()
    {
    }

    public object[] GetData ()
    {
      return _data.ToArray();
    }

    public void AddValue<T> (T value)
    {
      IFlattenedSerializable serializable = value as IFlattenedSerializable;
      if (serializable != null)
        AddFlattenedSerializable(serializable);
      else
        AddSimpleValue(value);
    }

    private void AddFlattenedSerializable (IFlattenedSerializable serializable)
    {
      AddSimpleValue (FlattenedSerializableMarker.Instance);
      AddSimpleValue (serializable.GetType ());
      serializable.SerializeIntoFlatStructure (this);
    }

    private void AddSimpleValue<T> (T value)
    {
      _data.Add (value);
    }

    public void AddArray<T> (T[] valueArray)
    {
      ArgumentUtility.CheckNotNull ("valueArray", valueArray);
      AddCollection (valueArray);
    }

    public void AddCollection<T> (ICollection<T> valueCollection)
    {
      ArgumentUtility.CheckNotNull ("valueCollection", valueCollection);
      AddValue (valueCollection.Count);
      foreach (T t in valueCollection)
        AddValue (t);
    }

    public void AddHandle<T> (T value)
    {
      if (value == null)
        AddValue (-1);
      else
      {
        int handle;
        if (!_handleMap.TryGetValue (value, out handle))
        {
          handle = _handleMap.Count;
          _handleMap.Add (value, handle);
          AddValue (handle);
          AddValue (value);
        }
        else
          AddValue (handle);
      }
    }
  }
}