using System;
using System.Collections;
using System.Reflection;
using System.Resources;
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
  private string _baseName = null;
  private Assembly _resourceAssembly = null;

  // construction and disposing

  /// <summary> Initalizes an instance. </summary>
  public MultiLingualResourcesAttribute (string baseName)
  {
    SetBaseName (baseName);
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
  
  protected void SetBaseName (string baseName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("baseName", baseName);
    _baseName = baseName;
  }

  public Assembly ResourceAssembly
  {
    get { return _resourceAssembly; }
  }

  protected void SetResourceAssembly (Assembly resourceAssembly)
  {
    ArgumentUtility.CheckNotNull ("resourceAssembly", resourceAssembly);
    _resourceAssembly = resourceAssembly;
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

    return GetResourceManager (
      objectType,
      includeHierarchy,
      out definingType);
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
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManager/param[@name="objectType" or @name="includeHierarchy" or @name="definingType"]' />
  private static ResourceManagerSet GetResourceManager (
      Type objectType,
      bool includeHierarchy,
      out Type definingType)
  {
    ArrayList resourceManagers = new ArrayList();

    MultiLingualResourcesAttribute[] resourceAttributes;
    //  Current hierarchy level, always report missing MultiLingualResourcesAttribute
    GetResourceNameAndType (objectType, false, out definingType, out resourceAttributes);

    ResourceManagerSet resourceManagerSet = null;

    string key = definingType.AssemblyQualifiedName + "/" + includeHierarchy.ToString();
   
    //  Look in cache and continue with the cached resource manager wrapper, if one is found
    resourceManagerSet = s_resourceManagerWrappersCache[key] as ResourceManagerSet;
    
    if (resourceManagerSet != null)
      return resourceManagerSet;

    //  Not found in cache, get new resource managers

    resourceManagers.AddRange (GetResourceManagers (definingType.Assembly, resourceAttributes));

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

        GetResourceNameAndType (currentType, true, out currentType, out resourceAttributes);

        //  No more base types defining the MultiLingualResourcesAttribute
        if (currentType != null)
        {
          //  Insert the found resources managers at the beginning of the list
          resourceManagers.InsertRange (
              0,
              GetResourceManagers (currentType.Assembly, resourceAttributes));
        }
      }
    }

    //  Create a new resource mananger wrapper set and put it into the cache.

    ResourceManager[] resourceManagerArray = (ResourceManager[]) resourceManagers.ToArray (typeof (ResourceManager));
    resourceManagerSet = ResourceManagerWrapper.CreateWrapperSet (resourceManagerArray);

    lock (s_resourceManagerWrappersCache)
    {
      s_resourceManagerWrappersCache[key] = resourceManagerSet;
    }

    return resourceManagerSet;
  }
  /// <summary>
  ///   Returns an <b>ResourceManager</b> array for the resource containers specified through the 
  ///   <paramref name="resourceAttributes"/>.
  /// </summary>
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceManagers/*' />
  private static ResourceManager[] GetResourceManagers (
      Assembly assembly,
      MultiLingualResourcesAttribute[] resourceAttributes)
  {
    ResourceManager[] resourceManagers = new ResourceManager [resourceAttributes.Length];

    //  Load the ResourceManagers for the type's resources

    for (int index = 0; index < resourceAttributes.Length; index++)
    {
      Assembly resourceAssembly = resourceAttributes[index].ResourceAssembly;
      if (resourceAssembly == null)
        resourceAssembly = assembly;
      string key = resourceAttributes[index].BaseName + " in " + resourceAssembly.FullName;

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
            string baseName = resourceAttributes[index].BaseName;
            resourceManagers[index] = new ResourceManager (baseName, resourceAssembly);
            if (resourceManagers[index] == null) throw new ResourceException ("No resource with name '" + baseName + "' found in assembly '" + resourceAssembly.FullName + "'.");

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
      MultiLingualResourcesAttribute[] resourceAttributes;

      GetResourceNameAndType (objectTypeToGetResourceFor, false, out definingType, out resourceAttributes);

      if (GetResourceManagers (definingType.Assembly, resourceAttributes).Length > 0)
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
  /// Finds the class where the <b>MultiLingualResourcesAttribute</b> was defined and returns
  /// its type and all the <b>MultiLingualResourcesAttribute</b> instances.
  /// </summary>
  /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/Class/GetResourceNameAndType/*' />
  private static void GetResourceNameAndType (
      Type concreteType,
      bool noUndefinedException,
      out Type definingType,
      out MultiLingualResourcesAttribute[] resourceAttributes)
  {
    Type type = concreteType;
    resourceAttributes = GetResourceAttributes (type);

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
