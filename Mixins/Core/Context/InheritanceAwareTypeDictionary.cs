using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  public class InheritanceAwareTypeDictionary<T> where T : class
  {
    private readonly Func<Type, T, T, T> _combinator;
    private readonly Func<Type, T, T> _adjuster;
    private readonly Dictionary<Type, T> _values = new Dictionary<Type, T>();

    public InheritanceAwareTypeDictionary (Func<Type, T, T, T> combinator, Func<Type, T, T> adjuster)
    {
      ArgumentUtility.CheckNotNull ("combinator", combinator);
      ArgumentUtility.CheckNotNull ("adjuster", adjuster);

      _combinator = combinator;
      _adjuster = adjuster;
    }

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

      T definitionValue = null;
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
        definitionValue = GetWithInheritance (type.GetGenericTypeDefinition ());

      T baseValue = null;
      if (type.BaseType != null)
        baseValue = GetWithInheritance (type.BaseType);

      if (definitionValue != null && baseValue != null)
        return CombineFor (type, baseValue, definitionValue);
      else if (definitionValue != null)
        return AdjustFor (type, definitionValue);
      else if (baseValue != null)
        return AdjustFor (type, baseValue);
      else
        return null;
    }

    private T CombineFor (Type type, T partOne, T partTwo)
    {
      return _combinator (type, partOne, partTwo);
    }

    private T AdjustFor (Type type, T value)
    {
      return _adjuster (type, value);
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