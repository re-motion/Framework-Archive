using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public interface IAccessTypeCache<TKey>
  {
    void Add (TKey key, AccessType[] accessTypes);
    AccessType[] Get (TKey key);
    void Clear ();
  }
}