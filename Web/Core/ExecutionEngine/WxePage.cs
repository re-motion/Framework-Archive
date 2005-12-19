using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> This interface represents a page that can be used in a <see cref="WxePageStep"/>. </summary>
/// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/Class/*' />
public interface IWxePage: ISmartPage, IPage, IWxeTemplateControl
{
  /// <summary> End this page step and continue with the WXE function. </summary>
  void ExecuteNextStep ();

  /// <summary> Executes the <paramref name="function"/> in the current. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function"]' />
  void ExecuteFunction (WxeFunction function);

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl);

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  void ExecuteFunction (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender);
  
  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget);

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl);

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);
  
  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl);
  
  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
  void ExecuteFunctionExternal (WxeFunction function, string target, Control sender, bool returningPostback);
  
  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback);

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl);

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);
  
  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl);
  
  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);

  /// <summary> Gets a flag describing whether this post back has been triggered by returning from a WXE function. </summary>
  bool IsReturningPostBack { get; }

  /// <summary> Gets the WXE function that has been executed in the current page. </summary>
  WxeFunction ReturningFunction { get; }

  /// <summary>
  ///   Gets or sets a flag that determines whether abort the session upon closing the window. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to abort the session upon navigtion away from the page. </value>
  bool IsAbortEnabled { get; }

  /// <summary> 
  ///   Gets a flag whether the status messages (i.e. is submitting, is aborting) will be displayed when the user
  ///   tries to e.g. postback while a request is being processed.
  /// </summary>
  bool AreStatusMessagesEnabled { get; }

  /// <summary> Gets the message displayed when the user attempts to submit while the page is already aborting. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  string StatusIsAbortingMessage { get; }

  /// <summary> 
  ///   Gets the message displayed when the user returnes to a cached page that has already been submited or aborted. 
  /// </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  string StatusIsCachedMessage { get; }


  /// <summary> Gets the permanent URL for the current page. </summary>
  string GetPermanentUrl();
  
  /// <summary> Gets the permanent URL for the current page using the specified <paramref name="queryString"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/GetPermanentUrl/param[@name="queryString"]' />
  string GetPermanentUrl (NameValueCollection queryString);
  
  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
  ///   and using the <paramref name="queryString"/>.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/GetPermanentUrl/param[@name="functionType" or @name="queryString"]' />
  string GetPermanentUrl (Type functionType, NameValueCollection queryString);

  /// <summary> Gets or sets the <see cref="WxeHandler"/> of the current request. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  WxeHandler WxeHandler { get; }
}

/// <summary>
///   <b>WxePage</b> is the default implementation of the <see cref="IWxePage"/> interface. Use this type
///   a base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/Class/*' />
/// <seealso cref="IWxePage"/>
/// <seealso cref="ISmartNavigablePage"/>
public class WxePage: SmartPage, IWxePage, IWindowStateManager
{
  #region IWxePage Impleplementation

  /// <summary> End this page step and continue with the WXE function. </summary>
  public void ExecuteNextStep ()
  {
    _wxeInfo.ExecuteNextStep();
  }

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function"]' />
  public void ExecuteFunction (WxeFunction function)
  {
    _wxeInfo.ExecuteFunction (function);
  }

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxeInfo.ExecuteFunction (function, createPermaUrl, useParentPermaUrl);
  }

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  public void ExecuteFunction (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    _wxeInfo.ExecuteFunction (function, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> 
  ///   field.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
  ///   When in doubt, use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">ExecuteFunctionNoRepost(WxeFunction,Control)</see>.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender, usesEventTarget);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> 
  ///   field.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender, createPermaUrl, useParentPermaUrl);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> 
  ///   field.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
  ///   When in doubt, use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean)">ExecuteFunctionNoRepost(WxeFunction,Control,Boolean,Boolean)</see>.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
  ///   When in doubt, use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">ExecuteFunctionNoRepost(WxeFunction,Control,Boolean,Boolean,NameValueCollection)</see>.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    _wxeInfo.ExecuteFunctionNoRepost (
        function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
  public void ExecuteFunctionExternal (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    _wxeInfo.ExecuteFunctionExternal (function, target, sender, returningPostback);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    _wxeInfo.ExecuteFunctionExternal (function, target, features, sender, returningPostback);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxeInfo.ExecuteFunctionExternal (function, target, sender, returningPostback, createPermaUrl, useParentPermaUrl);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    _wxeInfo.ExecuteFunctionExternal (
        function, target, sender, returningPostback, createPermaUrl, useParentPermaUrl, urlParameters);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxeInfo.ExecuteFunctionExternal (function, target, features, sender, returningPostback,  createPermaUrl, useParentPermaUrl);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    _wxeInfo.ExecuteFunctionExternal (
        function, target, features, sender, returningPostback,  createPermaUrl, useParentPermaUrl, urlParameters);
  }

  /// <summary> Gets a flag describing whether this post back has been triggered by returning from a WXE function. </summary>
  [Browsable (false)]
  public bool IsReturningPostBack
  {
    get { return _wxeInfo.IsReturningPostBack; }
  }

  /// <summary> Gets the WXE function that has been executed in the current page. </summary>
  [Browsable (false)]
  public WxeFunction ReturningFunction
  {
    get { return _wxeInfo.ReturningFunction; }
  }


  /// <summary> Gets the permanent URL for the current page. </summary>
  public string GetPermanentUrl ()
  {
    return _wxeInfo.GetPermanentUrl ();
  }
  
  /// <summary> Gets the permanent URL for the current page using the specified <paramref name="queryString"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/GetPermanentUrl/param[@name="queryString"]' />
  public string GetPermanentUrl (NameValueCollection queryString)
  {
    return _wxeInfo.GetPermanentUrl (queryString);
  }
  
  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
  ///   and using the <paramref name="queryString"/>.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/GetPermanentUrl/param[@name="functionType" or @name="queryString"]' />
  public string GetPermanentUrl (Type functionType, NameValueCollection queryString)
  {
    return _wxeInfo.GetPermanentUrl (functionType, queryString);
  }

  #endregion
  
  #region IWindowStateManager Implementation

  object IWindowStateManager.GetData (string key)
  {
    return _wxeInfo.GetData (key);
  }

  void IWindowStateManager.SetData (string key, object value)
  {
    _wxeInfo.SetData (key, value);
  }
  
  #endregion

  private WxePageInfo _wxeInfo;
  private bool disposed;
  private NaBooleanEnum _enableAbort = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableStatusMessages = NaBooleanEnum.Undefined;
  private string _statusIsAbortingMessage = string.Empty;
  private string _statusIsCachedMessage = string.Empty;

  public WxePage ()
  {
    _wxeInfo = new WxePageInfo (this);
    disposed = false;
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = _wxeInfo.EnsurePostBackModeDetermined (Context);
    _wxeInfo.Initialize (Context);

#if NET11
    base.OnPreInit();
#endif

    return result;
  }


  protected override void SavePageStateToPersistenceMedium (object viewState)
  {
    bool isViewStateInSession = true;
    if (isViewStateInSession)
      _wxeInfo.SavePageStateToPersistenceMedium (viewState);
    else
      base.SavePageStateToPersistenceMedium (viewState);
  }

  protected override object LoadPageStateFromPersistenceMedium()
  {
    bool isViewStateInSession = true;
    if (isViewStateInSession)
      return _wxeInfo.LoadPageStateFromPersistenceMedium ();
    else
      return base.LoadPageStateFromPersistenceMedium ();
  }

  /// <summary> Gets the post back data for the page. </summary>
  /// <remarks> Application developers should only rely on this collection for accessing the post back data. </remarks>
  public NameValueCollection GetPostBackCollection ()
  {
    return _wxeInfo.EnsurePostBackModeDetermined (Context);
  }

  NameValueCollection ISmartPage.GetPostBackCollection ()
  {
    return GetPostBackCollection();
  }

  protected override void OnPreRender (EventArgs e)
  {
    // wxeInfo.PreRender() must be called before base.OnPreRender (EventArgs)
    // Base-Implementation calls _smartPageInfo.PreRender(), which must be called after wxeInfo.PreRender()
    _wxeInfo.PreRender();

    base.OnPreRender (e);
  }

  /// <summary> Gets the <see cref="WxePageStep"/> that called this <see cref="WxePage"/>. </summary>
  [Browsable (false)]
  public WxePageStep CurrentStep
  {
    get { return _wxeInfo.CurrentStep; }
  }
  
  /// <summary> Gets the <see cref="WxeFunction"/> of which the <see cref="CurrentStep"/> is a part. </summary>
  /// <value> 
  ///   A <see cref="WxeFunction"/> or <see langwrpd="null"/> if the <see cref="CurrentStep"/> is not part of a
  ///   <see cref="WxeFunction"/>.
  /// </value>
  [Browsable (false)]
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  /// <summary> Gets the <see cref="WxeStep.Variables"/> collection of the <see cref="CurrentStep"/>. </summary>
  /// <value> 
  ///   A <see cref="NameObjectCollection"/> or <see langword="null"/> if the step is not part of a 
  ///   <see cref="WxeFunction"/>
  /// </value>
  [Browsable (false)]
  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  /// <summary> Gets the <see cref="WxeForm"/> of this page. </summary>
  protected WxeForm WxeForm
  {
    get { return _wxeInfo.WxeForm; }
  }

  /// <summary> Gets or sets the <see cref="WxeHandler"/> of the current request. </summary>
  WxeHandler IWxePage.WxeHandler 
  { 
    get { return _wxeInfo.WxeHandler; }
  }

  /// <summary> Disposes the page. </summary>
  /// <remarks>
  ///   <b>Dispose</b> is part of the ASP.NET page execution life cycle. It does not actually implement the 
  ///   disposeable pattern.
  ///   <note type="inheritinfo">
  ///     Do not override this method. Use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.Dispose(System.Boolean)">Dispose(Boolean)</see>
  ///     instead.
  ///   </note>
  /// </remarks>
  public override void Dispose()
  {
    base.Dispose ();
    if (! disposed)
    {
      Dispose (true);
      disposed = true;
      _wxeInfo.Dispose();
    }
  }

  /// <summary> Disposes the page. </summary>
  protected virtual void Dispose (bool disposing)
  {
  }


  /// <summary> Gets or sets the flag that determines whether to abort the session upon closing the window. </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to abort the session. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsAbortEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to abort the session when the window is closed. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableAbort
  {
    get { return _enableAbort; }
    set { _enableAbort = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableAbort"/> property. </summary>
  /// <value>
  ///   <see langowrd="false"/> if <see cref="EnableAbort"/> is <see cref="NaBooleanEnum.False"/>. 
  /// </value>
  protected virtual bool IsAbortEnabled
  {
    get { return _enableAbort != NaBooleanEnum.False; }
  }

  /// <summary> Implementation of <see cref="IWxePage.IsAbortEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsAbortEnabled"/>. </value>
  bool IWxePage.IsAbortEnabled
  {
    get { return IsAbortEnabled; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableAbortConfirmation"/> property. </summary>
  /// <value> 
  ///   <see langowrd="true"/> if <see cref="EnableAbortConfirmation"/> and <see cref="IsAbortEnabled"/> are
  ///   <see cref="NaBooleanEnum.True"/>. 
  /// </value>
  protected override bool IsAbortConfirmationEnabled
  {
    get { return IsAbortEnabled && base.IsAbortConfirmationEnabled; }
  }

  /// <summary> 
  ///   Gets or sets the flag that determines whether to display a message when the user tries to start a second
  ///   request or returns to a page that has already been submittet (i.e. a cached page).
  /// </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to enable the status messages. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="AreStatusMessagesEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a status message when the user attempts to start a "
             + "second request or returns to a page that has already been submitted (i.e. a cached page). "
             + "Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableStatusMessages
  {
    get { return _enableStatusMessages; }
    set { _enableStatusMessages = value; }
  }

  /// <summary> 
  ///   Gets a flag whether the status messages (i.e. is submitting, is aborting) will be displayed when the user
  ///   tries to e.g. postback while a request is being processed.
  /// </summary>
  protected virtual bool AreStatusMessagesEnabled
  {
    get { return _enableStatusMessages != NaBooleanEnum.False; }
  }

  /// <summary> Implementation of <see cref="IWxePage.AreStatusMessagesEnabled"/>. </summary>
  /// <value> The value returned by <see cref="AreStatusMessagesEnabled"/>. </value>
  bool IWxePage.AreStatusMessagesEnabled
  {
    get { return AreStatusMessagesEnabled; }
  }

  /// <exclude/>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public override NaBooleanEnum EnableStatusIsSubmittingMessage
  {
    get
    {
      return base.EnableStatusIsSubmittingMessage;
    }
    set
    {
      base.EnableStatusIsSubmittingMessage = value;
    }
  }

  /// <summary> Overridden to return the value of <see cref="AreStatusMessagesEnabled"/>. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected override bool IsStatusIsSubmittingMessageEnabled
  {
    get { return AreStatusMessagesEnabled; }
  }

  /// <summary> 
  ///   Gets or sets the message displayed when the user attempts to submit while the page is already aborting. 
  /// </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to submit while the page is already aborting.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string StatusIsAbortingMessage
  {
    get { return _statusIsAbortingMessage; }
    set { _statusIsAbortingMessage = StringUtility.NullToEmpty (value); }
  }

  /// <summary> 
  ///   Gets or sets the message displayed when the user returnes to a cached page that has already been submitted 
  ///   or aborted. 
  /// </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user returnes to a cached page that has already been submitted or aborted.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string StatusIsCachedMessage
  {
    get { return _statusIsCachedMessage; }
    set { _statusIsCachedMessage = StringUtility.NullToEmpty (value); }
  }

}

}
