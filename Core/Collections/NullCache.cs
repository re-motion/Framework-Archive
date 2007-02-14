using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Collections
{
  //TODO: Doc
  public class NullCache<TKey, TValue> : ICache<TKey, TValue>
  {
    public NullCache ()
    {
    }

    public void Add (TKey key, TValue value)
    {
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      value = default (TValue);
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);
      return valueFactory();
    }

    public void Clear ()
    {
    }

    bool INullableObject.IsNull
    {
      get { return true; }
    }
  }
}