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

    public bool TryGet (TKey key, out TValue value)
    {
      value = default (TValue);
      return false;
    }

    public void Clear ()
    {
    }
  }
}