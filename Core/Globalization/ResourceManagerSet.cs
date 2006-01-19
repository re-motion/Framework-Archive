using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.Text;
using Rubicon.Collections;
using log4net;

namespace Rubicon.Globalization
{

/// <summary>
///   Combines one or more <see cref="IResourceManager"/> instances to a set that can be accessed using a single interface.
/// </summary>
public class ResourceManagerSet: ReadOnlyCollectionBase, IResourceManager
{
	private static readonly ILog s_log = LogManager.GetLogger (typeof (ResourceManagerWrapper));

  private string _name;

  /// <summary>
  ///   Combines several IResourceManager instances to a single ResourceManagerSet, starting with the first entry of the first set.
  /// </summary>
  /// <remarks>
  ///   For parameters that are ResourceManagerSet instances, the contained IResourceManagers are added directly.
  /// </remarks>
  /// <example>
  ///   <para>
  ///     Given the following parameter list of resource managers (rm) and resource manager sets (rmset):
  ///   </para><para>
  ///     rm1, rm2, rmset (rm3, rm4, rm5), rm6, rmset (rm7, rm8)
  ///   </para><para>
  ///     The following resource manager set is created:
  ///   </para><para>
  ///     rmset (rm1, rm2, rm3, rm4, rm5, rm6, rm7, rm8)
  ///   </para>
  /// </example>
  /// <param name="resourceManagers"> The resource manager, starting with the least specific. </param>
	public ResourceManagerSet (params IResourceManager[] resourceManagers)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("resourceManagers", resourceManagers);

    TypedArrayList list = new TypedArrayList (typeof (IResourceManager));
    foreach (IResourceManager rm in resourceManagers)
    {
      ResourceManagerSet rmset = rm as ResourceManagerSet;
      if (rmset != null)
        list.AddRange (rmset.InnerList);
      else if (rm != null)
        list.Add (rm);
    }

    IResourceManager[] concreteResourceManagers = (IResourceManager[]) list.ToArray ();

    SeparatedStringBuilder sb = new SeparatedStringBuilder (", ", 30 * concreteResourceManagers.Length);
    foreach (IResourceManager rm in concreteResourceManagers)
      sb.Append (rm.Name);
    _name = sb.ToString();

    InnerList.AddRange (concreteResourceManagers);
	}

  public IResourceManager this[int index]
  {
    get { return (IResourceManager) InnerList[index]; }
  }
  
  public NameValueCollection GetAllStrings()
  {
    return GetAllStrings (string.Empty);
  }

  /// <summary>
  ///   Searches for all string resources inside the resource manager whose name is prefixed with a matching tag.
  /// </summary>
  /// <seealso cref="M:Rubicon.Globalization.IResourceManager.GetAllStrings(System.String)"/>
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

  /// <summary>
  ///   Gets the value of the specified string resource. 
  /// </summary>
  /// <seealso cref="M:Rubicon.Globalization.IResourceManager.GetString(System.String)"/>
  public string GetString (string id)
  {
    for (int i = this.Count - 1; i >= 0; --i)
    {
      string s = this[i].GetString (id);
      if (s != null && s != id)
        return s;
    }

    s_log.Debug ("Could not find resource with ID '" + id + "' in any of the following resource containers " + _name + ".");
    return id;
  }

  /// <summary>
  ///   Gets the value of the specified string resource. 
  /// </summary>
  /// <seealso cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)"/>
  public string GetString (Enum enumValue)
  {
    return GetString (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
  }

  public string Name
  {
    get { return _name; }
  }
}

}
