using System;
using System.Web.UI;
using System.Collections;

namespace Rubicon.Findit.Client.Controls
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
    ArrayList controlList = new ArrayList ();
    GetControlsRecursive (parentControl, type, controlList);
    return controlList;
  }

  private static void GetControlsRecursive (Control parentControl, Type type, ArrayList controlList)
  {
    foreach (Control control in parentControl.Controls)
    {
      if (control.GetType() == type)
        controlList.Add (control);
      
      GetControlsRecursive (control, type, controlList);
    }
  }
  
  // member fields

  // construction and disposing

  // methods and properties

}

}





