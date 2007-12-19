using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  public class InheritanceAwareTypeDictionary<T> where T : class
  {
    private readonly Dictionary<Type, T> _values = new Dictionary<Type, T>();

    public int Count
    {
      get { return _values.Count; }
    }

    public IEnumerable<Type> Keys
    {
      get { return _values.Keys; }
    }

    public IEnumerable<T> Values
    {
      get { return _values.Values; }
    }

    public void Add (Type type, T value)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _values.Add (type, value);
    }

    public T GetExact (Type type)
    {
      if (_values.ContainsKey (type))
        return _values[type];
      else
        return null;
    }

    public T GetWithInheritance (Type type)
    {
      T exactValue = GetExact (type);
      if (exactValue != null)
        return exactValue;

      if (type.IsGenericType && !type.IsGenericTypeDefinition)
      {
        T definitionValue = GetWithInheritance (type.GetGenericTypeDefinition ());
        if (definitionValue != null)
          return definitionValue;
      }

      if (type.BaseType != null)
      {
        T baseValue = GetWithInheritance (type.BaseType);
        if (baseValue != null)
          return baseValue;
      }

      return null;
    }

    public bool ContainsExact (Type type)
    {
      return GetExact (type) != null;
    }

    public bool ContainsWithInheritance (Type type)
    {
      return GetWithInheritance (type) != null;
    }

    public bool RemoveExact (Type type)
    {
      return _values.Remove (type);
    }
  }
}