using System;
using System.Collections;

namespace Remotion.Globalization
{

/// <summary>
///   Provides a cache that keeps an IResourceManager for each type.
/// </summary>
[Obsolete ("Use a Dictionary<Type,IResourceManager> instance instead.")]
public class ResourceManagerCache
{
  private static Hashtable s_cache = new Hashtable();

  public static IResourceManager Get (Type type)
  {
    lock (typeof (ResourceManagerCache))
    {
      return (IResourceManager) s_cache[type];
    }
  }

  public static void Add (Type type, IResourceManager resourceManager)
  {
    lock (typeof (ResourceManagerCache))
    {
      s_cache.Add (type, resourceManager);
    }
  }	
}

}
