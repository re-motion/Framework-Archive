using System;
using System.Globalization;
using System.Resources;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using log4net;

using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Globalization
{
/// <summary>
///   Provides methods for dispatching the resources inside an IResourceManager container
///   to a control.
/// </summary>
/// <include file='doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/Class/example' />
public sealed class ResourceDispatcher
{
  // types

  // static members and constants

  /// <summary> Use this ID to dispatch resources to the control that provides the resource manager. </summary>
  private const string c_thisElementID = "this";

	private static readonly ILog s_log = LogManager.GetLogger (typeof (ResourceDispatcher));
  private static ArrayList _registeredDispatchTargets = new ArrayList();

  /// <summary>
  ///   Dispatches resources.
  /// </summary>
  /// <include file='doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/Dispatch/remarks' />
  /// <param name="control">
  ///   The control for which resources are to be dispatched. Must not be <see langname="null"/>.
  /// </param>
  /// <param name="resourceManager">
  ///   The resource manager to be used. Must not be <see langname="null"/>.
  /// </param>  
  public static void Dispatch (Control control, IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
 
    const string prefix = "auto:";

    if (ControlHelper.IsDesignMode (control))
      return;

    IDictionary autoElements = ResourceDispatcher.GetResources (resourceManager, prefix);

    ResourceDispatcher.Dispatch (control, autoElements, resourceManager.Name);
  }

  /// <summary>
  ///   Dispatches resources provided by <see cref="IObjectWithResources.GetResourceManager()"/>
  /// </summary>
  /// <param name="control">
  ///   The control for which resources are to be dispatched. Must not be <see langname="null"/>.
  ///   The control and/or one or more of its parents must implement <see cref="IObjectWithResources"/>.
  /// </param>
  /// <param name="throwExceptionIfNoResources"> If true and neither the control nor its parents
  ///   define a resource manager, an InvalidOperationException is thrown. </param>
  public static void Dispatch (Control control, bool throwExceptionIfNoResources)
  {
    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (control, false);
    if (resourceManager == null)
    {
      if (throwExceptionIfNoResources)
        throw new InvalidOperationException ("Control " + control.UniqueID + " has no resource managers.");
      else
        return;
    }
    Dispatch (control, resourceManager);
  }

  /// <summary>
  ///   Dispatches an IDictonary of elementID/IDictonary pairs to the specified control.
  /// </summary>
  /// <include file='doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/DispatchMain/*' />
  public static void Dispatch (
      Control control,
      IDictionary elements,
      string resourceSource)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    ArgumentUtility.CheckNotNull ("elements", elements);

    if (ControlHelper.IsDesignMode (control))
      return;

    //  Dispatch the resources to the controls
    foreach (DictionaryEntry elementsEntry in elements)
    {
      string elementID = (string) elementsEntry.Key;

      Control targetControl = null;

      if (elementID == c_thisElementID)
        targetControl = control;
      else
        targetControl = control.FindControl (elementID);

      if (targetControl == null)
      {
        s_log.Warn ("Control '" + control.ToString() + "': No child-control with ID '" + elementID + "' found. ID was read from \"" + resourceSource + "\".");
      }
      else
      {
        //  Pass the value to the control
        IDictionary values = (IDictionary) elementsEntry.Value;
        IResourceDispatchTarget resourceDispatchTarget = targetControl as IResourceDispatchTarget;

        if (resourceDispatchTarget != null) //  Control knows how to dispatch
          resourceDispatchTarget.Dispatch (values);       
        else
          ResourceDispatcher.DispatchGeneric (targetControl, values);
      }
    }
  }

  /// <summary>
  ///   Dispatches the resources passed in <paramref name="values"/> to the properties of <paramref name="obj"/>.
  /// </summary>
  /// <include file='doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/DispatchGeneric/*' />
  public static void DispatchGeneric (object obj, IDictionary values)
  {
    ArgumentUtility.CheckNotNull ("obj", obj);
    ArgumentUtility.CheckNotNull ("values", values);

    foreach (DictionaryEntry entry in values)
    {
      string propertyName = (string) entry.Key;
      string propertyValue = (string) entry.Value;

      PropertyInfo property = obj.GetType ().GetProperty (propertyName, typeof (string));
      if (property != null)
      {
        property.SetValue (obj, propertyValue, new object[0]); 
      }
      else if (obj is Control)
      {
        Control control = (Control) obj;
        //  Test for HtmlControl, they can take anything
        HtmlControl genericHtmlControl = control as HtmlControl;
        if (genericHtmlControl != null)
          genericHtmlControl.Attributes[propertyName] = propertyValue;
        else //  Non-HtmlControls require valid property
          s_log.Warn ("Control '" + control.ID + "' of type '" + control.GetType().FullName + "' does not contain a public property '" + propertyName + "'.");
      }
    }
  }

  /// <summary>
  ///   Selects all resources matching the <c>prefix</c> into a HashTable.
  /// </summary>
  /// <param name="resourceManager">
  ///   The <see cref="IResourceManager"/> to select from. Must not be <see langname="null"/>.
  /// </param>
  /// <param name="prefix">
  ///   The filter prefix, can be empty. Must not be <see langname="null"/>.
  /// </param>
  /// <returns>
  ///   Hashtable&lt;string elementID, IDictionary&lt;string property, string value&gt; elementValues&gt;
  /// </returns>
  private static IDictionary GetResources (IResourceManager resourceManager, string prefix)
  {
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

    if (prefix == null)
      prefix = String.Empty;

    // Hashtable<string elementID, IDictionary<string property, string value> elementValues>
    IDictionary elements = new Hashtable (); 

    NameValueCollection resources = resourceManager.GetAllStrings (prefix);

    for (int index = 0; index < resources.Count; index++)
    {
      //  Compound key: "prfx:elementID:argument"
      //  The argument (including the colon) is optional
      //  resources contain only keys with the prefix "auto" because of the applied filter

      string key = resources.GetKey(index);

      //  Remove the prefix and colon
      key = key.Substring (prefix.Length);

      //  Test for a second colon in the key
      int posColon = key.IndexOf (':');

      if (posColon >= 0)
      {
        //  If one is found, this indicates an argument attached to the elementID
  
        string elementID = key.Substring (0, posColon);
        string property = key.Substring (posColon + 1);

        //  Now there can be more than one argument provided for a specific element
        //  Create a dictonary for the element,
        //  using the argument as key and the resources' value as the value.

        //  Get the dictonary for the current element
        IDictionary elementValues = (IDictionary) elements[elementID];

        //  If no dictonary exists, create it and insert it into the elements hashtable.
        if (elementValues == null)
        {
          elementValues = new HybridDictionary ();
          elements[elementID] = elementValues;
        }

        //  Insert the argument and resource's value into the dictonary for the specified element.
        elementValues.Add (property, resources[index]);
      }
    }

    return elements;
  }

  /// <summary>
  ///   Dispatch a value to a single property. Will be replaced using the reflection utility
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if propertyName is unknown in objectToSetPropertyFor
  /// </exception>
  // TODO: Replace using the reflection utility
  [Obsolete ("Use ReflectionUtility.SetPropertyOrFieldValue.")]
  public static void SetProperty (object objectToSetPropertyFor, string propertyName, string propertyValue)
  {
    ArgumentUtility.CheckNotNull ("objectToSetPropertyFor", objectToSetPropertyFor);
    ArgumentUtility.CheckNotNull ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    PropertyInfo property = objectToSetPropertyFor.GetType().GetProperty (propertyName, typeof (string));

    if (property == null)
      s_log.Warn ("Object of type '" + objectToSetPropertyFor.GetType().FullName + "' does not contain a public property '" + propertyName + "'.");

    property.SetValue (objectToSetPropertyFor, propertyValue, new object[0]);  
  }


  /// <summary>
  /// obsolete
  /// </summary>
  /// <param name="control"></param>
  /// <param name="resourceManager"></param>
  [Obsolete ("Use Dispatch (Control, IResourceManager) instead.")]
  public static void Dispatch (Control control, ResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

    ResourceDispatcher.Dispatch (control, new ResourceManagerWrapper (resourceManager));
  }

  //  construction and disposing
  
  private ResourceDispatcher ()
  {
  }
}

}