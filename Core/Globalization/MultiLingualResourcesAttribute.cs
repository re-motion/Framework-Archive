using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Globalization
{
/// <summary>
///   Attribute for specifying the resource container for a type.
/// </summary>
[AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class MultiLingualResourcesAttribute: Attribute
{
	// types

  // member fields

  /// <summary> Hashtable&lt;string, ResourceManager&gt; </summary>
  private static Hashtable s_resourceManagersCache = new Hashtable ();

  /// <summary> The root name of the resource container </summary>
  private string _resourceName;

  // construction and disposing

  /// <summary>
  ///   Simple constructor.
  /// </summary>
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/Constructor/param[@name="resourceName"]' />
  public MultiLingualResourcesAttribute (string resourceName)
  {
    _resourceName = resourceName;
  }

  // methods and properties

  /// <summary>
  ///   The root name of the resource container as specified by the attributes construction
  /// </summary>
  public string ResourceName 
  {
    get { return _resourceName; }
  }
  
  /// <summary>
  ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
  ///   in the class declaration of the type.
  /// </summary>
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/class/GetResourceManager/Common/*' />
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/class/GetResourceManager/param[@name="objectType" or @name="includeHierarchy"]' />
  public static ResourceManagerWrapper GetResourceManager (Type objectType, bool includeHierarchy)
  {
    Type definingType;
    StringCollection resourceNames;

    return GetResourceManager (
      objectType,
      includeHierarchy,
      out definingType,
      out resourceNames);
  }

  /// <summary>
  ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
  ///   in the class declaration of the type.
  /// </summary>
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/class/GetResourceManager/Common/*' />
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/class/GetResourceManager/param[@name="objectType"]' />
  public static ResourceManagerWrapper GetResourceManager (Type objectType)
  {
    return GetResourceManager (objectType, false);
  }

  public static string GetResourceText (Type objectTypeToGetResourceFor, string name)
  {
    IResourceManager rm = GetResourceManager (objectTypeToGetResourceFor);

    string text = rm.GetString (name);

    if (text == null)
      return String.Empty;

    return text;
  }

  public static string GetResourceText (object objectToGetResourceFor, string name)
  {
    ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);

    return GetResourceText (objectToGetResourceFor.GetType(), name);  
  }

  public static bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
  {
    try
    {
      IResourceManager rm = GetResourceManager (objectTypeToGetResourceFor);

      string text = rm.GetString (name);

      return (text != null);
    }
    catch
    {
      return false;
    }
  }
  
  public static bool ExistsResourceText (object objectToGetResourceFor, string name)
  {
    ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);

    return ExistsResourceText (objectToGetResourceFor.GetType(), name);  
  }

  public static bool ExistsResource (Type objectTypeToGetResourceFor)
  {
    ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
    try
    {
      Type definingType;
      StringCollection resourceNames;

      GetResourceNameAndType (objectTypeToGetResourceFor, false, out definingType, out resourceNames);

      if (GetResourceManagers (definingType.Assembly, resourceNames).Length > 0)
        return true;
      else
        return false;
    }
    catch (ResourceException)
    {
      return false;
    }
  }
  
  public static bool ExistsResource (object objectToGetResourceFor)
  {
    ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);

    return ExistsResource (objectToGetResourceFor.GetType());
  }

  /// <summary>
  ///   Returns an <c>IResourceManager</c> array for the resource containers specified
  ///   in the class declaration of the type.
  /// </summary>
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/class/GetResourceManager/Common/*' />
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/class/GetResourceManager/param[@name="objectType" or @name="resourceType" or @name="includeHierarchy" or @name="definingType" or @name="resourceNames"]' />
  private static ResourceManagerWrapper GetResourceManager (
      Type objectType,
      bool includeHierarchy,
      out Type definingType,
      out StringCollection resourceNames)
  {
    ArrayList resourceManagers = new ArrayList();

    //  Current hierarchy level, always report missing MultiLingualResourcesAttribute
    GetResourceNameAndType (objectType, false, out definingType, out resourceNames);

    resourceManagers.AddRange (
      GetResourceManagers (definingType.Assembly, resourceNames));

    if (includeHierarchy)
    {
      //  Walk through the class hierarchy
      //  and get the resources defined for these types

      while (definingType != null)
      {
        definingType = definingType.BaseType;
        
        //  No more base types
        if (definingType == null)
          break;

        GetResourceNameAndType (definingType, true, out definingType, out resourceNames);

        //  No more base types defining the MultiLingualResourcesAttribute
        if (definingType != null)
        {
          //  Insert the found resources managers at the beginning of the list
          resourceManagers.InsertRange (
            0,
            GetResourceManagers (definingType.Assembly, resourceNames));
        }
      }
    }

    return new ResourceManagerWrapper(
      (ResourceManager[])resourceManagers.ToArray(typeof(ResourceManager)));
  }

  /// <summary>
  ///   Returns an <c>ResourceManager</c> array for the resource containers
  ///   specified by resourceName.
  /// </summary>
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="assembly"]' />
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="resourceNames"]' />
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManagers/returns' />
  private static ResourceManager[] GetResourceManagers (
    Assembly assembly,
    StringCollection resourceNames)
  {
      ResourceManager[] resourceManagers = new ResourceManager [resourceNames.Count];

    //  Load the ResourceManagers for the type's resources

    for (int index = 0; index < resourceNames.Count; index++)
    {
      string key = resourceNames[index] + " in " + assembly.FullName;

      //  Look in cache and continue with cached the resource manager, if one is found

      resourceManagers[index] = s_resourceManagersCache[key] as ResourceManager;
      if (resourceManagers[index] != null)
        continue;

      //  Create a new resource mananger and put it into the cache.

      resourceManagers[index] = new ResourceManager (resourceNames[index], assembly);
      if (resourceManagers[index] == null) throw new ResourceException ("No resource with name " + resourceNames + " found in assembly \"" + assembly.FullName + "\".");

      lock (s_resourceManagersCache)
      {
        s_resourceManagersCache[key] = resourceManagers[index];
      }
    }

    return resourceManagers;
  }
  

  /// <summary>
  /// Finds the class where the <c>MultiLingualResourcesAttribute</c> was defined and returns
  /// it's type and the the names of the resource containers.
  /// </summary>
  /// <include file='doc\include\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceNameAndType/*' />
  private static void GetResourceNameAndType (
    Type concreteType,
    bool noUndefinedException,
    out Type definingType,
    out StringCollection resourceNames)
  {
    Type type = concreteType;
    MultiLingualResourcesAttribute[] resourceAttributes = GetResourceAttributes (type);

    //  Find the base type where MultiLingualResourceAttribute was specified for this type
    //  Base type can be identical to type

    while (resourceAttributes.Length == 0) 
    {
      type = type.BaseType;
      if (type == null)
      {
        if (noUndefinedException)
          break;
        else
          throw new ResourceException ("Type " + concreteType.FullName + " and its base classes do not define the attribute MultiLingualResourcesAttribute.");
      }

      resourceAttributes = GetResourceAttributes (type);
    } 

    definingType = type;
    resourceNames = new StringCollection();

    foreach (MultiLingualResourcesAttribute resourceAttribute in resourceAttributes)
        resourceNames.Add (resourceAttribute.ResourceName);
  }

  private static MultiLingualResourcesAttribute[] GetResourceAttributes (Type type)
  {
    return (MultiLingualResourcesAttribute[]) 
        type.GetCustomAttributes (typeof (MultiLingualResourcesAttribute), false);
  }
}

}
