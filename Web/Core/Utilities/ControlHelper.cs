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
  
  // member fields

  // construction and disposing

  // methods and properties

}

}





