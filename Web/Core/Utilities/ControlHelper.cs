using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Rubicon.Web.UI.Utilities
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

  public static ArrayList GetControlsRecursive (Control parentControl, Type type)
  {
    return GetControlsRecursive (parentControl, type, null);
  }

  public static ArrayList GetControlsRecursive (Control parentControl, Type type, Control[] stopList)
  {
    ArrayList controlList = new ArrayList ();
    ArrayList stopListArray = stopList == null ? new ArrayList () : new ArrayList (stopList);
    GetControlsRecursiveInternal (parentControl, type, stopListArray, controlList);
    return controlList;
  }

  private static void GetControlsRecursiveInternal 
      (Control parentControl, Type type, ArrayList stopList, ArrayList controlList)
  {
    foreach (Control control in parentControl.Controls)
    {
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

  // member fields

  // construction and disposing

  // methods and properties

}

}





