using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.Design;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   This control can be used to display or edit reference values.
/// </summary>
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocReferenceValue//: //BusinessObjectBoundModifiableWebControl //, IPostBackDataHandler
{
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };


}

}
