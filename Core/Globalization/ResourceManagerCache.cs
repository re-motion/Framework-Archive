using System;
using System.Collections;

namespace Rubicon.Globalization
{

/// <summary>
///   Provides a cache that keeps an IResourceManager for each type.
/// </summary>
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
