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
///     Limited on resources for the current UI culture and its less specific cultures.
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
  /// <include file='doc\include\Globalization\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/Constructor/param[@name="resourceManager"]' />
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
  ///   searches for resources. Multiple roots are separated by a comma.
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
  ///   <seealso cref="IResourceManager.GetAllStrings"/>
  /// </summary>
  /// <include file='doc\include\Globalization\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/GetAllStrings/remarks' />
  public NameValueCollection GetAllStrings (string prefix)
  {
    if (prefix == null || prefix == String.Empty)
      prefix = "";

    NameValueCollection result = new NameValueCollection();

    //  Loop through all entries in the resource managers
    //  Copy the resources into a collection

    CultureInfo[] cultureHierarchy = GetCultureHierarchy (CultureInfo.CurrentUICulture);

    // Loop from most neutral to current UICulture
    for (int i = 0; i < cultureHierarchy.Length; i++)
    {
      CultureInfo culture = (CultureInfo) cultureHierarchy[i];
      ResourceSet resourceSet = _resourceManager.GetResourceSet (culture, true, false);
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
  ///   Gets the value of the specified string resource. 
  ///   <seealso cref="IResourceManager.GetString"/>
  /// </summary>
  public string GetString (Enum enumValue)
  {
    return GetString (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
  }

  /// <summary>
  ///   Gets the value of the specified string resource. 
  ///   <seealso cref="IResourceManager.GetString"/>
  /// </summary>
  public string GetString (string id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    string result = _resourceManager.GetString (id);
    if (result != null)
      return result;
    
    s_log.Debug ("Could not find resource with ID '" + id + "' in reosurce container '" + _resourceManager.BaseName + "'.");
    return id;
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
