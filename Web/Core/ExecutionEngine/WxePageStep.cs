/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  //TODO: refactor page, pageref into page-object
  //TODO: refactor ctors
  /// <summary> This step interrupts the server side execution to display a page to the user. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Class/*' />
  [Serializable]
  public class WxePageStep : WxeStep, IExecutionStateContext
  {
    private IWxePageExecutor _pageExecutor = new WxePageExecutor();
    private readonly string _page;
    private readonly string _pageref;
    private string _pageRoot;
    private string _pageToken;
    private string _pageState;
    
    private WxeFunction _innerFunction;
    private string _userControlID;
    private string _userControlState;

    [NonSerialized]
    private WxeHandler _wxeHandler;

    private bool _isReturningInnerFunction;
    private IExecutionState _executionState;

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

      if (resourceAssembly == null)
        _pageRoot = string.Empty;
      else
        _pageRoot = ResourceUrlResolver.GetAssemblyRoot (false, resourceAssembly);
    }

    /// <summary> The URL of the page to be displayed by this <see cref="WxePageStep"/>. </summary>
    public string Page
    {
      get
      {
        string name;
        if (_page != null)
          name = _page;
        else if (_pageref != null && Variables[_pageref] != null)
          name = (string) Variables[_pageref];
        else
          throw new WxeException ("No Page specified for " + GetType().FullName + ".");
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

      if (_wxeHandler != null)
      {
        context.HttpContext.Handler = _wxeHandler;
        _wxeHandler = null;
      }

      if (_executionState == null)
      {
        //  This is the PageStep if it isn't executing a sub-function

        SetStateForCurrentFunction (context);

        if (_innerFunction != null)
        {
          bool isPostBackBackUp = context.IsPostBack;
          try
          {
            _innerFunction.Execute (context);
          }
          catch (WxeExecuteUserControlStepException)
          {
            context.SetIsPostBack (isPostBackBackUp);
          }
        }
      }
      else
      {
        //  This is the PageStep currently executing a sub-function

        while (_executionState.IsExecuting)
          _executionState.ExecuteSubFunction (context);

        //  This point is only reached after the sub-function has completed execution or a function executing an external function has been post-backed to.

        _executionState.PostProcessSubFunction (context);
      }

      try
      {
        _pageExecutor.ExecutePage (context, Page);
      }
      catch (WxeExecuteUserControlNextStepException)
      {
        _isReturningInnerFunction = true;

        try
        {
          _pageExecutor.ExecutePage (context, Page);
        }
        finally
        {
          _userControlID = null;
          _innerFunction = null;
          _userControlState = null;
          _isReturningInnerFunction = false;
        }
      }
    }

    /// <summary> 
    ///   Initalizes a new <see cref="WxeFunctionState"/> with the passed <paramref name="function"/> and returns the associated function token.
    /// </summary>
    internal string GetFunctionTokenForExternalFunction (WxeFunction function, bool returnFromExecute)
    {
      bool enableCleanUp = ! returnFromExecute;
      WxeFunctionState functionState = new WxeFunctionState (function, enableCleanUp);
      WxeFunctionStateManager functionStates = WxeFunctionStateManager.Current;
      functionStates.Add (functionState);
      return functionState.FunctionToken;
    }


    /// <summary> Gets the URL to be used for transfering to the external function. </summary>
    internal string GetDestinationUrlForExternalFunction (WxeFunction function, string functionToken, WxePermaUrlOptions permaUrlOptions)
    {
      WxeContext wxeContext = WxeContext.Current;

      string href;
      if (permaUrlOptions.UsePermaUrl)
      {
        NameValueCollection internalUrlParameters;
        if (permaUrlOptions.UrlParameters == null)
          internalUrlParameters = function.SerializeParametersForQueryString();
        else
          internalUrlParameters = permaUrlOptions.UrlParameters.Clone ();
        internalUrlParameters.Set (WxeHandler.Parameters.WxeFunctionToken, functionToken);

        href = wxeContext.GetPermanentUrl (function.GetType (), internalUrlParameters, permaUrlOptions.UseParentPermaUrl);
      }
      else
      {
        UrlMappingEntry mappingEntry = UrlMappingConfiguration.Current.Mappings[function.GetType()];
        string path = (mappingEntry != null) ? mappingEntry.Resource : wxeContext.HttpContext.Request.Url.AbsolutePath;
        href = wxeContext.GetPath (path, functionToken, permaUrlOptions.UrlParameters);
      }

      return href;
    }

    /// <summary> Gets the currently executing <see cref="WxeStep"/>. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecutingStep/*' />
    public override WxeStep ExecutingStep
    {
      get
      {
        if (_executionState != null)
          return _executionState.Parameters.SubFunction.ExecutingStep;
        else
          return this;
      }
    }

    /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecuteFunction/*' />
    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (IWxePage page, WxeFunction function, WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      NameValueCollection postBackCollection = BackupPostBackCollection (page);

      InternalExecuteFunction (page, function, permaUrlOptions, postBackCollection);
    }

    /// <summary>
    ///   Executes the specified <see cref="WxeFunction"/>, then returns to this page without raising the 
    ///   postback event after the user returns.
    /// </summary>
    /// <remarks> Invoke this method by calling <see cref="WxeExecutor{TWxePage}.ExecuteFunctionNoRepost"/>. </remarks>
    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunctionNoRepost (
        IWxePage page, WxeFunction function, Control sender, bool usesEventTarget, WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      NameValueCollection postBackCollection = BackupPostBackCollection (page);
      RemoveEventSource (postBackCollection, sender, usesEventTarget);

      InternalExecuteFunction (page, function, permaUrlOptions, postBackCollection);
    }

    /// <summary> Executes the specified <see cref="WxeFunction"/>, then returns to this page. </summary>
    /// <remarks> 
    ///   This method contains common execution code and is called by the <see cref="ExecuteFunction"/> 
    ///   and <see cref="ExecuteFunctionNoRepost"/> methods.
    /// </remarks>
    private void InternalExecuteFunction (
        IWxePage page, WxeFunction function, WxePermaUrlOptions permaUrlOptions, NameValueCollection postBackCollection)
    {
      PrepareExecuteFunction (function, true);
      if (permaUrlOptions.UsePermaUrl)
      {
        var parameters = new WxePageStepExecutionStates.ExecuteWithPermaUrl.PreparingSubFunctionStateParameters (function, postBackCollection, permaUrlOptions);
        _executionState = new WxePageStepExecutionStates.ExecuteWithPermaUrl.PreparingRedirectToSubFunctionState(this, parameters);
      }
      else
      {
        var parameters = new ExecutionStateParameters (function, postBackCollection);
        _executionState = new WxePageStepExecutionStates.ExecuteWithoutPermaUrl.ExecutingSubFunctionState (this, parameters);
      }

      page.SaveAllState();

      _wxeHandler = page.WxeHandler;
      Execute();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunctionExternalByRedirect (
        IWxePage page, WxeFunction function, WxePermaUrlOptions permaUrlOptions, WxeReturnOptions returnOptions)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);
      ArgumentUtility.CheckNotNull ("returnOptions", returnOptions);

      NameValueCollection postBackCollection = BackupPostBackCollection (page);
      PrepareExecuteFunction (function, false);

      var parameters = new WxePageStepExecutionStates.ExecuteExternalByRedirect.PreparingSubFunctionStateParameters (function, postBackCollection, permaUrlOptions, returnOptions);
      _executionState = new WxePageStepExecutionStates.ExecuteExternalByRedirect.PreparingRedirectToSubFunctionState (this, parameters);
      
      page.SaveAllState();

      _wxeHandler = page.WxeHandler;
      Execute ();
    }

    /// <summary> Gets the token for this page step. </summary>
    /// <include file='doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/PageToken/*' />
    public string PageToken
    {
      get { return _pageToken; }
    }

    //TODO: Remove
    [EditorBrowsable (EditorBrowsableState.Never)]
    public WxeFunction SubFunction
    {
      get { return _executionState != null ? _executionState.Parameters.SubFunction : null; }
    }

    //TODO: Remove
    [EditorBrowsable (EditorBrowsableState.Never)]
    public NameValueCollection PostBackCollection
    {
      get { return _executionState != null ? _executionState.Parameters.PostBackCollection : null; }
    }

    public override string ToString ()
    {
      return "WxePageStep: " + Page;
    }

    /// <summary> Saves the passed <paramref name="state"/> object into the <see cref="WxePageStep"/>. </summary>
    /// <param name="state"> An <b>ASP.NET</b> viewstate object. </param>
    public void SavePageStateToPersistenceMedium (object state)
    {
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      _pageState = writer.ToString();
    }

    /// <summary> 
    ///   Returns the viewstate previsously saved through the <see cref="SavePageStateToPersistenceMedium"/> method. 
    /// </summary>
    /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
    public object LoadPageStateFromPersistenceMedium ()
    {
      LosFormatter formatter = new LosFormatter();
      return formatter.Deserialize (_pageState);
    }

    /// <summary> 
    ///   Aborts the <see cref="WxePageStep"/>. Aborting will cascade to any <see cref="WxeFunction"/> executed
    ///   in the scope of this step if they are part of the same hierarchy, i.e. share a common <see cref="WxeStep.RootFunction"/>.
    /// </summary>
    protected override void AbortRecursive ()
    {
      base.AbortRecursive();
      if (_executionState != null && _executionState.Parameters.SubFunction.RootFunction == this.RootFunction)
        _executionState.Parameters.SubFunction.Abort ();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (WxeUserControl2 userControl, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("function", function);

      BackupPostBackCollection (userControl.WxePage);

      _userControlState = userControl.SaveAllState ();
      _innerFunction = function;
      _userControlID = userControl.UniqueID;
      function.SetParentStep (this);

      _wxeHandler = userControl.WxePage.WxeHandler;
      Execute ();
    }

    private void PrepareExecuteFunction (WxeFunction function, bool isSubFunction)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      if (_executionState != null)
        throw new InvalidOperationException ("Cannot execute function while another function executes.");

      if (isSubFunction)
        function.SetParentStep (this);
    }

    private NameValueCollection BackupPostBackCollection (IWxePage page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      return new NameValueCollection (page.GetPostBackCollection ());
    }

    private void RemoveEventSource (NameValueCollection postBackCollection, Control sender, bool usesEventTarget)
    {
      if (usesEventTarget)
      {
        postBackCollection.Remove (ControlHelper.PostEventSourceID);
        postBackCollection.Remove (ControlHelper.PostEventArgumentID);
      }
      else
      {
        ArgumentUtility.CheckNotNull ("sender", sender);
        if (!(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
        {
          throw new ArgumentException (
              "The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
        }
        postBackCollection.Remove (sender.UniqueID);
      }
    }

    private void SetStateForCurrentFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      //  Use the Page's postback data
      context.PostBackCollection = null;
      context.SetIsReturningPostBack (false);
    }

    public string UserControlID
    {
      get { return _userControlID; }
    }

    public WxeFunction InnerFunction
    {
      get { return _innerFunction; }
    }

    public string UserControlState
    {
      get { return _userControlState; }
    }

    public bool IsReturningInnerFunction
    {
      get { return _isReturningInnerFunction; }
    }

    public IWxePageExecutor PageExecutor
    {
      get { return _pageExecutor; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SetPageExecutor (IWxePageExecutor pageExecutor)
    {
      ArgumentUtility.CheckNotNull ("pageExecutor", pageExecutor);
      _pageExecutor = pageExecutor;
    }

    IExecutionState IExecutionStateContext.ExecutionState
    {
      get { return _executionState; }
    }

    void IExecutionStateContext.SetExecutionState (IExecutionState executionState)
    {
      _executionState = executionState;
    }
  }
}