using System;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public class WxeContext
{
//  [ThreadStatic]
//  private static WxeContext _current;

  public static WxeContext Current
  {
    // get { return _current; }
    get { return System.Runtime.Remoting.Messaging.CallContext.GetData ("WxeContext") as WxeContext; }
  }

  internal static void SetCurrent (WxeContext value)
  {
    System.Runtime.Remoting.Messaging.CallContext.SetData ("WxeContext", value);
    // _current = value; 
  }

  private HttpContext _httpContext;
  private bool _isPostBack;
  private bool _isReturningPostBack;
  private NameValueCollection _postBackCollection;
  private string _functionToken = null;
  private WxeFunction _returningFunction = null;

  public WxeContext (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);

    _httpContext = context;
    _isPostBack = false;
    _isReturningPostBack = false;
    _postBackCollection = null;
  }

  public HttpContext HttpContext
  {
    get { return _httpContext; }
  }

  public bool IsPostBack
  {
    get { return _isPostBack; }
    set { _isPostBack = value; }
  }

  /// <summary>
  /// During the execution of a page, specifies whether the current postback cycle was caused by returning from a WXE function.
  /// </summary>
  public bool IsReturningPostBack 
  {
    get { return _isReturningPostBack; }
    set { _isReturningPostBack = value; }
  }

  public NameValueCollection PostBackCollection
  {
    get { return _postBackCollection; }
    set { _postBackCollection = value; }
  }

  public string FunctionToken
  {
    get { return _functionToken; }
    set { _functionToken = value; }
  }

  public WxeFunction ReturningFunction 
  {
    get { return _returningFunction; }
    set { _returningFunction = value; }
  }

  /// <summary>
  ///   Gets the URL that resumes the current function.
  /// </summary>
  /// <remarks>
  ///   If a WXE application branches to an external web site, the external site can
  ///   link back to this URL to resume the current function at the point where 
  ///   it was interrupted. Note that if the user stays on the external site longer
  ///   that the session or function timeout, resuming will fail with a timeout
  ///   exception.
  /// </remarks>
  public string GetResumeUrl ()
  {
    return HttpContext.Request.Url.GetLeftPart (UriPartial.Path) + "?WxeFunctionToken=" + FunctionToken;
  }
}

}
