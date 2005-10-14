using System;
using System.ComponentModel;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;

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

  /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
  /// <param name="request"> The <see cref="HttpRequest"/>. Must not be <see langword="null"/>. </param>
  /// <param name="response"> The <see cref="HttpResponse"/>. Must not be <see langword="null"/>. </param>
  /// <param name="functionToken"> 
  ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
  /// </param>
  /// <param name="queryString"> An optional query string. </param>
  public static string GetResumePath (
      HttpRequest request, HttpResponse response, string functionToken, string queryString)
  {
    ArgumentUtility.CheckNotNull ("request", request);
    return WxeContext.GetResumePath (request.Url.AbsolutePath, response, functionToken, queryString);
  }

  /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
  /// <param name="path"> The path to the <see cref="WxeHandler"/>. Must not be <see langword="null"/> or emtpy. </param>
  /// <param name="response"> The <see cref="HttpResponse"/>. Must not be <see langword="null"/>. </param>
  /// <param name="functionToken"> 
  ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
  /// </param>
  /// <param name="queryString"> An optional query string. </param>
  public static string GetResumePath (
      string path, HttpResponse response, string functionToken, string queryString)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("path", path);
    ArgumentUtility.CheckNotNull ("response", response);
    ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
    
    if (! StringUtility.IsNullOrEmpty (queryString) && ! queryString.StartsWith ("?"))
      queryString = "?" + queryString;
    
    if (! StringUtility.IsNullOrEmpty (queryString))
      queryString = PageUtility.DeleteUrlParameter (queryString, WxeHandler.Parameters.WxeFunctionToken);
    if (StringUtility.IsNullOrEmpty (queryString) || queryString == "?")
      queryString = string.Empty;
    queryString = PageUtility.AddUrlParameter (queryString, WxeHandler.Parameters.WxeFunctionToken, functionToken);
    
    path = response.ApplyAppPathModifier (path);
    return path + queryString;
  }

  private HttpContext _httpContext;
  private bool _isPostBack = false;
  private bool _isReturningPostBack = false;
  private NameValueCollection _postBackCollection = null;
  private WxeFunction _returningFunction = null;
  private WxeFunctionState _functionState;
  private string _queryString = string.Empty;

  public WxeContext (HttpContext context, WxeFunctionState functionState, string queryString)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNull ("functionState", functionState);

    _httpContext = context;
    _functionState = functionState;

    if (! StringUtility.IsNullOrEmpty (queryString))
    {
      if (! queryString.StartsWith ("?"))
        queryString = "?" + queryString;
      queryString = PageUtility.DeleteUrlParameter (queryString, WxeHandler.Parameters.WxeFunctionToken);
      queryString = PageUtility.DeleteUrlParameter (queryString, WxeHandler.Parameters.WxeFunctionType);
      if (queryString != "?")
        _queryString = queryString;
    }
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

  protected WxeFunctionState FunctionState
  {
    get { return _functionState; }
  }

  public string FunctionToken
  {
    get { return _functionState.FunctionToken; }
  }

  public int PostBackID
  {
    get { return _functionState.PostBackID; }
  }

  public string QueryString
  {
    get { return _queryString; }
  }

  public WxeFunction ReturningFunction 
  {
    get { return _returningFunction; }
    set { _returningFunction = value; }
  }

  /// <summary> Gets the absolute URL that resumes the current function. </summary>
  /// <remarks>
  ///   If a WXE application branches to an external web site, the external site can
  ///   link back to this URL to resume the current function at the point where 
  ///   it was interrupted. Note that if the user stays on the external site longer
  ///   that the session or function timeout, resuming will fail with a timeout
  ///   exception.
  /// </remarks>
  public string GetResumeUrl ()
  {
    string pathPart = GetResumePath ();
    pathPart = HttpContext.Response.ApplyAppPathModifier (pathPart);
    string serverPart = HttpContext.Request.Url.GetLeftPart (System.UriPartial.Authority);
    return serverPart + pathPart;
  }

  /// <summary> Gets the absolute path that resumes the current function. </summary>
  public string GetResumePath ()
  {
    return WxeContext.GetResumePath (_httpContext.Request, _httpContext.Response, FunctionToken, QueryString);
  }

  /// <summary> Gets the absolute path for to the <b>WxeHandler</b> used in the request. </summary>
  public string GetPath ()
  {
    string path = _httpContext.Response.ApplyAppPathModifier (_httpContext.Request.Url.AbsolutePath);
    return path + QueryString;
  }
}

}
