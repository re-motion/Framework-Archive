using System;

namespace Rubicon.Web.UI.Controls
{
public interface INavigablePage
{
  bool AllowImmediateClose { get; }
  bool CleanupOnImmediateClose { get; }
  bool NavigationRequest (string url);
  bool AutoDeleteSessionVariables { get; }
  void NavigateTo (string url, bool returnToThisPage);
  string Token{ get; }
}
}
