using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Rubicon.Collections;
using Rubicon.Configuration;

namespace Rubicon.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IGlobalAccessTypeCacheProvider"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullGlobalAccessTypeCacheProvider : ExtendedProviderBase, IGlobalAccessTypeCacheProvider
  {
    private NullCache<Tuple<SecurityContext, string>, AccessType[]> _cache = new NullCache<Tuple<SecurityContext, string>, AccessType[]>();

    public NullGlobalAccessTypeCacheProvider()
        : this ("Null", new NameValueCollection())
    {
    }


    public NullGlobalAccessTypeCacheProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache()
    {
      return _cache;
    }

    bool INullableObject.IsNull
    {
      get { return true; }
    }
  }
}