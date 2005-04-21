using System;
using System.Web.UI;

namespace Rubicon.Web.UI.Controls
{

public class PostBackHandlerEventArgs: EventArgs
{
  private string _eventArgument;

  public PostBackHandlerEventArgs (string eventArgument)
  {
    _eventArgument = eventArgument;
  }

  /// <summary>
  ///   The argument passed to 
  /// </summary>
  public string EventArgument
  {
    get { return _eventArgument; }
  }
}

public delegate void PostBackHandlerEventHandler (object sender, PostBackHandlerEventArgs e);

/// <summary>
///   Invisible control that receives the event created by <c>__doPostBack</c> and creates a server side event.
/// </summary>
public class PostBackHandler: Control, IPostBackEventHandler
{
  private static readonly object s_postBackEventt = new object();

  public event PostBackHandlerEventHandler PostBackEvent
  {
    add { Events.AddHandler (s_postBackEventt, value); }
    remove { Events.RemoveHandler (s_postBackEventt, value); }
  }

  void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
  {
    PostBackHandlerEventHandler handler = (PostBackHandlerEventHandler) Events[s_postBackEventt];
    if (handler != null)
      handler (this, new PostBackHandlerEventArgs (eventArgument));
  }

  protected override void Render (HtmlTextWriter writer)
  {
    // invisible control
  }

}

}
