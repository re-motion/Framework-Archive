using System;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace Rubicon.Findit.Globalization.Classes
{

public sealed class ResourceManagerPool
{
  // static members

  private static Hashtable s_resourceManagerCache = new Hashtable ();

  private static void GetResourceNameAndType (Type concreteType, out Type definingType, out string resourceName)
  {
    Type type = concreteType;
    MultiLingualResourcesAttribute[] resourceAttributes = GetResourceAttributes (type);

    while (type != null && resourceAttributes.Length == 0) 
    {
      type = type.BaseType;
      if (type == null)
        throw new ResourceException ("Type " + concreteType.FullName + " and its base classes do not define the attribute MultiLingualResourcesAttribute.");

      resourceAttributes = GetResourceAttributes (type);
    } 

    definingType = type;
    resourceName = resourceAttributes[0].ResourceName;
  }

  private static MultiLingualResourcesAttribute[] GetResourceAttributes (Type type)
  {
    return (MultiLingualResourcesAttribute[]) 
        type.GetCustomAttributes (typeof (MultiLingualResourcesAttribute), false);
  }

  public static bool ExistsResource (Type objectTypeToGetResourceFor)
  {
    ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
    try
    {
      return GetResourceSet (objectTypeToGetResourceFor) != null;
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

  public static bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
  {
    try
    {
      string text = GetResourceText (objectTypeToGetResourceFor, name);
      return (text != null);
    }
    catch
    {
      return false;
    }
  }
  
  public static bool ExistsResourceText (object objectToGetResourceFor, string name)
  {
    try
    {
      string text = GetResourceText (objectToGetResourceFor, name);
      return (text != null);
    }
    catch
    {
      return false;
    }  
  }
  
  public static string GetResourceText (Type objectTypeToGetResourceFor, string name)
  {
    Type definingType;
    string resourceName;
    ResourceManager rm = GetOrCreateResourceManager (objectTypeToGetResourceFor, out definingType, out resourceName);

    string text = rm.GetString (name, GetUICulture ());

    if (text == null)
      throw new ResourceException (string.Format ("The resource '{0}' in '{1}' could not be found", name, resourceName));

    return text;
  }

  public static string GetResourceText (object objectToGetResourceFor, string name)
  {
    ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);

    return GetResourceText (objectToGetResourceFor.GetType(), name);  
  }

  /*
  private static string GetResourceName (Type objectType)
  {
    MultiLingualResourcesAttribute[] resourceAttributes = (MultiLingualResourcesAttribute[]) objectType.GetCustomAttributes (
      typeof (MultiLingualResourcesAttribute), false);

    if (resourceAttributes.Length == 0)
      throw new ResourceException ("Cannot dispatch resources for object types that do not have the MultiLingualResources attribute.");
     
    return resourceAttributes[0].ResourceName;
  }
  */

  public static ResourceSet GetResourceSet (Type objectType)
  {
    ResourceManager rm = GetOrCreateResourceManager (objectType);
    return GetResourceSet (rm, true);
  }

  public static ResourceSet GetResourceSet (ResourceManager resourceManager, bool throwExceptionIfNotFound)
  {
    Exception innerException = null;
    ResourceSet resourceSet = null;
    try
    {
      resourceSet = resourceManager.GetResourceSet (GetUICulture (), true, true);
    }
    catch (MissingManifestResourceException e)
    {
      innerException = e;
    }
    if (throwExceptionIfNotFound && resourceSet == null)
      throw new ResourceException (string.Format ("No resource set in culture {0} found for resource {1}.", GetUICulture().Name, resourceManager.BaseName), innerException);
    return resourceSet;
  }

  public static ResourceManager GetOrCreateResourceManager (Type objectType)
  {
    Type definingType;
    string resourceName;
    return GetOrCreateResourceManager (objectType, out definingType, out resourceName);
  }

  public static ResourceManager GetOrCreateResourceManager (Type objectType, out Type definingType, out string resourceName)
  {
    GetResourceNameAndType (objectType, out definingType, out resourceName);

    Assembly assembly = definingType.Assembly;
    string key = resourceName + " in " + assembly.FullName;

    ResourceManager resourceManager = (ResourceManager) s_resourceManagerCache[key];
    if (resourceManager != null)
      return resourceManager;

    resourceManager = new ResourceManager (resourceName, assembly);
    if (resourceManager == null)
      throw new ResourceException ("No resource with name " + resourceName + " found in assembly \"" + assembly.FullName + "\".");

    lock (s_resourceManagerCache)
    {
      s_resourceManagerCache[key] = resourceManager;
    }

    return resourceManager;
  }

  /*
  private static ResourceManager GetOrCreateResourceManager (Type objectType, string resourceName)
  {
    if (s_resourceManagerCache.ContainsKey (resourceName))
    {
      return (ResourceManager) s_resourceManagerCache[resourceName];
    }
    else
    {
      ResourceManager rm = new ResourceManager (resourceName, objectType.Assembly);
      if (rm == null)
        throw new ResourceException ("No resource with name " + resourceName + " found.");

      lock (typeof (ResourceDispatcher))
      {
        s_resourceManagerCache[resourceName] = rm;
      }

      return rm;
    }
  }
  */

  private static CultureInfo GetUICulture ()
  {
    return Thread.CurrentThread.CurrentUICulture;
  }
  
  // construction and disposal

  private ResourceManagerPool()
  {
  }
}

[AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class MultiLingualResourcesAttribute: Attribute
{
  private string _resourceName;

  public MultiLingualResourcesAttribute (string resourceName)
  {
    _resourceName = resourceName;
  }

  public string ResourceName 
  {
    get { return _resourceName; }
  }
}

[Serializable]
public class ResourceException: Exception
{
  public ResourceException (string message)
    : base (message)
  {
  }

  public ResourceException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  public ResourceException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
