using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Utilities;

namespace Rubicon.Web.Utilities
{

public class ControlHelper
{
  // types

  // static members

  public static string EventTarget
  { get { return "__EVENTTARGET";} }
  
  public static string EventArgument
  { get { return "__EVENTARGUMENT"; } }
  
  public static string ViewState
  { get { return "__VIEWSTATE"; } }

  public static Control[] GetControlsRecursive (Control parentControl, Type type)
  {
    ArrayList controlList = new ArrayList ();
    GetControlsRecursiveInternal (parentControl, type, controlList);
    return (Control[]) controlList.ToArray (type);
  }

  public static Control[] GetControlsRecursive (Control parentControl, Type type, Control[] stopList)
  {
    ArrayList controlList = new ArrayList ();
    GetControlsRecursiveInternal (parentControl, type, new ArrayList (stopList), controlList);
    return (Control[]) controlList.ToArray (type);
  }

  private static void GetControlsRecursiveInternal 
      (Control parentControl, Type type, ArrayList stopList, ArrayList controlList)
  {
    ControlCollection controls = parentControl.Controls;
    for (int i = 0; i < controls.Count; ++i)
    {
      Control control = controls[i];
      if (!stopList.Contains (control))
      {
        if (type.IsInstanceOfType (control))
          controlList.Add (control);
        
        GetControlsRecursiveInternal (control, type, stopList, controlList);
      }
    }
  }

  private static void GetControlsRecursiveInternal 
      (Control parentControl, Type type, ArrayList controlList)
  {
    ControlCollection controls = parentControl.Controls;
    for (int i = 0; i < controls.Count; ++i)
    {
      Control control = controls[i];
      if (type.IsInstanceOfType (control))
        controlList.Add (control);
      
      GetControlsRecursiveInternal (control, type, controlList);
    }
  }
  
  public static bool ValidateOrder (BaseValidator smallerValidator, BaseValidator largerValidator, Type type)
  {
    TextBox smallerField = smallerValidator.NamingContainer.FindControl (smallerValidator.ControlToValidate) as TextBox;
    if (smallerField == null) throw new ArgumentException ("ControlToValidate must be TextBox", "smallerValidator");
    TextBox largerField = largerValidator.NamingContainer.FindControl (largerValidator.ControlToValidate) as TextBox;
    if (largerField == null) throw new ArgumentException ("ControlToValidate must be TextBox", "largerValidator");

    if (smallerField.Text.Trim() == string.Empty || largerField.Text.Trim() == string.Empty)
      return true;

    smallerValidator.Validate();
    largerValidator.Validate();
    if (! (smallerValidator.IsValid && largerValidator.IsValid) )
      return true;

    IComparable smallerValue = (IComparable) Convert.ChangeType (smallerField.Text, type);
    IComparable largerValue = (IComparable) Convert.ChangeType (largerField.Text, type);
        
    if (smallerValue.CompareTo (largerValue) > 0)
      return false;
    else
      return true;
  }

  /// <summary>
  ///   This method returns the nearest containing Template Control (i.e., Page or User Control).
  /// </summary>
  public static TemplateControl GetParentTemplateControl (Control control)
  {
    for (Control parent = control;
         parent != null;
         parent = parent.Parent)
    {
      if (parent is TemplateControl)
        return (TemplateControl) parent;
    }
    return null;
  }

  /// <summary>
  ///   This method returns <see langword="true"/> if the <paramref name="control"/> is in 
  ///   design mode.
  /// </summary>
  /// <remarks>
  ///   Does not verify the control's context.
  /// </remarks>
  /// <param name="control"> 
  ///   The <see cref="Control"/> to be tested for being in design mode. 
  /// </param>
  /// <returns> 
  ///   Returns <see langword="true"/> if the <paramref name="control"/> is in design mode.
  /// </returns>
  public static bool IsDesignMode (Control control)
  {
    return   (control.Site != null && control.Site.DesignMode)
          || (control.Page != null && control.Page.Site != null && control.Page.Site.DesignMode);
  }

  /// <summary>
  ///   This method returns <see langword="true"/> if the <paramref name="control"/> is in 
  ///   design mode.
  /// </summary>
  /// <param name="control"> 
  ///   The <see cref="Control"/> to be tested for being in design mode. 
  /// </param>
  /// <param name="context"> 
  ///   The <see cref="HttpContext"/> of the <paramref name="control"/>. 
  /// </param>
  /// <returns> 
  ///   Returns <see langword="true"/> if the <paramref name="control"/> is in design mode.
  /// </returns>
  public static bool IsDesignMode (IControl control, HttpContext context)
  {
    return context == null || ControlHelper.IsDesignMode (control);
  }

  /// <summary>
  ///   This method returns <see langword="true"/> if the <paramref name="control"/> is in 
  ///   design mode.
  /// </summary>
  /// <remarks>
  ///   Does not verify the control's context.
  /// </remarks>
  /// <param name="control"> 
  ///   The <see cref="IControl"/> to be tested for being in design mode. 
  /// </param>
  /// <returns> 
  ///   Returns <see langword="true"/> if the <paramref name="control"/> is in design mode.
  /// </returns>
  public static bool IsDesignMode (IControl control)
  {
    return   (control.Site != null && control.Site.DesignMode)
          || (control.Page != null && control.Page.Site != null && control.Page.Site.DesignMode);
  }

  public static Control FindControl (Control namingContainer, string controlID)
  {
    ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);

    try
    {
      //  WORKAROUND: In Designmode the very first call to FindControl results in a duplicate entry.
      //  Once that initial confusion has passed, everything seems to work just fine.
      //  Reason unknown (bug in Rubicon-code or bug in Framework-code)
      return namingContainer.FindControl (controlID);
    }
    catch (HttpException)
    {
      if (ControlHelper.IsDesignMode (namingContainer))
        return namingContainer.FindControl (controlID);
      else
        throw;
    }
  }

  public static object GetDesignTimePropertyValue (Control control, string propertyName)
  {
    if (! IsDesignMode (control))
      return null;

    try
    {
      ISite site = control.Site;

      //EnvDTE._DTE environment = (EnvDTE._DTE) site.GetService (typeof (EnvDTE._DTE));
      MethodInfo getServiceMethod = site.GetType().GetMethod ("GetService");
      Type _DTEType = Type.GetType ("EnvDTE._DTE, EnvDTE");
      object[] arguments = new object[] { _DTEType };
      object environment = getServiceMethod.Invoke (site, arguments);

      if (environment != null)
      {
        //EnvDTE.Project project = environment.ActiveDocument.ProjectItem.ContainingProject;
        object activeDocument =_DTEType.InvokeMember ("ActiveDocument", BindingFlags.GetProperty, null, environment, null);
        object projectItem = activeDocument.GetType().InvokeMember ("ProjectItem", BindingFlags.GetProperty, null, activeDocument, null);
        object project = projectItem.GetType().InvokeMember ("ContainingProject", BindingFlags.GetProperty, null, projectItem, null);

        ////project.Properties uses a 1-based index
        //foreach (EnvDTE.Property property in project.Properties)
        object properties = project.GetType().InvokeMember ("Properties", BindingFlags.GetProperty, null, project, null);
        IEnumerator propertiesEnumerator = (IEnumerator) properties.GetType().InvokeMember ("GetEnumerator", BindingFlags.InvokeMethod, null, properties, null);
        while (propertiesEnumerator.MoveNext())
        {
          object property = propertiesEnumerator.Current;

          //if (property.Name == propertyName)
          string projectPropertyName = (string) property.GetType().InvokeMember ("Name", BindingFlags.GetProperty, null, property, null);
          if (projectPropertyName == propertyName)
          {
            //return property.Value;
            return property.GetType().InvokeMember ("Value", BindingFlags.GetProperty, null, property, null);
          }
        }
      }
    }
    catch
    {
      return null;
    }

    return null;
  }

  /// <summary> Encapsulates the invokation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
  /// <param name="target"> The <see cref="Control"/> to be restored. </param>
  /// <param name="viewState"> The view state object used for restoring. </param>
  public static void LoadViewStateRecursive (Control target, object viewState)
  {
    const BindingFlags bindingFlags = BindingFlags.DeclaredOnly 
                                    | BindingFlags.Instance 
                                    | BindingFlags.NonPublic
                                    | BindingFlags.InvokeMethod;

    //  HACK: Reflection on internal void Control.LoadViewStateRecursive (object)
    //  internal void System.Web.UI.Control.LoadViewStateRecursive (object)
    typeof (Control).InvokeMember (
      "LoadViewStateRecursive",
      bindingFlags,
      null,
      target,
      new object[] {viewState});
  }

  /// <summary> Encapsulates the invokation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
  /// <param name="target"> The <see cref="Control"/> to be saved. </param>
  /// <returns> The view state object for <paramref name="target"/>. </returns>
  public static object SaveViewStateRecursive (Control target)
  {
    const BindingFlags bindingFlags = BindingFlags.DeclaredOnly 
                                    | BindingFlags.Instance 
                                    | BindingFlags.NonPublic
                                    | BindingFlags.InvokeMethod;

    //  HACK: Reflection on internal object Control.SaveViewStateRecursive()
    //  internal object System.Web.UI.Control.LoadViewStateRecursive()
    object viewState = typeof (Control).InvokeMember (
        "SaveViewStateRecursive",
        bindingFlags,
        null,
        target,
        new object[] {});

    return viewState;
  }

  // member fields

  // construction and disposing

  // methods and properties

}

}
