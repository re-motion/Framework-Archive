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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Encapsulates execute logic for WXE functions.
  /// </summary>
  /// <remarks>
  /// Dispose the <see cref="WxeExecutor"/> at the end of the page life cycle, i.e. in the <see cref="Control.Dispose"/> method.
  /// </remarks>
  public class WxeExecutor : IDisposable
  {
    internal static bool GetUsesEventTarget (IWxePage page)
    {
      NameValueCollection postBackCollection = page.GetPostBackCollection ();
      if (postBackCollection == null)
      {
        if (page.IsPostBack)
          throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
        return false;
      }
      return !StringUtility.IsNullOrEmpty (postBackCollection[ControlHelper.PostEventSourceID]);
    }
    
    private readonly HttpContext _httpContext;

    private readonly IWxePage _page;
    private readonly WxePageInfo _wxePageInfo;

    public WxeExecutor (HttpContext context, IWxePage page, WxePageInfo wxePageInfo)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("wxePageInfo", wxePageInfo);

      _wxePageInfo = wxePageInfo;
      _page = page;
      _httpContext = context;
      _httpContext.Handler = page;
    }

    /// <summary>
    /// Invoke <see cref="IDisposable.Dispose"/> at the end of the page life cycle, i.e. in the <see cref="Control.Dispose"/> method.
    /// </summary>
    void IDisposable.Dispose ()
    {
      _httpContext.Handler = _wxePageInfo.WxeHandler;
    }

    public HttpContext HttpContext
    {
      get { return _httpContext; }
    }

    public void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
    {
      try
      {
        _httpContext.Handler = _wxePageInfo.WxeHandler;
        _wxePageInfo.CurrentStep.ExecuteFunction (_page, function, createPermaUrl, useParentPermaUrl, permaUrlParameters);
      }
      finally
      {
        _httpContext.Handler = _page;
      }
    }

    public void ExecuteFunctionNoRepost (
        WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
    {
      try
      {
        _httpContext.Handler = _wxePageInfo.WxeHandler;
        _wxePageInfo.CurrentStep.ExecuteFunctionNoRepost (_page, function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, permaUrlParameters);
      }
      finally
      {
        _httpContext.Handler = _page;
      }
    }

    /// <summary> 
    ///   Gets a flag describing whether the post back was most likely caused by the ASP.NET post back mechanism.
    /// </summary>
    /// <value> <see langword="true"/> if the post back collection contains the <b>__EVENTTARGET</b> field. </value>
    protected bool UsesEventTarget
    {
      get { return GetUsesEventTarget (_page); }
    }

    public void ExecuteFunctionExternal (
        WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters,
        bool returnToCaller, NameValueCollection callerUrlParameters)
    {
      try
      {
        _httpContext.Handler = _wxePageInfo.WxeHandler;
        _wxePageInfo.CurrentStep.ExecuteFunctionExternal (_page, function, createPermaUrl, useParentPermaUrl, urlParameters, returnToCaller, callerUrlParameters);
      }
      finally
      {
        _httpContext.Handler = _page;
      }
    }

    public void ExecuteFunctionExternal (
        WxeFunction function, string target, string features, Control sender, bool returningPostback,
        bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNullOrEmpty ("target", target);

      string functionToken = _wxePageInfo.CurrentStep.GetFunctionTokenForExternalFunction (function, returningPostback);

      string href = _wxePageInfo.CurrentStep.GetDestinationUrlForExternalFunction (
          function, functionToken, createPermaUrl, useParentPermaUrl, urlParameters);

      string openScript;
      if (features != null)
        openScript = string.Format ("window.open('{0}', '{1}', '{2}');", href, target, features);
      else
        openScript = string.Format ("window.open('{0}', '{1}');", href, target);
      ScriptUtility.RegisterStartupScriptBlock ((Page) _page, "WxeExecuteFunction", openScript);

      function.ReturnUrl = "javascript:" + GetClosingScriptForExternalFunction (functionToken, sender, returningPostback);
    }

    /// <summary> Gets the client script to be used as the return URL for the window of the external function. </summary>
    private string GetClosingScriptForExternalFunction (string functionToken, Control sender, bool returningPostback)
    {
      if (!returningPostback)
      {
        return "window.close();";
      }
      else if (UsesEventTarget)
      {
        NameValueCollection postBackCollection = _page.GetPostBackCollection ();
        if (postBackCollection == null)
          throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");

        string eventTarget = postBackCollection[ControlHelper.PostEventSourceID];
        string eventArgument = postBackCollection[ControlHelper.PostEventArgumentID];
        return FormatDoPostBackClientScript (
            functionToken, _page.CurrentStep.PageToken, sender.ClientID, eventTarget, eventArgument);
      }
      else
      {
        ArgumentUtility.CheckNotNull ("sender", sender);
        if (!(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
          throw new ArgumentException ("The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
        return FormatDoSubmitClientScript (functionToken, _page.CurrentStep.PageToken, sender.ClientID);
      }
    }

    /// <summary> 
    ///   Gets the client script used to execute <c>__dopostback</c> in the parent form before closing the window of the 
    ///   external function.
    /// </summary>
    private string FormatDoPostBackClientScript (string functionToken, string pageToken, string senderID, string eventTarget, string eventArgument)
    {
      return string.Format (
          "\r\n"
          + "if (   window.opener != null \r\n"
          + "    && ! window.opener.closed \r\n"
          + "    && window.opener.wxeDoPostBack != null \r\n"
          + "    && window.opener.document.getElementById('{0}') != null \r\n"
          + "    && window.opener.document.getElementById('{0}').value == '{1}') \r\n"
          + "{{ \r\n"
          + "  window.opener.wxeDoPostBack('{2}', '{3}', '{4}'); \r\n"
          + "}} \r\n"
          + "window.close(); \r\n",
          WxePageInfo.PageTokenID, pageToken, eventTarget, eventArgument, functionToken);
    }

    /// <summary> 
    ///   Gets the client script used to submit the parent form before closing the window of the external function. 
    /// </summary>
    private string FormatDoSubmitClientScript (string functionToken, string pageToken, string senderID)
    {
      return string.Format (
          "\r\n"
          + "if (   window.opener != null \r\n"
          + "    && ! window.opener.closed \r\n"
          + "    && window.opener.wxeDoSubmit != null \r\n"
          + "    && window.opener.document.getElementById('{0}') != null \r\n"
          + "    && window.opener.document.getElementById('{0}').value == '{1}') \r\n"
          + "{{ \r\n"
          + "  window.opener.wxeDoSubmit('{2}', '{3}'); \r\n"
          + "}} \r\n"
          + "window.close(); \r\n",
          WxePageInfo.PageTokenID, pageToken, senderID, functionToken);
    }
  }
}