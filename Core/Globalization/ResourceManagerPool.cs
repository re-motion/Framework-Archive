using System;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

using Rubicon.Findit.Globalization.Globalization;

namespace Rubicon.Findit.Globalization.Classes
{

public sealed class ResourceDispatcher
{
  // types

  // static members and constants

  private static Hashtable s_resourceManagerCache = new Hashtable ();

  public static string GetResourceText (Type objectTypeToGetResourceFor, string name)
  {
    return GetResourceText (null, objectTypeToGetResourceFor, GetResourceName (objectTypeToGetResourceFor), name);
  }

  public static string GetResourceText (object objectToGetResourceFor, string name)
  {
    Type type = GetType (objectToGetResourceFor);

    return GetResourceText (objectToGetResourceFor, type, GetResourceName (type), name);  
  }

  public static string GetResourceText (
      object objectToGetResourceFor, 
      Type objectType, 
      string resourceName, 
      string name)
  {
    ResourceManager rm = GetOrCreateResourceManager (objectType, resourceName);


    string text = rm.GetString (name, MultiLingualUtility.GetUICulture ());

    if (text == null)
      text = "###";

    return text;
  }

  public static void Dispatch (Control control)
  {
    Dispatch (control, GetType (control));
  }

  public static void Dispatch (Control control, Type controlType)
  {
    Dispatch (control, controlType, GetResourceName (controlType));
  }

  public static void Dispatch (Control control, Type controlType, string resourceName)
  {
    // Hashtable<string elementID, IDictionary<string argument, string value> elementValues>
    IDictionary elements = new Hashtable (); 

    ResourceSet resources = GetResourceSet (controlType, resourceName);

    foreach (DictionaryEntry resourceEntry in resources)
    {
      string key = (string) resourceEntry.Key;
      if (key.StartsWith ("auto:"))
      {
        key = key.Substring (5);
        int posColon = key.IndexOf (':');
        if (posColon >= 0)
        {
          string elementID = key.Substring (0, posColon);
          string argument = key.Substring (posColon + 1);

          IDictionary elementValues = (IDictionary) elements[elementID];
          if (elementValues == null)
          {
            elementValues = new HybridDictionary ();
            elements[elementID] = elementValues;
          }
          elementValues.Add (argument, resourceEntry.Value);
        }
      }
    }

    foreach (DictionaryEntry elementsEntry in elements)
    {
      string elementID = (string) elementsEntry.Key;
      Control childControl = control.FindControl (elementID);
      if (childControl == null)
        throw new ApplicationException ("No control with ID " + elementID + " found. ID was read from resource " + resourceName + ".");

      IDictionary values = (IDictionary) elementsEntry.Value;
      IResourceDispatchTarget resourceDispatchTarget = childControl as IResourceDispatchTarget;
      if (resourceDispatchTarget != null)
        resourceDispatchTarget.Dispatch (values);
      else
        DispatchGeneric (childControl, values);
    }
  }

  public static void DispatchGeneric (Control control, IDictionary values)
  {
    foreach (DictionaryEntry entry in values)
    {
      string propertyName = (string) entry.Key;
      string propertyValue = (string) entry.Value;

      HtmlControl genericHtmlControl = control as HtmlControl;
      if (genericHtmlControl != null)
      {
        genericHtmlControl.Attributes[propertyName] = propertyValue;
      }
      else  
      {
        PropertyInfo property = control.GetType().GetProperty (propertyName, typeof (string));

        if (property == null)
        {
          throw new ApplicationException ("Type " + control.GetType().FullName + " does not contain a public property " + propertyName + ".");
        }

        property.SetValue (control, propertyValue, new object[0]);
      }
    }
  }

  private static Type GetType (object objectToGetTypeFor)
  {
    Type type = objectToGetTypeFor.GetType();
    
    if (type != null && 
        typeof(Control).IsAssignableFrom (type) && 
        type.Namespace == "ASP" && 
        (type.Name.EndsWith ("_aspx") || type.Name.EndsWith ("_ascx")))
    {
      type = type.BaseType;
    }

    return type;
  }

  private static string GetResourceName (Type objectType)
  {
    MultiLingualResourcesAttribute[] resourceAttributes = (MultiLingualResourcesAttribute[]) objectType.GetCustomAttributes (
      typeof (MultiLingualResourcesAttribute), false);

    if (resourceAttributes.Length == 0)
      throw new ApplicationException ("Cannot dispatch resources for object types that do not have the MultiLingualResources attribute.");
     
    return resourceAttributes[0].ResourceName;
  }

  private static ResourceSet GetResourceSet (Type objectType, string resourceName)
  {
    ResourceManager rm = GetOrCreateResourceManager (objectType, resourceName);

    return rm.GetResourceSet (MultiLingualUtility.GetUICulture (), true, true);
  }

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
        throw new ApplicationException ("No resource with name " + resourceName + " found.");

      lock (typeof (ResourceDispatcher))
      {
        s_resourceManagerCache[resourceName] = rm;
      }

      return rm;
    }

    /*
    if (HttpContext.Current.Application[resourceName] != null)
    {
      return (ResourceManager) HttpContext.Current.Application[resourceName];
    }
    else
    {
      ResourceManager rm = new ResourceManager (resourceName, controlType.Assembly);
      if (rm == null)
        throw new ApplicationException ("No resource with name " + resourceName + " found.");

      HttpContext.Current.Application[resourceName] = rm;

      return rm;
    }
    */
  }

  // member fields

  // construction and disposing

  private ResourceDispatcher()
  {
  }

  // methods and properties

}

public interface IResourceDispatchTarget
{
  void Dispatch (IDictionary values);
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
}
