using System;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Reflection;
using System.Resources;

using Rubicon.Utilities;
using log4net;

namespace Rubicon.Globalization
{

/// <summary>
///   A wrapper for the .net Framework <c>ResourceManager</c> implementation.
/// </summary>
/// <remarks>
///   <para>
///     Limited to accessing string resources.
///     Limited on resources for the current UI culture and it's less specific cultures.
///   </para><para>
///     If multiple Resource Managers are added which belonging to derived types, 
///     make sure to sort the resource managers in the order of inheritance before wrapping them.
///   </para>
/// </remarks>
public class ResourceManagerWrapper: IResourceManager
{
  //  static members

	private static readonly ILog s_log = LogManager.GetLogger (typeof (ResourceManagerWrapper));

  public static ResourceManagerSet CreateWrapperSet (ResourceManager[] resourceManagers)
  {
    ResourceManagerWrapper[] wrappers = new ResourceManagerWrapper[resourceManagers.Length];
    for (int i = 0; i < wrappers.Length; ++i)
      wrappers[i] = new ResourceManagerWrapper (resourceManagers[i]);

    return new ResourceManagerSet (wrappers);
  }

  // member fields

  private ResourceManager _resourceManager;

  // construction and disposing

  /// <summary>
  ///   Constructor for wrapping multiple resource managers
  /// </summary>
  /// <include file='doc\include\Globalization\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/Constructor/param[@name="resourceManagers"]' />
  public ResourceManagerWrapper (ResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
    _resourceManager = resourceManager;
  }

  // methods and properties

  /// <summary>
  ///   Gets the wrapped <c>ResourceManager</c> instance. 
  /// </summary>
  public ResourceManager ResourceManager 
  {
    get { return _resourceManager; }
  }

  /// <summary>
  ///   Gets the root names of the resource files that the <c>IResourceManager</c>
  ///   searches for resources. Multiple roots are seperated by a comma.
  /// </summary>
  string IResourceManager.Name
  {
    get { return _resourceManager.BaseName; }
  }

  /// <summary>
  ///   Returns all string resources inside the wrapped resource managers.
  /// </summary>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
  /// </returns>
  public NameValueCollection GetAllStrings()
  {
    return GetAllStrings (string.Empty);
  }

  /// <summary>
  ///   Searches for all string resources inside the resource manager whose name is prefixed 
  ///   with a matching tag.
  /// </summary>
  /// <include file='doc\include\Globalization\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/GetAllStrings/remarks' />
  /// <param name="prefix">
  ///   The prefix all returned string resources must have.
  /// </param>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
  /// </returns>
  public NameValueCollection GetAllStrings (string prefix)
  {
    if (prefix == null || prefix == String.Empty)
      prefix = "";

    NameValueCollection result = new NameValueCollection();

    //  Loop through all entries in the resource managers
    //  Copy the resources into a collection

    CultureInfo[] cultureHierarchy = GetCultureHierarchy (CultureInfo.CurrentUICulture);

    // Loop from most neutral to current UICulture
    foreach (CultureInfo culture in cultureHierarchy)
    {
      ResourceSet resourceSet = null;

      resourceSet = _resourceManager.GetResourceSet (culture, true, false);
      if (resourceSet != null)
      {
        foreach (DictionaryEntry entry in resourceSet)
        {
          string key = (string)entry.Key;

          if (key.StartsWith (prefix))
            result[key] = (string)entry.Value;
        }
      }
    }

    return result;
  }

  /// <summary>
  ///   Gets the value of the specified String resource. The resource is identified by
  ///   concatenating the type's FullName and the enumvalue's string representation.
  /// </summary>
  /// <param name="type">The type to which the resource belongs</param>
  /// <param name="enumValue">The last part of the reosurce identifier.</param>
  /// <returns>
  ///   The value of the resource. If a match is not possible, a null reference is returned
  /// </returns>
  public string GetString (Type type, Enum enumValue)
  {
    return GetString (type.FullName + "." + enumValue.ToString());
  }

  /// <summary>
  ///   Gets the value of the specified String resource.
  /// </summary>
  /// <include file='doc\include\Globalization\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/GetAllStrings/remarks' />
  /// <param name="id">
  ///   The ID of the resource to get. Must not be <see langname="null"/>.
  /// </param>
  /// <returns>
  ///   The value of the resource. If a match is not possible, <see langname="null"/> is returned.
  /// </returns>
  public string GetString (string id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    return _resourceManager.GetString (id);
  }

  /// <summary>
  ///   Returns the culture hierarchy, starting with the most specialized culture.
  /// </summary>
  /// <param name="mostSpecialized">
  ///   The starting point for walking the culture tree upwards. Must not be <see langame="null"/>.
  /// </param>
  /// <returns>
  ///   The cultures, starting with the invariant culture, ending with the most specialized culture.
  /// </returns>
  public static CultureInfo[] GetCultureHierarchy (CultureInfo mostSpecialized)
  {
    ArrayList hierarchyTopDown = new ArrayList();
    
    CultureInfo currentLevel = mostSpecialized;

    do {
      hierarchyTopDown.Add (currentLevel);
      currentLevel = currentLevel.Parent;
    } while (currentLevel != CultureInfo.InvariantCulture);

    if (mostSpecialized != CultureInfo.InvariantCulture)
      hierarchyTopDown.Add (currentLevel);

    hierarchyTopDown.Reverse();

    return (CultureInfo[])hierarchyTopDown.ToArray (typeof (CultureInfo));
  }
}

}
