using System;
using System.Configuration.Provider;
using Rubicon.Collections;

namespace Rubicon.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IGlobalAccessTypeCacheProvider"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullGlobalAccessTypeCacheProvider : ProviderBase, IGlobalAccessTypeCacheProvider
  {
    private NullCache<Tuple<SecurityContext, string>, AccessType[]> _cache = new NullCache<Tuple<SecurityContext, string>, AccessType[]>();

    public NullGlobalAccessTypeCacheProvider()
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