using System;

namespace Rubicon.Web.UI.Controls
{

public interface IControlStateManager
{
  void LoadControlState (object state);
  object SaveControlState();
}

}
