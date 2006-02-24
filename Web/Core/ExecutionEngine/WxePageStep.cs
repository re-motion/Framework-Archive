using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine.UrlMapping;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> This step interrupts the server side execution to display a page to the user. </summary>
/// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Class/*' />
[Serializable]
public class WxePageStep: WxeStep
{
  private string _page = null;
  private string _pageref = null;
  private string _pageRoot;
  private string _pageToken;
  private WxeFunction _function;
  private NameValueCollection _postBackCollection;
  private string _viewState;
  private bool _isRedirectToPermanentUrlRequired = false;
  private bool _useParentPermaUrl = false;
  private NameValueCollection _permaUrlParameters;
  private bool _createPermaUrl = false;
  private bool _returnToCaller = false;
  private NameValueCollection _callerUrlParameters = null;
  private bool _isRedirectedToPermanentUrl = false;
  private bool _hasReturnedFromRedirectToPermanentUrl = false;
  private string _resumeUrl;
  private bool _isExecuteFunctionExternalRequired = false;
  private bool _isExternalFunctionInvoked = false;

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page"]' />
  public WxePageStep (string page)
    : this (null, page)
  {
  }

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page" or @name="resourceAssembly"]' />
  protected WxePageStep (Assembly resourceAssembly, string page)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("page", page);

    _page = page;
    Initialize (resourceAssembly);
  }

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref"]' />
  public WxePageStep (WxeVariableReference pageref)
    : this (null, pageref)
  {
  }

  /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref" or @name="resourceAssembly"]' />
  protected WxePageStep (Assembly resourceAssembly, WxeVariableReference pageref)
  {
    ArgumentUtility.CheckNotNull ("pageref", pageref);

    _pageref = pageref.Name;
    Initialize (resourceAssembly);
  }

  /// <summary> Common initalization code for the <see cref="WxePageStep"/>. </summary>
  /// <param name="resourceAssembly"> The (optional) <see cref="Assembly"/> containing the page. </param>
  private void Initialize (Assembly resourceAssembly)
  {
    _pageToken = Guid.NewGuid().ToString();
    _function = null;

    if (resourceAssembly == null)
      _pageRoot = string.Empty;
    else
      _pageRoot = ResourceUrlResolver.GetAssemblyRoot (false, resourceAssembly);
  }

  /// <summary> The URL of the page to be displayed by this <see cref="WxePageStep"/>. </summary>
  protected string Page
  { 
    get
    {
      string name;
      if (_page != null)
        name = _page;
      else if (_pageref != null && Variables[_pageref] != null)
        name = (string) Variables[_pageref];
      else
        throw new WxeException ("No Page specified for " + this.GetType().FullName + ".");
      return _pageRoot + name;
    }
  }

  /// <summary> 
  ///   Displays the <see cref="WxePageStep"/>'s page or the sub-function that has been invoked by the 
  ///   <see cref="ExecuteFunction"/> method.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Execute/*' />
  public override void Execute (WxeContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);

    if (_function == null)
    {
      //  This is the PageStep if it isn't executing a sub-function
      
      ProcessCurrentFunction (context);
    }
    else
    {
      if (! _isExecuteFunctionExternalRequired)
      {
        //  This is the PageStep currently executing a sub-function
        
        EnsureHasRedirectedToPermanentUrl (context);
        _function.Execute (context);
        //  This point is only reached after the sub-function has completed execution.

        //  This is the PageStep after the sub-function has completed execution
        
        EnsureHasReturnedFromRedirectToPermanentUrl (context);
        ProcessExecutedFunction (context);
        CleanupAfterHavingReturnedFromRedirectToPermanentUrl();
      }
      else
      {
        //  This is the PageStep currently executing an external function
        EnsureExternalFunctionInvoked (context);
        //  This point is only reached after the external function has been started.

        //  This is the PageStep after the external function has completed execution 
        //  or a postback to the executing page has been received
        ProcessExecutedFunction (context);
        CleanupAfterHavingInvokedExternalFunction();
      }
    }

    try 
    {
      context.HttpContext.Server.Transfer (Page, context.IsPostBack);
    }
    catch (HttpException e)
    {
      if (e.InnerException is WxeExecuteNextStepException)
        return;
      if (e.InnerException is HttpUnhandledException && e.InnerException.InnerException is WxeExecuteNextStepException)
        return;
      throw;
    }
  }

  private void ProcessCurrentFunction (WxeContext context)
  {
    //  Use the Page's postback data
    context.PostBackCollection = null;
    context.SetIsReturningPostBack (false);
  }

  private void ProcessExecutedFunction (WxeContext context)
  {
    //  Provide the executed sub-function to the executing page
    context.ReturningFunction = _function;
    _function = null;

    context.SetIsPostBack (true);

    //  Provide the backed up postback data to the executing page
    context.PostBackCollection = _postBackCollection;
    _postBackCollection = null;
    context.SetIsReturningPostBack (true);
  }

  private void EnsureHasRedirectedToPermanentUrl (WxeContext context)
  {
    if (_isRedirectToPermanentUrlRequired && ! _isRedirectedToPermanentUrl)
    {
      string destinationUrl = GetDestinationPermanentUrl (context);

      _permaUrlParameters = null;
      _resumeUrl = context.GetResumePath();
      _isRedirectedToPermanentUrl = true;
      PageUtility.Redirect (context.HttpContext.Response, destinationUrl);
    }
  }

  private void EnsureHasReturnedFromRedirectToPermanentUrl (WxeContext context)
  {
    if (_isRedirectToPermanentUrlRequired && _isRedirectedToPermanentUrl && ! _hasReturnedFromRedirectToPermanentUrl)
    {
      _hasReturnedFromRedirectToPermanentUrl = true;        
      PageUtility.Redirect (context.HttpContext.Response, _resumeUrl);
    }
  }

  private void CleanupAfterHavingReturnedFromRedirectToPermanentUrl()
  {
    if (_isRedirectToPermanentUrlRequired && _hasReturnedFromRedirectToPermanentUrl)
    {
      _isRedirectToPermanentUrlRequired = false;
      _isRedirectedToPermanentUrl = false;
      _hasReturnedFromRedirectToPermanentUrl = false;
    }
  }

  private string GetDestinationPermanentUrl (WxeContext context)
  {
    NameValueCollection internalUrlParameters;
    if (_permaUrlParameters == null)
      internalUrlParameters = _function.SerializeParametersForQueryString();
    else
      internalUrlParameters = CollectionUtility.Clone (_permaUrlParameters);
    
    internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);

    return context.GetPermanentUrl (_function.GetType(), internalUrlParameters, _useParentPermaUrl);
  }

  private void EnsureExternalFunctionInvoked (WxeContext context)
  {
    if (_isExecuteFunctionExternalRequired && ! _isExternalFunctionInvoked)
    {
      string functionToken = GetFunctionTokenForExternalFunction (_function, _returnToCaller);

      string destinationUrl = GetDestinationUrlForExternalFunction (
          _function, functionToken, _createPermaUrl, _useParentPermaUrl, _permaUrlParameters);

      if (_returnToCaller)
      {
        NameValueCollection internalCallerUrlParameters;
        if (_callerUrlParameters == null)
          internalCallerUrlParameters = ParentFunction.SerializeParametersForQueryString();
        else
          internalCallerUrlParameters = CollectionUtility.Clone (_callerUrlParameters);
        internalCallerUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, context.FunctionToken);

        _function.ReturnUrl = context.GetPermanentUrl (ParentFunction.GetType(), internalCallerUrlParameters);
      }

      _permaUrlParameters = null;
      _callerUrlParameters = null;
      _isExternalFunctionInvoked = true;
      PageUtility.Redirect (context.HttpContext.Response, destinationUrl);
    }
  }

  private void CleanupAfterHavingInvokedExternalFunction()
  {
    if (_isExecuteFunctionExternalRequired && _isExternalFunctionInvoked)
    {
      _isExecuteFunctionExternalRequired = false;
      _isExternalFunctionInvoked = false;
    }
  }

  /// <summary> 
  ///   Initalizes a new <see cref="WxeFunctionState"/> with the passed <paramref name="function"/> and returns
  ///   the associated function token.
  /// </summary>
  internal string GetFunctionTokenForExternalFunction (WxeFunction function, bool returnFromExecute)
  {
    bool enableCleanUp = ! returnFromExecute;
    WxeFunctionState functionState = new WxeFunctionState (function, enableCleanUp);
    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    functionStates.Add (functionState);
    return functionState.FunctionToken;
  }


  /// <summary> Gets the URL to be used for transfering to the external function. </summary>
  internal string GetDestinationUrlForExternalFunction (
      WxeFunction function, string functionToken, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    WxeContext wxeContext = WxeContext.Current;

    string href;
    if (createPermaUrl)
    {
      NameValueCollection internalUrlParameters;
      if (urlParameters == null)
        internalUrlParameters = function.SerializeParametersForQueryString();
      else
        internalUrlParameters = CollectionUtility.Clone (urlParameters);
      internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, functionToken);

      href = wxeContext.GetPermanentUrl (function.GetType(), internalUrlParameters, useParentPermaUrl);
    }
    else
    {
      UrlMappingEntry mappingEntry = UrlMappingConfiguration.Current.Mappings[function.GetType()];
      string path = (mappingEntry != null) ? mappingEntry.Resource : wxeContext.HttpContext.Request.Url.AbsolutePath;
      string queryString = null;
      if (urlParameters != null)
        queryString = UrlUtility.FormatQueryString (urlParameters);
      href = wxeContext.GetPath (path, functionToken, queryString);
    }

    return href;
  }

  /// <summary> Gets the currently executing <see cref="WxeStep"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecutingStep/*' />
  public override WxeStep ExecutingStep
  {
    get
    {
      if (_function != null)
        return _function.ExecutingStep;
      else
        return this;
    }
  }

  /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecuteFunction/*' />
  public void ExecuteFunction (
      IWxePage page, WxeFunction function, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("page", page);

    //  Back-up postback data of the executing page
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());
    
    InternalExecuteFunction (page, function, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary>
  ///   Executes the specified <see cref="WxeFunction"/>, then returns to this page without raising the 
  ///   postback event after the user returns.
  /// </summary>
  /// <remarks> Invoke this method by calling <see cref="WxePageInfo.ExecuteFunctionNoRepost"/>. </remarks>
  internal void ExecuteFunctionNoRepost (
      IWxePage page, WxeFunction function, Control sender, bool usesEventTarget,
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("page", page);

    //  Back-up post back data of the executing page
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());

    //  Remove post back event source from the post back data
    if (usesEventTarget)
    {
      _postBackCollection.Remove (ControlHelper.PostEventSourceID);
      _postBackCollection.Remove (ControlHelper.PostEventArgumentID );
    }
    else
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      if (! (sender is IPostBackEventHandler || sender is IPostBackDataHandler))
        throw new ArgumentException ("The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
      _postBackCollection.Remove (sender.UniqueID);
    }

    InternalExecuteFunction (page, function, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/InternalExecuteFunction/*' />
  private void InternalExecuteFunction (
      IWxePage page, WxeFunction function, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("page", page);
    ArgumentUtility.CheckNotNull ("function", function);

    if (_function != null)
      throw new InvalidOperationException ("Cannot execute function while another function executes.");

    _function = function; 
    _function.SetParentStep (this);
    _isRedirectToPermanentUrlRequired = createPermaUrl;
    _useParentPermaUrl = useParentPermaUrl;
    _permaUrlParameters = permaUrlParameters;

    InvokeSaveAllState ((Page) page);

    Execute();
  }

  internal void ExecuteFunctionExternal (
      IWxePage page, WxeFunction function,
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters,
      bool returnToCaller, NameValueCollection callerUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("page", page);
    ArgumentUtility.CheckNotNull ("function", function);

    //  Back-up post back data of the executing page
    _postBackCollection = new NameValueCollection (page.GetPostBackCollection());

    _isExecuteFunctionExternalRequired = true;
    _isExternalFunctionInvoked = false;
    _function = function; 
    _createPermaUrl = createPermaUrl;
    _useParentPermaUrl = useParentPermaUrl;
    _permaUrlParameters = permaUrlParameters;
    _returnToCaller = returnToCaller;
    _callerUrlParameters = callerUrlParameters;

    InvokeSaveAllState ((Page) page);

    Execute();
  }

  private void InvokeSaveAllState (Page page)
  {
    // page.SaveVieState()
    MethodInfo saveViewStateMethod;
#if NET11    
    saveViewStateMethod = typeof (Page).GetMethod ("SavePageViewState", BindingFlags.Instance | BindingFlags.NonPublic);
#else
    saveViewStateMethod = typeof (Page).GetMethod ("SaveAllState", BindingFlags.Instance | BindingFlags.NonPublic);
#endif
    saveViewStateMethod.Invoke (page, new object[0]); 
  }

  /// <summary> Gets the token for this page step. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/PageToken/*' />
  public string PageToken
  {
    get { return _pageToken; }
  }

  /// <summary> Returns the string identifying the <b>ASP.NET</b> page used for this <see cref="WxePageStep"/>. </summary>
  /// <returns> The value of <see cref="Page"/>. </returns>
  public override string ToString()
  {
    return Page;
  }

  /// <summary> Saves the passed <paramref name="viewState"/> object into the <see cref="WxePageStep"/>. </summary>
  /// <param name="viewState"> An <b>ASP.NET</b> viewstate object. </param>
  public void SavePageStateToPersistenceMedium (object viewState)
  {
    LosFormatter formatter = new LosFormatter ();
    StringWriter writer = new StringWriter ();
    formatter.Serialize (writer, viewState);
    _viewState = writer.ToString();
  }

  /// <summary> 
  ///   Returns the viewstate previsously saved through the <see cref="SavePageStateToPersistenceMedium"/> method. 
  /// </summary>
  /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
  public object LoadPageStateFromPersistenceMedium()
  {
    LosFormatter formatter = new LosFormatter ();
    return formatter.Deserialize (_viewState);
  }
  
  /// <summary> 
  ///   Aborts the <see cref="WxePageStep"/>. Aborting will cascade to any <see cref="WxeFunction"/> executed
  ///   in the scope of this step.
  /// </summary>
  protected override void AbortRecursive()
  {
    base.AbortRecursive ();
    if (_function != null)
      _function.Abort();
  }
}

}
