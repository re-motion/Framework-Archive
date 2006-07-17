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

    private NullAccessTypeCache<Tupel<SecurityContext, string>> _nullCache = new NullAccessTypeCache<Tupel<SecurityContext, string>> ();

    // construction and disposing

    public ClientTransactionAccessTypeCacheProvider ()
    {
    }

    // methods and properties

    public IAccessTypeCache<Tupel<SecurityContext, string>> GetAccessTypeCache ()
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