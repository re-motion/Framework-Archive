using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Globalization
{

/// <summary>
///   Combines one or more <see cref="IResourceManager"/> instances to a set that can be accessed using a single interface.
/// </summary>
public class ResourceManagerSet: ReadOnlyCollectionBase, IResourceManager
{
  /// <summary>
  ///   Combines several ResourceManagerSets to a single ResourceManagerSet, starting with the first entry of the first set.
  /// </summary>
  /// <param name="resourceManagerSets"> The resource manager sets, starting with the least specific. </param>
  public static ResourceManagerSet Combine (params ResourceManagerSet[] resourceManagerSets)
  {
    ArrayList list = new ArrayList();
    foreach (ResourceManagerSet set in resourceManagerSets)
      list.AddRange (set.InnerList);

    return new ResourceManagerSet ((IResourceManager[]) list.ToArray (typeof (IResourceManager)));
  }

  private string _name;

  /// <summary>
  ///   Creates a new ResourceManagerSet.
  /// </summary>
  /// <param name="resourceManagers"> The resource managers, starting with the least specific. </param>
	public ResourceManagerSet (params IResourceManager[] resourceManagers)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("resourceManagers", resourceManagers);

    StringBuilder sb = new StringBuilder (30 * resourceManagers.Length);
    for (int i = 0; i < resourceManagers.Length; ++i)
    {
      if (resourceManagers[i] == null)
        throw new ArgumentNullException ("resourceManagers[" + i + "]");

      if (i > 0)
        sb.Append (", ");
      sb.Append (resourceManagers[i].Name);
    }
    _name = sb.ToString();

    InnerList.AddRange (resourceManagers);
	}

  public IResourceManager this[int index]
  {
    get { return (IResourceManager) InnerList[index]; }
  }
  
  public NameValueCollection GetAllStrings()
  {
    return GetAllStrings (string.Empty);
  }

  public NameValueCollection GetAllStrings (string prefix)
  {
    NameValueCollection result = new NameValueCollection();

    foreach (IResourceManager resourceManager in this)
    {
      NameValueCollection strings = resourceManager.GetAllStrings (prefix);
      for (int i = 0; i < strings.Count; i++)
      {
        string key = strings.Keys[i];
        result[key] = strings[i];
      }
    }
    return result;
  }

  public string GetString (string id)
  {
    for (int i = this.Count - 1; i >= 0; --i)
    {
      string s = this[i].GetString (id);
      if (s != null)
        return s;
    }
    return null;
  }

  public string GetString (Type type, Enum enumValue)
  {
    return GetString (type.FullName + "." + enumValue.ToString());
  }

  public string Name
  {
    get { return _name; }
  }
}

}
