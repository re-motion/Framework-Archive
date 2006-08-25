using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.Collections;

namespace Rubicon.Security.Data.DomainObjects
{
  public class ClientTransactionAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    // types

    public enum CacheKey
    {
      Value
    }

    // static members

    // member fields

    private NullCache<Tuple<SecurityContext, string>, AccessType[]> _nullCache = new NullCache<Tuple<SecurityContext, string>, AccessType[]> ();

    // construction and disposing

    public ClientTransactionAccessTypeCacheProvider ()
    {
    }

    // methods and properties

    public ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ()
    {
      if (!ClientTransaction.HasCurrent)
        return _nullCache;

      ClientTransaction transaction = ClientTransaction.Current;

      Cache<Tuple<SecurityContext, string>, AccessType[]> cache;
      object value;
      if (transaction.ApplicationData.TryGetValue (CacheKey.Value, out value))
      {
        cache = (Cache<Tuple<SecurityContext, string>, AccessType[]>) value;
      }
      else
      {
        cache = new Cache<Tuple<SecurityContext, string>, AccessType[]> ();
        transaction.ApplicationData.Add (CacheKey.Value, cache);
      }

      return cache;
    }
  }
}