using System;
using System.Web.UI.Design;

namespace Rubicon.ObjectBinding.Web.Design
{

/// <summary>
///   A desinger that requries the complete loading of the control.
/// </summary>
public class BocDesigner: ControlDesigner
{
  public override bool DesignTimeHtmlRequiresLoadComplete
  {
    get { return true; }
  }

}

}
