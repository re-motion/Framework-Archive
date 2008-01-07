using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  public class DomainObjectSerializationInfo
  {
    private readonly List<object> _data = new List<object>();
    private readonly Dictionary<object, int> _handleMap = new Dictionary<object, int>();

    public DomainObjectSerializationInfo ()
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
        serializable.SerializeIntoFlatStructure (this);
      else
        _data.Add (value);
    }

    public void AddArray<T> (T[] valueArray)
    {
      ArgumentUtility.CheckNotNull ("valueArray", valueArray);
      AddValue (valueArray.Length);
      for (int i = 0; i < valueArray.Length; ++i)
        AddValue (valueArray[i]);
    }

    public void AddHandle<T> (T value)
    {
      int handle;
      if (!_handleMap.TryGetValue (value, out handle))
      {
        handle = _handleMap.Count;
        AddValue (handle);
        AddValue (value);
        _handleMap.Add (value, handle);
      }
      else
        AddValue (handle);
    }
  }
}