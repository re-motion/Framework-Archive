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

using Rubicon.Findit.Globalization.Classes;

namespace Rubicon.Findit.Client.Controls
{

public sealed class ResourceDispatcher
{
  // types

  // static members and constants

  public static void Dispatch (Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    ResourceManager resourceManager = ResourceManagerPool.GetOrCreateResourceManager (control.GetType());
    Dispatch (control, resourceManager);
  }

  public static void Dispatch (Control control, Type controlType)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    ResourceManager resourceManager = ResourceManagerPool.GetOrCreateResourceManager (controlType);
    Dispatch (control, resourceManager);
  }

  public static void Dispatch (Control control, ResourceManager resourceManager)
  {
    // Hashtable<string elementID, IDictionary<string argument, string value> elementValues>
    IDictionary elements = new Hashtable (); 

    ResourceSet resources = ResourceManagerPool.GetResourceSet (resourceManager, true);

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
        throw new InvalidOperationException ("No control with ID " + elementID + " found. ID was read from resource " + resourceManager.BaseName + ".");

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

      string line = string.Format("Control: ID={0} {1}={2} ", control.ID, propertyName, propertyValue);
      Trace.WriteLine(line);
      HtmlControl genericHtmlControl = control as HtmlControl;
      if (genericHtmlControl != null)
      {
        genericHtmlControl.Attributes[propertyName] = propertyValue;
      }
      else  
      {
        SetProperty (control, propertyName, propertyValue);
      }
    }
  }

  public static void SetProperty (object objectToSetPropertyFor, string propertyName, string propertyValue)
  {
    PropertyInfo property = objectToSetPropertyFor.GetType().GetProperty (propertyName, typeof (string));

    if (property == null)
    {
      throw new InvalidOperationException ("Type " + objectToSetPropertyFor.GetType().FullName + " does not contain a public property " + propertyName + ".");
    }

    property.SetValue (objectToSetPropertyFor, propertyValue, new object[0]);  
  }
  
  /*
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
  */


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

}
