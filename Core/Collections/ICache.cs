using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Collections
{
  //TODO: Doc
  public interface ICache<TKey, TValue>
  {
    void Add (TKey key, TValue value);
    bool TryGetValue (TKey key, out TValue value);
    void Clear ();
  }
}