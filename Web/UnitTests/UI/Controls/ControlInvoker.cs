using System;
using System.Web.UI;

using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls
{

public class ControlInvoker
{
  // types

  // static members and constants

  // member fields
  
  private Control _control;

  // construction and disposing

  public ControlInvoker (Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    _control = control;
  }

  // methods and properties

  public Control Control
  {
    get { return _control; }
  }

  public void InitRecursive ()
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, "InitRecursive", new object[] { null });
  }

  public void LoadRecursive ()
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, "LoadRecursive", new object[0]);
  }

  public void PreRenderRecursive ()
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, "PreRenderRecursiveInternal", new object[0]);
  }
}

}
