using System;
using System.Web.UI;

namespace Rubicon.Web.UI
{

/// <summary>
///   Allows controls to receive a call after the <see cref="Control.Load"/> event.
/// </summary>
/// <remarks>
/// </remarks>
public interface ISupportsPostLoadControl: IControl
{
  /// <summary>
  ///   This method may be called after the <see cref="Control.Load"/> event.
  /// </summary>
  void OnPostLoad();
}

/// <summary>
///   Calls <see cref="ISupportsPostLoadControl.OnPostLoad"/> on all controls that support the interface.
/// </summary>
/// <remarks>
///   Children are called after their parents.
/// </remarks>
public class PostLoadInvoker
{
  public static void InvokePostLoad (Control control)
  {
    if (control is ISupportsPostLoadControl)
      ((ISupportsPostLoadControl)control).OnPostLoad ();

    ControlCollection controls = control.Controls;
    for (int i = 0; i < controls.Count; ++i)
    {
      Control childControl = controls[i];
      InvokePostLoad (childControl);
    }
  }

  private Control _control;
  private bool _invoked;

  public PostLoadInvoker (Control control)
  {
    _control = control;
    _invoked = false;
  }

  public void EnsurePostLoadInvoked ()
  {
    if (! _invoked)
    {
      InvokePostLoad (_control);
      _invoked = true;
    }
  }

}

}
