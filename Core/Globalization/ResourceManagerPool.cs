using System;
using System.Globalization;
using System.Resources;
using System.Web.UI;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace Rubicon.Findit.Globalization.Classes
{

public sealed class ResourceDispatcher
{
  public static string GetResourceText (Page page, string name)
  {
    return GetResourceText (page, GetPageType (page), name);  
  }

  public static string GetResourceText (Page page, Type pageType, string name)
  {
    return GetResourceText (page, pageType, GetResourceName (page, pageType), name);
  }

  public static string GetResourceText (Page page, Type pageType, string resourceName, string name)
  {
    ResourceManager rm = GetOrCreateResourceManager (page, pageType, resourceName);

    return rm.GetString (name, MultiLingualUtility.GetUICulture ());
  }

  public static void Dispatch (Page page)
  {
    Dispatch (page, GetPageType (page));
  }

  public static void Dispatch (Page page, Type pageType)
  {
    Dispatch (page, pageType, GetResourceName (page, pageType));
  }

  public static void Dispatch (Page page, Type pageType, string resourceName)
  {
    // Hashtable<string elementID, IDictionary<string argument, string value> elementValues>
    IDictionary elements = new Hashtable (); 

    // TODO: Read current CultureInfo from user, domain, ...
    ResourceSet resources = GetResourceSet (page, pageType, resourceName);

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
      Control control = page.FindControl (elementID);
      if (control == null)
        throw new ApplicationException ("No control with ID " + elementID + " found. ID was read from resource " + resourceName + ".");

      IDictionary values = (IDictionary) elementsEntry.Value;
      IResourceDispatchTarget resourceDispatchTarget = control as IResourceDispatchTarget;
      if (resourceDispatchTarget != null)
        resourceDispatchTarget.Dispatch (values);
      else
        DispatchGeneric (control, values);
    }
  }

  public static void DispatchGeneric (Control control, IDictionary values)
  {
    foreach (DictionaryEntry entry in values)
    {
      string propertyName = (string) entry.Key;
      string propertyValue = (string) entry.Value;

      PropertyInfo property = control.GetType().GetProperty (propertyName, typeof (string));
      if (property == null)
        throw new ApplicationException ("Type " + control.GetType().FullName + " does not contain a public property " + propertyName + ".");

      property.SetValue (control, propertyValue, new object[0]);
    }
  }

  private static Type GetPageType (Page page)
  {
    Type type = page.GetType();
    
    if (type != null && typeof(Page).IsAssignableFrom (type) && type.Namespace == "ASP" && type.Name.EndsWith ("_aspx"))
      type = type.BaseType;

    return type;
  }

  private static string GetResourceName (Page page, Type pageType)
  {
    PageResourcesAttribute[] resourceAttributes = (PageResourcesAttribute[]) pageType.GetCustomAttributes (
      typeof (PageResourcesAttribute), false);

    if (resourceAttributes.Length == 0)
      throw new ApplicationException ("Cannot dispatch resources for pages that do not have the PageResourcesAttribute attribute.");
     
    return resourceAttributes[0].ResourceName;
  }

  private static ResourceSet GetResourceSet (Page page, Type pageType, string resourceName)
  {
    ResourceManager rm = GetOrCreateResourceManager (page, pageType, resourceName);

    return rm.GetResourceSet (MultiLingualUtility.GetUICulture (), true, true);
  }

  private static ResourceManager GetOrCreateResourceManager (Page page, Type pageType, string resourceName)
  {
    if (page.Application[resourceName] != null)
    {
      return (ResourceManager) page.Application[resourceName];
    }
    else
    {
      ResourceManager rm = new ResourceManager (resourceName, pageType.Assembly);
      if (rm == null)
        throw new ApplicationException ("No resource with name " + resourceName + " found.");

      page.Application[page.ID] = rm;

      return rm;
    }
  }

	private ResourceDispatcher()
	{
	}
}

public interface IResourceDispatchTarget
{
  void Dispatch (IDictionary values);
}

[AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PageResourcesAttribute: Attribute
{
  private string _resourceName;

  public PageResourcesAttribute (string resourceName)
  {
    _resourceName = resourceName;
  }

  public string ResourceName 
  {
    get { return _resourceName; }
  }
}
}
