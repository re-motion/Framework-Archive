using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  public class ClassContextCollection : ICollection, ICollection<ClassContext>
  {
    private readonly Dictionary<Type, ClassContext> _values = new Dictionary<Type, ClassContext> ();
    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    public ClassContextCollection ()
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

    public IEnumerator<ClassContext> GetEnumerator ()
    {
      return _values.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public void CopyTo (ClassContext[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ((ICollection) this).CopyTo (array, arrayIndex);
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ((ICollection) _values.Values).CopyTo (array, index);
    }

    public void Clear ()
    {
      _values.Clear();
    }

    public void Add (ClassContext value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      if (ContainsExact (value.Type))
      {
        string message = string.Format ("A class context for type {0} was already added.", value.Type.FullName);
        throw new InvalidOperationException (message);
      }
      _values.Add (value.Type, value);
    }

    public bool RemoveExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _values.Remove (type);
    }

    bool ICollection<ClassContext>.Remove (ClassContext item)
    {
      if (!Contains (item))
        return false;
      else
      {
        bool result = RemoveExact (item.Type);
        Assertion.IsTrue (result);
        Assertion.IsFalse (Contains (item));
        return result;
      }
    }

    public ClassContext GetExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (_values.ContainsKey (type))
        return _values[type];
      else
        return null;
    }

    public ClassContext GetWithInheritance (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _inheritanceAlgorithm.GetWithInheritance (type);
    }

    public bool ContainsExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return GetExact (type) != null;
    }

    public bool ContainsWithInheritance (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return GetWithInheritance (type) != null;
    }

    public bool Contains (ClassContext item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      return item.Equals (GetExact (item.Type));
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)_values).SyncRoot; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    bool ICollection<ClassContext>.IsReadOnly
    {
      get { return false; }
    }
  }
}