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
  private NameValueCollection _postBackCollection;

  public WxeContext (HttpContext context)
    : this (context, false)
  {
  }

  public WxeContext (HttpContext context, bool isPostBack)
  {
    ArgumentUtility.CheckNotNull ("context", context);

    _httpContext = context;
    _isPostBack = isPostBack;
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

  public NameValueCollection PostBackCollection
  {
    get { return _postBackCollection; }
    set { _postBackCollection = value; }
  }
}

}
