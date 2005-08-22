using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Reflection;
using System.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> 
///   The <see cref="IHttpHandler"/> implementation responsible for handling requests to the 
///   <b>rubicon Execution Engine.</b>
/// </summary>
  /// <include file='doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/Class/*' />
public class WxeHandler: IHttpHandler, IRequiresSessionState
{
  /// <summary> Contains a list of parameters supported by the <see cref="WxeHandler"/>. </summary>
  /// <remarks> The string representations of the individual parameters are identical to the field names. </remarks>
  public class Parameters
  {
    /// <summary> Denotes the type of the <see cref="WxeFunction"/> to initialize. </summary>
    /// <remarks> 
    ///   The argument may be presented as a regular .net Type string or the abbreviated version as specified in
    ///   <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
    /// </remarks>
    public const string WxeFunctionType = "WxeFunctionType";

    /// <summary> Denotes the <b>ID</b> of the <see cref="WxeFunction"/> to resume or assigned during initialization. </summary>
    public const string WxeFunctionToken = "WxeFunctionToken";
    
    /// <summary> Denotes the <b>URL</b> to return to after the function has completed. </summary>
    /// <remarks>   
    ///   Only evaluated during initialization. Replaces the <see cref="WxeFunction.ReturnUrl"/> defined by the 
    ///   function it self. 
    /// </remarks>
    public const string WxeReturnUrl = "ReturnUrl";
    
    /// <summary> Denotes a special action to be executed. </summary>
    /// <remarks> See the <see cref="Actions"/> type for a list of supported arguments. </remarks>
    public const string WxeAction = "WxeAction";
  }

  /// <summary> Denotes the arguments supported for the <see cref="Parameters.WxeAction"/> parameter. </summary>
  /// <remarks> The string representations of the individual argument values are identical to the field names. </remarks>
  public class Actions
  {
    /// <summary> Denotes a session refresh. </summary>
    public const string Refresh = "Refresh";
    
    /// <summary> Denotes a session abort. </summary>
    public const string Abort = "Abort";

    /// <summary> Denotes a session abort. (Obsolete) </summary>
    public const string Cancel = "Cancel";
  }

  /// <summary> The <see cref="WxeFunctionState"/> representing the <see cref="CurrentFunction"/> and it's context. </summary>
  WxeFunctionState _currentFunctionState;

  /// <summary> The root function executed by the <b>WxeHanlder</b>. </summary>
  /// <value> The <see cref="WxeFunction"/> invoked by the <see cref="Parameters.WxeFunctionType"/> parameter. </value>
  //TODO: Find a better name
  public WxeFunction CurrentFunction
  {
    get { return _currentFunctionState.Function; }
  }

  /// <summary> Gets a flag indication whether session management is enabled for the application. </summary>
  /// <value> <see langword="true"/> if session management is enabled. </value>
  /// <remarks> Without session management both session refreshing and session aborting are disabled. </remarks>
  public static bool IsSessionManagementEnabled
  {
    get { return WebConfiguration.Current.ExecutionEngine.EnableSessionManagement; }
  }

  /// <summary> Gets a flag indication whether session refreshing is enabled for the application. </summary>
  /// <value> <see langword="true"/> if session refreshing is enabled. </value>
  public static bool IsSessionRefreshEnabled
  {
    get { return WebConfiguration.Current.ExecutionEngine.RefreshInterval > 0; }
  }

  /// <summary> Gets session refresh interval for the application. </summary>
  /// <value> The time between refresh postbacks in minutes. </value>
  public static int RefreshInterval
  {
    get { return WebConfiguration.Current.ExecutionEngine.RefreshInterval; }
  }

  /// <summary> Processes the requests associated with the <see cref="WxeHandler"/>. </summary>
  /// <param name="context"> The <see cref="HttpContext"/> or the request. Must not be <see langword="null"/>. </param>
  public virtual void ProcessRequest (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);

    CheckTimeoutConfiguration (context);

    string typeName = context.Request.Params[Parameters.WxeFunctionType];
    bool hasTypeName = ! StringUtility.IsNullOrEmpty (typeName);

    string functionToken = context.Request.Params[Parameters.WxeFunctionToken];
    bool hasFunctionToken = ! StringUtility.IsNullOrEmpty (functionToken);

    if (hasTypeName)
    {
      _currentFunctionState = CreateNewFunction (context, typeName, functionToken);
      ExecuteFunctionState (context, _currentFunctionState, true);
      ProcessReturnUrl (context, _currentFunctionState);
    }
    else if (hasFunctionToken)
    {
      _currentFunctionState = ResumeExistingFunction (context, functionToken);
      if (_currentFunctionState != null)
      {
        ExecuteFunctionState (context, _currentFunctionState, false);
        ProcessReturnUrl (context, _currentFunctionState);
      }
    }
    else
    {
      throw new HttpException ("Missing URL parameter '" + Parameters.WxeFunctionType + "'");
    }
  }

  /// <summary> Checks whether the timeout settings are valid. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/CheckTimeoutConfiguration/*' />
  protected void CheckTimeoutConfiguration (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);

    if (! IsSessionManagementEnabled)
      return;

    int refreshInterval = WebConfiguration.Current.ExecutionEngine.RefreshInterval;
    if (refreshInterval > 0)
    {
      if (refreshInterval >= context.Session.Timeout)
        throw new ApplicationException ("The RefreshInterval setting in the configuration must be less than the session timeout.");
      int functionTimeout = WebConfiguration.Current.ExecutionEngine.FunctionTimeout;
      if (refreshInterval >= functionTimeout)
        throw new ApplicationException ("The RefreshInterval setting in the configuration must be less than the function timeout.");
    }
  }

  /// <summary> Initializes a new <see cref="WxeFunction"/>, encapsulated in a <see cref="WxeFunctionState"/> object. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/CreateNewFunction/*' />
  protected WxeFunctionState CreateNewFunction (HttpContext context, string typeName, string functionToken)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    if (functionStates == null)
    {
      functionStates = new WxeFunctionStateCollection();
      WxeFunctionStateCollection.Instance = functionStates;
    }
    else
    {
      functionStates.CleanUpExpired();
    }

    Type type = TypeUtility.GetType (typeName, true, false);
    WxeFunction function = (WxeFunction) Activator.CreateInstance (type);

    WxeFunctionState functionState;
    if (StringUtility.IsNullOrEmpty (functionToken))
      functionState = new WxeFunctionState (function);
    else
      functionState = new WxeFunctionState (function, functionToken);
    functionStates.Add (functionState);

    function.InitializeParameters (context.Request.Params);
    string returnUrlArg = context.Request.Params[Parameters.WxeReturnUrl];
    if (! StringUtility.IsNullOrEmpty (returnUrlArg))
      function.ReturnUrl = returnUrlArg;

    return functionState;
  }

  /// <summary> Resumes an existing <see cref="WxeFunction"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ResumeExistingFunction/*' />
  protected WxeFunctionState ResumeExistingFunction (HttpContext context, string functionToken)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);

    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    if (functionStates == null)
      throw new ApplicationException ("Session timeout."); // TODO: display error message

    WxeFunctionState functionState = functionStates.GetItem (functionToken);
    if (functionState == null || functionState.IsExpired)
      throw new ApplicationException ("Page timeout."); // TODO: display error message
    if (functionState.IsAborted)
      throw new ApplicationException ("WxeFunctionState {0} is aborted." + functionState.FunctionToken); // TODO: display error message
  
    string action = context.Request[Parameters.WxeAction];
    bool isRefresh = StringUtility.AreEqual (action, Actions.Refresh, true);
    bool isAbort =   StringUtility.AreEqual (action, Actions.Abort, true) 
                  || StringUtility.AreEqual (action, Actions.Cancel, true);

    if (isRefresh)
    {
      functionState.Touch();
      return null;
    }
    else if (isAbort)
    {
      functionStates.CleanUpExpired();
      functionStates.Abort (functionState);
      return null;
    }
    else
    {
      functionState.Touch();
      functionStates.CleanUpExpired();
      if (functionState.Function == null)
        throw new ApplicationException ("Function missing in WxeFunctionState {0}." + functionState.FunctionToken);
      return functionState;
    }
  }

  /// <summary> 
  ///   Sets the current <see cref="WxeContext"/> and invokes <see cref="ExecuteFunction"/> on the
  ///   <paramref name="functionState"/>'s <see cref="WxeFunctionState.Function"/>.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ExecuteFunctionState/*' />
  protected void ExecuteFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNull ("functionState", functionState);
    if (functionState.IsAborted)
      throw new ApplicationException ("The functionState is aborted.");

    WxeContext wxeContext = new WxeContext (context); 
    wxeContext.FunctionToken = functionState.FunctionToken;
    WxeContext.SetCurrent (wxeContext);

    ExecuteFunction (functionState.Function, wxeContext, isNewFunction);
  }

  /// <summary>  Invokes <see cref="WxeFunction.Execute"/> on the <paramref name="function"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ExecuteFunction/*' />
  protected virtual void ExecuteFunction (WxeFunction function, WxeContext context, bool isNew)
  {
    ArgumentUtility.CheckNotNull ("function", function);
    ArgumentUtility.CheckNotNull ("context", context);
    if (function.IsAborted)
      throw new ApplicationException ("The function is aborted.");

    function.AppendCatchExceptionTypes (typeof(WxeUserCancelException));
    function.Execute (context);
  }

  /// <summary> Redirects the <see cref="HttpContext.Response"/> to an optional <see cref="WxeFunction.ReturnUrl"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ProcessReturnUrl/*' />
  protected void ProcessReturnUrl (HttpContext context, WxeFunctionState functionState)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNull ("functionState", functionState);

    string returnUrl = functionState.Function.ReturnUrl;
    if (returnUrl != null)
    {
      // Variables.Clear();
      if (returnUrl.StartsWith ("javascript:"))
      {
        context.Response.Clear();
        string script = returnUrl.Substring ("javascript:".Length);
        context.Response.Write ("<html><script language=\"JavaScript\">" + script + "</script></html>");
        context.Response.End();
      }
      else
      {
        context.Response.Redirect (returnUrl, true);
      }
    }
  }

  bool IHttpHandler.IsReusable
  {
    get { return false; }
  }
}

}