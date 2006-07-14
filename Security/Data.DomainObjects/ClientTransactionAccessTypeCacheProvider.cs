using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Data.DomainObjects
{
  public class ClientTransactionAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    // types

    // static members

    // member fields

    private NullAccessTypeCache<GlobalAccessTypeCacheKey> _nullCache = new NullAccessTypeCache<GlobalAccessTypeCacheKey> ();

    // construction and disposing

    public ClientTransactionAccessTypeCacheProvider ()
    {
    }

    // methods and properties

    public IAccessTypeCache<GlobalAccessTypeCacheKey> GetAccessTypeCache ()
    {
      if (!ClientTransaction.HasCurrent)
        return _nullCache;

      ClientTransaction transaction = ClientTransaction.Current;      
      string extensionKey = typeof (AccessTypeCacheClientTransactionExtension).FullName;
      AccessTypeCacheClientTransactionExtension extension = (AccessTypeCacheClientTransactionExtension) transaction.Extensions[extensionKey];
      if (extension == null)
      {
        extension = new AccessTypeCacheClientTransactionExtension ();
        transaction.Extensions.Add (extensionKey, extension);
      }

      return extension.Cache;
    }
  }
}