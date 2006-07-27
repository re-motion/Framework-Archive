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

    // static members

    // member fields

    private NullCache<Tupel<SecurityContext, string>, AccessType[]> _nullCache = new NullCache<Tupel<SecurityContext, string>, AccessType[]> ();

    // construction and disposing

    public ClientTransactionAccessTypeCacheProvider ()
    {
    }

    // methods and properties

    public ICache<Tupel<SecurityContext, string>, AccessType[]> GetCache ()
    {
      if (!ClientTransaction.HasCurrent)
        return _nullCache;

      ClientTransaction transaction = ClientTransaction.Current;
      string cacheKey = typeof (ClientTransactionAccessTypeCacheProvider).FullName;

      Cache<Tupel<SecurityContext, string>, AccessType[]> cache;
      object value;
      if (transaction.ApplicationData.TryGetValue (cacheKey, out value))
      {
        cache = (Cache<Tupel<SecurityContext, string>, AccessType[]>) value;
      }
      else
      {
        cache = new Cache<Tupel<SecurityContext, string>, AccessType[]> ();
        transaction.ApplicationData.Add (cacheKey, cache);
      }

      return cache;
    }
  }
}