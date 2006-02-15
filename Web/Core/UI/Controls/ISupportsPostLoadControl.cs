using System;
using System.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Controls
{

/// <summary>
///   Allows controls to receive a call after the <see cref="Control.Load"/> event.
/// </summary>
public interface ISupportsPostLoadControl: IControl
{
  /// <summary>
  ///   This method may be called after the <see cref="Control.Load"/> event.
  /// </summary>
  void OnPostLoad();
}

}
