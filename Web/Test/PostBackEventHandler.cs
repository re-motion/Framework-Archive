using System;
using System.Web.UI;

namespace Rubicon.Web.Test
{
  public class PostBackEventHandler : Control, IPostBackEventHandler
  {
    public event EventHandler<IDEventArgs> PostBack;

    public void RaisePostBackEvent (string eventArgument)
    {
      if (PostBack != null)
        PostBack (this, new IDEventArgs (eventArgument));
    }
  }
}