using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Collections.Specialized;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Globalization
{
/// <summary>
///   Attribute for specifying the resource container for a type.
/// </summary>
[AttributeUsage (AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
public class MultiLingualResourcesAttribute: Attribute
{
	// types

  // member fields

  /// <summary> Hashtable&lt;string, ResourceManager&gt; </summary>
  private static Hashtable s_resourceManagersCache = new Hashtable ();

  /// <summary> Hashtable&lt;string, ResourceManagerWrapper&gt; </summary>
  private static Hashtable s_resourceManagerWrappersCache = new Hashtable ();

  /// <summary> The base name of the resource container </summary>
  private string _baseName;

  // construction and disposing

  /// <summary> Initalizes an instance. </summary>
  public MultiLingualResourcesAttribute (string baseName)
  {
    _baseName = baseName;
  }

  // methods and properties

  /// <summary>
  ///   Gets the base name of the resource container as specified by the attributes construction.
  /// </summary>
  /// <remarks>
  /// The base name of the resource conantainer to be used by this type
  /// (&lt;assembly&gt;.&lt;path inside project&gt;.&lt;resource file name without extension&gt;).
  /// </remarks>
  public string BaseName 
  {
    get { return _baseName; }
  }
  
  /// <summary>
  ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
  ///   in the class declaration of the type.
  /// </summary>
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManager/Common/*' />
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManager/param[@name="objectType" or @name="includeHierarchy"]' />
  public static ResourceManagerSet GetResourceManager (Type objectType, bool includeHierarchy)
  {
    Type definingType;
    StringCollection baseNames;

    return GetResourceManager (
      objectType,
      includeHierarchy,
      out definingType,
      out baseNames);
  }

  /// <summary>
  ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
  ///   in the class declaration of the type.
  /// </summary>
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManager/Common/*' />
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManager/param[@name="objectType"]' />
  public static ResourceManagerSet GetResourceManager (Type objectType)
  {
    return GetResourceManager (objectType, false);
  }

  /// <summary>
  ///   Returns an <c>IResourceManager</c> array for the resource containers specified
  ///   in the class declaration of the type.
  /// </summary>
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManager/Common/*' />
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManager/param[@name="objectType" or @name="includeHierarchy" or @name="definingType" or @name="baseNames"]' />
  private static ResourceManagerSet GetResourceManager (
      Type objectType,
      bool includeHierarchy,
      out Type definingType,
      out StringCollection baseNames)
  {
    ArrayList resourceManagers = new ArrayList();

    //  Current hierarchy level, always report missing MultiLingualResourcesAttribute
    GetResourceNameAndType (objectType, false, out definingType, out baseNames);

    ResourceManagerSet resourceManagerSet = null;

    string key = definingType.AssemblyQualifiedName + "/" + includeHierarchy.ToString();
   
    //  Look in cache and continue with the cached resource manager wrapper, if one is found
    resourceManagerSet = s_resourceManagerWrappersCache[key] as ResourceManagerSet;
    
    if (resourceManagerSet != null)
      return resourceManagerSet;

    //  Not found in cache, get new resource managers

    resourceManagers.AddRange (GetResourceManagers (definingType.Assembly, baseNames));

    if (includeHierarchy)
    {
      //  Walk through the class hierarchy
      //  and get the resources defined for these types

      Type currentType = definingType;
      while (currentType != null)
      {
        currentType = currentType.BaseType;
        
        //  No more base types
        if (currentType == null)
          break;

        GetResourceNameAndType (currentType, true, out currentType, out baseNames);

        //  No more base types defining the MultiLingualResourcesAttribute
        if (currentType != null)
        {
          //  Insert the found resources managers at the beginning of the list
          resourceManagers.InsertRange (
              0,
              GetResourceManagers (currentType.Assembly, baseNames));
        }
      }
    }

    //  Create a new resource mananger wrapper set and put it into the cache.

    resourceManagerSet = ResourceManagerWrapper.CreateWrapperSet (
        (ResourceManager[])resourceManagers.ToArray(typeof(ResourceManager)));
    
    lock (s_resourceManagerWrappersCache)
    {
      s_resourceManagerWrappersCache[key] = resourceManagerSet;
    }

    return resourceManagerSet;
  }
  /// <summary>
  ///   Returns an <c>ResourceManager</c> array for the resource containers
  ///   specified by baseName.
  /// </summary>
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManagers/*' />
  private static ResourceManager[] GetResourceManagers (
      Assembly assembly,
      StringCollection baseNames)
  {
    ResourceManager[] resourceManagers = new ResourceManager [baseNames.Count];

    //  Load the ResourceManagers for the type's resources

    for (int index = 0; index < baseNames.Count; index++)
    {
      string key = baseNames[index] + " in " + assembly.FullName;

      //  Look in cache 
      resourceManagers[index] = (ResourceManager) s_resourceManagersCache[key];
      if (resourceManagers[index] == null)
      {
        //  Create a new resource mananger and put it into the cache.
        lock (s_resourceManagersCache)
        {
          resourceManagers[index] = (ResourceManager) s_resourceManagersCache[key];
          if (resourceManagers[index] == null)
          {
            resourceManagers[index] = new ResourceManager (baseNames[index], assembly);
            if (resourceManagers[index] == null) throw new ResourceException ("No resource with name '" + baseNames + "' found in assembly '" + assembly.FullName + "'.");

            s_resourceManagersCache[key] = resourceManagers[index];
          }
        }
      }
    }

    return resourceManagers;
  }
  
  /// <summary>
  ///   Loads a string resource for the specified type, identified by ID.
  /// </summary>
  /// <param name="objectTypeToGetResourceFor">
  ///   The <see cref="Type"/> for which to get the resource.
  /// </param>
  /// <param name="name"> The ID of the resource. </param>
  /// <returns> The found string resource or an empty string. </returns>
  public static string GetResourceText (Type objectTypeToGetResourceFor, string name)
  {
    IResourceManager rm = GetResourceManager (objectTypeToGetResourceFor);

    string text = rm.GetString (name);

    if (text == name)
      return String.Empty;

    return text;
  }

  /// <summary>
  ///   Loads a string resource for the object's type, identified by ID.
  /// </summary>
  /// <param name="objectToGetResourceFor">
  ///   The object for whose <see cref="Type"/> to get the resource.
  /// </param>
  /// <param name="name"> The ID of the resource. </param>
  /// <returns> The found string resource or an empty string. </returns>
  public static string GetResourceText (object objectToGetResourceFor, string name)
  {
    ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);

    return GetResourceText (objectToGetResourceFor.GetType(), name);  
  }

  /// <summary>
  ///   Checks for the existence of a string resource for the specified type, identified by ID.
  /// </summary>
  /// <param name="objectTypeToGetResourceFor">
  ///   The <see cref="Type"/> for which to check the resource.
  /// </param>
  /// <param name="name"> The ID of the resource. </param>
  /// <returns> <see langword="true"/> if the resource can be found. </returns>
  public static bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
  {
    try
    {
      IResourceManager rm = GetResourceManager (objectTypeToGetResourceFor);

      string text = rm.GetString (name);

      return (text != name);
    }
    catch
    {
      return false;
    }
  }
  
  /// <summary>
  ///   Checks for the existence of a string resource for the specified type, identified by ID.
  /// </summary>
  /// <param name="objectToGetResourceFor">
  ///   The object for whose <see cref="Type"/> to check the resource.
  /// </param>
  /// <param name="name"> The ID of the resource. </param>
  /// <returns> <see langword="true"/> if the resource can be found. </returns>
  public static bool ExistsResourceText (object objectToGetResourceFor, string name)
  {
    ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);

    return ExistsResourceText (objectToGetResourceFor.GetType(), name);  
  }

  /// <summary>
  ///   Checks for the existence of a resource set for the specified type.
  /// </summary>
  /// <param name="objectTypeToGetResourceFor">
  ///   The <see cref="Type"/> for which to check for the resource set.
  /// </param>
  /// <returns> <see langword="true"/> if the resource ser can be found. </returns>
  public static bool ExistsResource (Type objectTypeToGetResourceFor)
  {
    ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
    try
    {
      Type definingType;
      StringCollection baseNames;

      GetResourceNameAndType (objectTypeToGetResourceFor, false, out definingType, out baseNames);

      if (GetResourceManagers (definingType.Assembly, baseNames).Length > 0)
        return true;
      else
        return false;
    }
    catch (ResourceException)
    {
      return false;
    }
  }
  
  /// <summary>
  ///   Checks for the existence of a resource set for the specified type.
  /// </summary>
  /// <param name="objectToGetResourceFor">
  ///   The object for whose <see cref="Type"/> to check for the resource set.
  /// </param>
  /// <returns> <see langword="true"/> if the resource ser can be found. </returns>
  public static bool ExistsResource (object objectToGetResourceFor)
  {
    ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);

    return ExistsResource (objectToGetResourceFor.GetType());
  }

  /// <summary>
  /// Finds the class where the <c>MultiLingualResourcesAttribute</c> was defined and returns
  /// its type and the the names of the resource containers.
  /// </summary>
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceNameAndType/*' />
  private static void GetResourceNameAndType (
      Type concreteType,
      bool noUndefinedException,
      out Type definingType,
      out StringCollection baseNames)
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
    baseNames = new StringCollection();

    foreach (MultiLingualResourcesAttribute resourceAttribute in resourceAttributes)
      baseNames.Add (resourceAttribute.BaseName);
  }

  /// <summary>
  ///   Returns the <see cref="MultiLingualResourcesAttribute"/> attributes defined for the 
  ///   <paramref name="type"/>.
  /// </summary>
  /// <param name="type">The <see cref="Type"/> to analyze.</param>
  /// <returns>An array of <see cref="MultiLingualResourcesAttribute"/> attributes.</returns>
  private static MultiLingualResourcesAttribute[] GetResourceAttributes (Type type)
  {
    return (MultiLingualResourcesAttribute[]) 
        type.GetCustomAttributes (typeof (MultiLingualResourcesAttribute), false);
  }
}

}
