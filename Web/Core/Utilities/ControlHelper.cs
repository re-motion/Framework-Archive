using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Utilities;

namespace Rubicon.Web.Utilities
{

/// <summary>
/// Summary description for ClassName.
/// </summary>
/// <remarks>
/// 
/// </remarks>
public class ControlHelper
{
  // types

  // static members

  public static Control[] GetControlsRecursive (Control parentControl, Type type)
  {
    return GetControlsRecursive (parentControl, type, null);
  }

  public static Control[] GetControlsRecursive (Control parentControl, Type type, Control[] stopList)
  {
    ArrayList controlList = new ArrayList ();
    ArrayList stopListArray = stopList == null ? new ArrayList () : new ArrayList (stopList);
    GetControlsRecursiveInternal (parentControl, type, stopListArray, controlList);
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

  // member fields

  // construction and disposing

  // methods and properties

}

}





