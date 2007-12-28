using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  public class InheritanceAwareClassContextCollection
  {
    private readonly Dictionary<Type, ClassContext> _values = new Dictionary<Type, ClassContext> ();
    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    public InheritanceAwareClassContextCollection ()
    {
      _inheritanceAlgorithm = new InheritedClassContextRetrievalAlgorithm (GetExact, GetWithInheritance);
    }

    public int Count
    {
      get { return _values.Count; }
    }

    public IEnumerable<Type> Keys
    {
      get { return _values.Keys; }
    }

    public IEnumerable<ClassContext> Values
    {
      get { return _values.Values; }
    }

    public void Add (Type type, ClassContext value)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _values.Add (type, value);
    }

    public bool Remove (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _values.Remove (type);
    }

    public ClassContext GetExact (Type type)
    {
      if (_values.ContainsKey (type))
        return _values[type];
      else
        return null;
    }

    public ClassContext GetWithInheritance (Type type)
    {
      return _inheritanceAlgorithm.GetWithInheritance (type);
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