using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class AccessTypeCache<TKey> : IAccessTypeCache<TKey>
  {
    // types

    // static members

    // member fields

    private Dictionary<TKey, AccessType[]> _cache = new Dictionary<TKey,AccessType[]>();

    // construction and disposing

    public AccessTypeCache ()
    {
    }

    // methods and properties

    public void Add (TKey key, AccessType[] accessTypes)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNullOrItemsNull ("accessTypes", accessTypes);

      _cache[key] = accessTypes;
    }

    public AccessType[] Get (TKey key)
    {
      AccessType[] accessTypes;
      if (_cache.TryGetValue (key, out accessTypes))
        return accessTypes;

      return null;
    }

    public void Clear ()
    {
      _cache.Clear ();
    }
  }
}