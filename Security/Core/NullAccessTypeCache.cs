using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class NullAccessTypeCache<TKey> : IAccessTypeCache<TKey>
  {
    public NullAccessTypeCache ()
    {
    }

    public void Add (TKey key, AccessType[] accessTypes)
    {
    }

    public AccessType[] Get (TKey key)
    {
      return null;
    }

    public void Clear ()
    {
    }
  }
}