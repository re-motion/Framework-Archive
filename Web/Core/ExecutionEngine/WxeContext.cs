using System;
using System.ComponentModel;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
///   The <b>WxeContext</b> contains information about the current WXE execution cycle.
/// </summary>
public class WxeContext
{
//  [ThreadStatic]
//  private static WxeContext _current;

  public static WxeContext Current
  {
    // get { return _current; }
    get { return System.Runtime.Remoting.Messaging.CallContext.GetData ("WxeContext") as WxeContext; }
//    get 
//    {
//      object obj = System.Runtime.Remoting.Messaging.CallContext.GetData ("WxeContext");
//      if (obj == null)
//        return null;
//      WxeContext context = obj as WxeContext;
//      if (context != null)
//        return context;
//      // Loop unitl WxeContext-like type is found
//      //if (obj.GetType().FullName == typeof (WxeContext).FullName)
//      //  throw new InvalidOperationException ("Wrong Assembly");
//      //else // Provoke an invalid cast exception
//        return (WxeContext) obj;
//    }
  }

  internal static void SetCurrent (WxeContext value)
  {
    // _current = value; 
    System.Runtime.Remoting.Messaging.CallContext.SetData ("WxeContext", value);
  }

  private HttpContext _httpContext;
  private bool _isPostBack;
  private bool _isReturningPostBack;
  private NameValueCollection _postBackCollection;
  private string _functionToken;
  private WxeFunction _returningFunction = null;

  public WxeContext (HttpContext context, string functionToken)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);

    _httpContext = context;
    _functionToken = functionToken;
    _isPostBack = false;
    _isReturningPostBack = false;
    _postBackCollection = null;
  }

  public HttpContext HttpContext
  {
    get { return _httpContext; }
  }

  /// <summary>
  ///   Gets a flag that corresponds to the <see cref="System.Web.UI.Page.IsPostBack">Page.IsPostBack</see> flag, but is 
  ///   available from the beginning of the execution cycle, i.e. even before <b>OnInit</b>.
  /// </summary>
  public bool IsPostBack
  {
    get { return _isPostBack; }
    set { _isPostBack = value; }
  }

  /// <summary>
  ///   During the execution of a page, specifies whether the current postback cycle was caused by returning from a 
  ///   <see cref="WxeFunction"/>.
  /// </summary>
  public bool IsReturningPostBack 
  {
    get { return _isReturningPostBack; }
    set { _isReturningPostBack = value; }
  }

  /// <summary> Gets or sets the postback data for the page if it has executed a sub-function. </summary>
  /// <value> The postback data generated during the roundtrip that led to the execution of the sub-function. </value>
  /// <remarks> 
  ///   <para>
  ///     This property is used only for transfering the postback data from the backup location to the page's
  ///     initialization infrastructure.
  ///   </para><para>
  ///     Application developers should only use the 
  ///     <see cref="IWxePage.GetPostBackCollection">IWxePage.GetPostBackCollection</see> method to access the
  ///     postback data.
  ///   </para><para>
  ///     Control developers should either implement <see cref="System.Web.UI.IPostBackDataHandler"/> to access 
  ///     postback data relevant to their control or, if they develop a composite control, use the child controls' 
  ///     integrated data handling features to access the data.
  ///   </para>
  /// </remarks>
  [EditorBrowsable (EditorBrowsableState.Advanced)]
  public NameValueCollection PostBackCollection
  {
    get { return _postBackCollection; }
    set { _postBackCollection = value; }
  }

  public string FunctionToken
  {
    get { return _functionToken; }
  }

  public WxeFunction ReturningFunction 
  {
    get { return _returningFunction; }
    set { _returningFunction = value; }
  }

  /// <summary>
  ///   Gets the absolute URL that resumes the current function.
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
    string pathPart = GetResumePath (true);
    pathPart = HttpContext.Response.ApplyAppPathModifier (pathPart);
    string serverPart = HttpContext.Request.Url.GetLeftPart (System.UriPartial.Authority);
    return serverPart + pathPart;
  }

  /// <summary>
  ///   Gets the path that resumes the current function.
  /// </summary>
  /// <param name="absolute"> 
  ///   <see langword="true"/> to get the absolute path, otherwise only the <b>WxeHandler</b>'s filename and query 
  ///   are returned.
  /// </param>
  public string GetResumePath (bool absolute)
  {
    return WxeContext.GetResumePath (HttpContext.Request, HttpContext.Response, FunctionToken, absolute);
  }

  /// <summary> Gets the path that resumes the function with specified token. </summary>
  /// <param name="functionToken"> The token of function to resume. </param>
  /// <param name="absolute"> 
  ///   <see langword="true"/> to get the absolute path, otherwise only the <b>WxeHandler</b>'s filename and query 
  ///   are returned.
  /// </param>
  public static string GetResumePath (HttpRequest request, HttpResponse response, string functionToken, bool absolute)
  {
    string path;
    if (absolute)
      path = response.ApplyAppPathModifier (request.Url.AbsolutePath);
    else
      path = System.IO.Path.GetFileName (request.Url.AbsolutePath);
    return path + "?" + WxeHandler.Parameters.WxeFunctionToken + "=" + functionToken;
  }

  /// <summary> Gets the path for to the <b>WxeHandler</b> used in the request. </summary>
  /// <param name="absolute"> 
  ///   <see langword="true"/> to get the absolute path, otherwise only the <b>WxeHandler</b>'s filename and query 
  ///   are returned.
  /// </param>
  public string GetPath (bool absolute)
  {
    return WxeContext.GetPath (HttpContext.Request, HttpContext.Response, absolute);
  }

  /// <summary> Gets the path for to the <b>WxeHandler</b> used in the <paramref name="request"/>. </summary>
  /// <param name="absolute"> 
  ///   <see langword="true"/> to get the absolute path, otherwise only the <b>WxeHandler</b>'s filename and query 
  ///   are returned.
  /// </param>
  public static string GetPath (HttpRequest request, HttpResponse response, bool absolute)
  {
    if (absolute)
      return response.ApplyAppPathModifier (request.Url.AbsolutePath);
    else
      return System.IO.Path.GetFileName (request.Url.AbsolutePath);
  }
}

}
