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
public interface IWxePage: ISmartPage, IWxeTemplateControl
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
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender);
  
  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget);

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl);

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);
  
  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl);
  
  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
  ///   new function.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
  ///   new function.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters,
      bool returnToCaller, NameValueCollection callerUrlParameters);

  #region ExecuteFunctionExternalNoRepost
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
//  void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);
//
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
//  void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, bool usesEventTarget, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters);
//
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
//  void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters,
//      bool returnToCaller, NameValueCollection callerUrlParameters);
//
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
//  void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, bool usesEventTarget, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters,
//      bool returnToCaller, NameValueCollection callerUrlParameters);
  #endregion

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
  void ExecuteFunctionExternal (WxeFunction function, string target, Control sender, bool returningPostback);
  
  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback);

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl);

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);
  
  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl);
  
  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters);

  /// <summary> Gets a flag describing whether this post-back has been triggered by returning from a WXE function. </summary>
  bool IsReturningPostBack { get; }

  /// <summary> Gets the WXE function that has been executed in the current page. </summary>
  WxeFunction ReturningFunction { get; }

  /// <summary>
  ///   Gets a flag that determines whether to abort the session upon closing the window. 
  ///  </summary>
  /// <value> <see langword="true"/> to abort the session upon navigtion away from the page. </value>
  bool IsAbortEnabled { get; }

  /// <summary>
  ///   Gets a flag that determines whether to allow out-of-sequence postbacks (i.e. post-backs from an already 
  ///   submitted page because of the cache). 
  ///  </summary>
  /// <value> <see langword="true"/> to enable out of sequence post-backs. </value>
  /// <remarks> 
  ///   <see cref="AreOutOfSequencePostBacksEnabled"/> should only return <see langword="true"/> if 
  ///   <see cref="IsAbortEnabled"/> evaluates <see langword="false"/>.
  /// </remarks>
  bool AreOutOfSequencePostBacksEnabled { get; }

  /// <summary>
  ///   Gets a flag that describes whether the current postback cycle was caused by resubmitting a page from the 
  ///   client's cache.
  /// </summary>
  /// <value> <see langword="true"/> if the page has been re-submitted. </value>
  bool IsOutOfSequencePostBack { get; }

  /// <summary> 
  ///   Gets a flag whether the status messages (i.e. is submitting, is aborting) will be displayed when the user
  ///   tries to e.g. postback while a request is being processed.
  /// </summary>
  bool AreStatusMessagesEnabled { get; }

  /// <summary> Gets the message displayed when the user attempts to submit while the page is already aborting. </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  string StatusIsAbortingMessage { get; }

  /// <summary> 
  ///   Gets the message displayed when the user returnes to a cached page that has already been submited or aborted. 
  /// </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  string StatusIsCachedMessage { get; }

  /// <summary> Gets the permanent URL parameters the current page. </summary>
  NameValueCollection GetPermanentUrlParameters();

  /// <summary> Gets the permanent URL for the current page. </summary>
  string GetPermanentUrl();
  
  /// <summary> Gets the permanent URL for the current page using the specified <paramref name="queryString"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' 
  ///     path='IWxePage/GetPermanentUrl/param[@name="queryString"]' />
  string GetPermanentUrl (NameValueCollection queryString);
  
  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
  ///   and using the <paramref name="queryString"/>.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' 
  ///     path='IWxePage/GetPermanentUrl/param[@name="functionType" or @name="queryString"]' />
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
public class WxePage: SmartPage, IWxePage, IWindowStateManager
{
  #region IWxePage Impleplementation

  /// <summary> End this page step and continue with the WXE function. </summary>
  public void ExecuteNextStep ()
  {
    _wxePageInfo.ExecuteNextStep();
  }

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function"]' />
  public void ExecuteFunction (WxeFunction function)
  {
    _wxePageInfo.ExecuteFunction (function);
  }

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxePageInfo.ExecuteFunction (function, createPermaUrl, useParentPermaUrl);
  }

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
  public void ExecuteFunction (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
  {
    _wxePageInfo.ExecuteFunction (function, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> 
  ///   field.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    _wxePageInfo.ExecuteFunctionNoRepost (function, sender);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
  ///   When in doubt, use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">ExecuteFunctionNoRepost(WxeFunction,Control)</see>.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    _wxePageInfo.ExecuteFunctionNoRepost (function, sender, usesEventTarget);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
  ///   on returning.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> 
  ///   field.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxePageInfo.ExecuteFunctionNoRepost (function, sender, createPermaUrl, useParentPermaUrl);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
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
    _wxePageInfo.ExecuteFunctionNoRepost (function, sender, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
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
    _wxePageInfo.ExecuteFunctionNoRepost (function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl);
  }

  /// <summary>
  ///   Executes the <paramref name="function"/> in the current window without triggering the current post-back event 
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
    _wxePageInfo.ExecuteFunctionNoRepost (
        function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, permaUrlParameters);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
  ///   new function.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
     _wxePageInfo.ExecuteFunctionExternal (function, createPermaUrl, useParentPermaUrl, urlParameters);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
  ///   new function.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters,
      bool returnToCaller, NameValueCollection callerUrlParameters)
  {
     _wxePageInfo.ExecuteFunctionExternal (function, createPermaUrl, useParentPermaUrl, urlParameters, returnToCaller, callerUrlParameters);
  }

  #region ExecuteFunctionExternalNoRepost
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
//  {
//     _wxePageInfo.ExecuteFunctionExternalNoRepost (function, sender, createPermaUrl, useParentPermaUrl, urlParameters);
//  }
//
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, bool usesEventTarget, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
//  {
//     _wxePageInfo.ExecuteFunctionExternalNoRepost (function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, urlParameters);
//  }
//
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters,
//      bool returnToCaller, NameValueCollection callerUrlParameters)
//  {
//     _wxePageInfo.ExecuteFunctionExternalNoRepost (function, sender, createPermaUrl, useParentPermaUrl, urlParameters, returnToCaller, callerUrlParameters);
//  }
//
//  /// <summary> 
//  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
//  ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the 
//  ///   new function.
//  /// </summary>
//  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternalNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
//  public void ExecuteFunctionExternalNoRepost (
//      WxeFunction function, Control sender, bool usesEventTarget, 
//      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters,
//      bool returnToCaller, NameValueCollection callerUrlParameters)
//  {
//     _wxePageInfo.ExecuteFunctionExternalNoRepost (function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, urlParameters, returnToCaller, callerUrlParameters);
//  }
  #endregion

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame by through a javascript call.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
  public void ExecuteFunctionExternal (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    _wxePageInfo.ExecuteFunctionExternal (function, target, sender, returningPostback);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    _wxePageInfo.ExecuteFunctionExternal (function, target, features, sender, returningPostback);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxePageInfo.ExecuteFunctionExternal (function, target, sender, returningPostback, createPermaUrl, useParentPermaUrl);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    _wxePageInfo.ExecuteFunctionExternal (
        function, target, sender, returningPostback, createPermaUrl, useParentPermaUrl, urlParameters);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    _wxePageInfo.ExecuteFunctionExternal (
      function, target, features, sender, returningPostback,  createPermaUrl, useParentPermaUrl);
  }

  /// <summary> 
  ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
  ///   specified window or frame through javascript window.open(...).
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
  {
    _wxePageInfo.ExecuteFunctionExternal (
        function, target, features, sender, returningPostback,  createPermaUrl, useParentPermaUrl, urlParameters);
  }

  /// <summary> Gets a flag describing whether this post-back has been triggered by returning from a WXE function. </summary>
  [Browsable (false)]
  public bool IsReturningPostBack
  {
    get { return _wxePageInfo.IsReturningPostBack; }
  }

  /// <summary> Gets the WXE function that has been executed in the current page. </summary>
  [Browsable (false)]
  public WxeFunction ReturningFunction
  {
    get { return _wxePageInfo.ReturningFunction; }
  }

  /// <summary> Gets the permanent URL parameters the current page. </summary>
  public NameValueCollection GetPermanentUrlParameters()
  {
    return _wxePageInfo.GetPermanentUrlParameters();
  }

  /// <summary> Gets the permanent URL for the current page. </summary>
  public string GetPermanentUrl ()
  {
    return _wxePageInfo.GetPermanentUrl ();
  }
  
  /// <summary> Gets the permanent URL for the current page using the specified <paramref name="queryString"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/GetPermanentUrl/param[@name="queryString"]' />
  public string GetPermanentUrl (NameValueCollection queryString)
  {
    return _wxePageInfo.GetPermanentUrl (queryString);
  }
  
  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
  ///   and using the <paramref name="queryString"/>.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/GetPermanentUrl/param[@name="functionType" or @name="queryString"]' />
  public string GetPermanentUrl (Type functionType, NameValueCollection queryString)
  {
    return _wxePageInfo.GetPermanentUrl (functionType, queryString);
  }

  /// <summary> 
  ///   Gets or sets the message displayed when the user attempts to submit while the page is already aborting. 
  /// </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to submit while the page is already aborting.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string StatusIsAbortingMessage
  {
    get { return _wxePageInfo.StatusIsAbortingMessage; }
    set { _wxePageInfo.StatusIsAbortingMessage = value; }
  }

  /// <summary> 
  ///   Gets or sets the message displayed when the user returnes to a cached page that has already been submitted 
  ///   or aborted. 
  /// </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user returnes to a cached page that has already been submitted or aborted.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string StatusIsCachedMessage
  {
    get { return _wxePageInfo.StatusIsCachedMessage; }
    set { _wxePageInfo.StatusIsCachedMessage = value; }
  }

  /// <summary> Gets or sets the <see cref="WxeHandler"/> of the current request. </summary>
  WxeHandler IWxePage.WxeHandler 
  { 
    get { return _wxePageInfo.WxeHandler; }
  }
  
  #endregion
  
  #region IWindowStateManager Implementation

  object IWindowStateManager.GetData (string key)
  {
    return _wxePageInfo.GetData (key);
  }

  void IWindowStateManager.SetData (string key, object value)
  {
    _wxePageInfo.SetData (key, value);
  }
  
  #endregion

  private WxePageInfo _wxePageInfo;
  private bool disposed;
  private NaBooleanEnum _enableOutOfSequencePostBacks = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableAbort = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableStatusMessages = NaBooleanEnum.Undefined;

  public WxePage ()
  {
    _wxePageInfo = new WxePageInfo (this);
    disposed = false;
  }


  /// <summary> Overrides <see cref="Page.DeterminePostBackMode"/>. </summary>
  /// <remarks> Uses <see cref="WxePageInfo.EnsurePostBackModeDetermined"/> determine the postback mode. </remarks>
  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = _wxePageInfo.EnsurePostBackModeDetermined (Context);
    _wxePageInfo.Initialize (Context);

    return result;
  }

  /// <summary> Gets the post-back data for the page. </summary>
  /// <remarks> Application developers should only rely on this collection for accessing the post-back data. </remarks>
  public NameValueCollection GetPostBackCollection ()
  {
    return _wxePageInfo.EnsurePostBackModeDetermined (Context);
  }

  /// <summary> Gets the value returned by <see cref="GetPostBackCollection"/>. </summary>
  NameValueCollection ISmartPage.GetPostBackCollection ()
  {
    return GetPostBackCollection();
  }


  /// <remarks> Uses <see cref="WxePageInfo.SavePageStateToPersistenceMedium"/> to save the viewstate. </remarks>
  protected override void SavePageStateToPersistenceMedium (object viewState)
  {
    bool isViewStateInSession = true;
    if (isViewStateInSession)
      _wxePageInfo.SavePageStateToPersistenceMedium (viewState);
    else
      base.SavePageStateToPersistenceMedium (viewState);
  }

  /// <remarks> Uses <see cref="WxePageInfo.LoadPageStateFromPersistenceMedium"/> to load the viewstate. </remarks>
  protected override object LoadPageStateFromPersistenceMedium()
  {
    bool isViewStateInSession = true;
    if (isViewStateInSession)
      return _wxePageInfo.LoadPageStateFromPersistenceMedium ();
    else
      return base.LoadPageStateFromPersistenceMedium ();
  }


  /// <remarks> Invokes <see cref="WxePageInfo.PreRender"/> before calling the base-implementation. </remarks>
  protected override void OnPreRender (EventArgs e)
  {
    // wxeInfo.PreRender() must be called before base.OnPreRender (EventArgs)
    // Base-Implementation uses SmartPageInfo, which registers for the PreRender event.
    _wxePageInfo.PreRender();

    base.OnPreRender (e);
  }

  
  /// <summary> Gets the <see cref="WxePageStep"/> that called this <see cref="WxePage"/>. </summary>
  [Browsable (false)]
  public WxePageStep CurrentStep
  {
    get { return _wxePageInfo.CurrentStep; }
  }
  
  /// <summary> Gets the <see cref="WxeFunction"/> of which the <see cref="CurrentStep"/> is a part. </summary>
  /// <value> 
  ///   A <see cref="WxeFunction"/> or <see langwrpd="null"/> if the <see cref="CurrentStep"/> is not part of a
  ///   <see cref="WxeFunction"/>.
  /// </value>
  [Browsable (false)]
  public WxeFunction CurrentFunction
  {
    get { return _wxePageInfo.CurrentFunction; }
  }

  /// <summary> Gets the <see cref="WxeStep.Variables"/> collection of the <see cref="CurrentStep"/>. </summary>
  /// <value> 
  ///   A <see cref="NameObjectCollection"/> or <see langword="null"/> if the step is not part of a 
  ///   <see cref="WxeFunction"/>
  /// </value>
  [Browsable (false)]
  public NameObjectCollection Variables 
  {
    get { return _wxePageInfo.Variables; }
  }


  /// <summary> Gets the <see cref="WxeForm"/> of this page. </summary>
  protected WxeForm WxeForm
  {
    get { return _wxePageInfo.WxeForm; }
  }

  
  /// <summary> Disposes the page. </summary>
  /// <remarks>
  ///   <b>Dispose</b> is part of the ASP.NET page execution life cycle. It does not actually implement the 
  ///   disposeable pattern.
  ///   <note type="inheritinfo">
  ///     Do not override this method.
  ///     Use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.Dispose(System.Boolean)">Dispose(Boolean)</see> instead.
  ///   </note>
  /// </remarks>
  public override void Dispose()
  {
    base.Dispose ();
    if (! disposed)
    {
      Dispose (true);
      disposed = true;
      _wxePageInfo.Dispose();
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
  ///   <see langword="false"/> if <see cref="EnableAbort"/> is <see cref="NaBooleanEnum.False"/>.
  /// </value>
  protected virtual bool IsAbortEnabled
  {
    get { return _enableAbort != NaBooleanEnum.False; }
  }

  /// <summary> Gets the value returned by <see cref="IsAbortEnabled"/>. </summary>
  bool IWxePage.IsAbortEnabled
  {
    get { return IsAbortEnabled; }
  }

  /// <summary>
  ///   Gets a flag that determines whether to allow out-of-sequence postbacks (i.e. post-backs from an already 
  ///   submitted page because of the cache). 
  /// </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> enable out of sequence post-backs.
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.False"/>.
  /// </value>
  /// <remarks>
  ///   <para>
  ///     Use <see cref="AreOutOfSequencePostBacksEnabled"/> to evaluate this property.
  ///   </para><para>
  ///     Setting this flag disables the function abort and the abort confirmation message.
  ///   </para>
  /// </remarks>
  [Description(  "The flag that determines whether to allow out-of-sequence postbacks (i.e. post-backs from an already "
               + "submitted page because of the cache). Undefined is interpreted as false.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableOutOfSequencePostBacks
  {
    get { return _enableOutOfSequencePostBacks; }
    set { _enableOutOfSequencePostBacks = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableOutOfSequencePostBacks"/> property. </summary>
  /// <value>
  ///   <see langword="true"/> if <see cref="EnableOutOfSequencePostBacks"/> is <see cref="NaBooleanEnum.True"/>
  ///   and <see cref="IsAbortEnabled"/> evaluates <see langword="false"/>.
  /// </value>
  protected virtual bool AreOutOfSequencePostBacksEnabled
  {
    get { return _enableOutOfSequencePostBacks == NaBooleanEnum.True && ! IsAbortEnabled; }
  }

  /// <summary> Gets the value returned by <see cref="AreOutOfSequencePostBacksEnabled"/>. </summary>
  bool IWxePage.AreOutOfSequencePostBacksEnabled 
  { 
    get { return AreOutOfSequencePostBacksEnabled; }
  }

  /// <summary>
  ///   Gets a flag that describes whether the current postback cycle was caused by resubmitting a page from the 
  ///   client's cache.
  /// </summary>
  /// <value> <see langword="true"/> if the page has been re-submitted. </value>
  public bool IsOutOfSequencePostBack 
  { 
    get { return _wxePageInfo.IsOutOfSequencePostBack; }
  }

  /// <summary> Gets the evaluated value for the <see cref="ShowAbortConfirmation"/> property. </summary>
  /// <value> 
  ///   <see langword="true"/> if <see cref="SmartPage.IsAbortConfirmationEnabled"/> and <see cref="IsAbortEnabled"/> 
  ///   evaluate <see langword="true"/>. 
  /// </value>
  protected override bool IsAbortConfirmationEnabled
  {
    get { return IsAbortEnabled && base.IsAbortConfirmationEnabled; }
  }

  /// <summary> 
  ///   Gets the value of the base class's <see cref="SmartPage.IsDirtyStateTrackingEnabled"/> property ANDed with
  ///   <see cref="IsAbortEnabled"/>.
  /// </summary>
  /// <value> 
  ///   <see langword="true"/> if <see cref="SmartPage.IsDirtyStateTrackingEnabled"/> and <see cref="IsAbortEnabled"/> 
  ///   evaluate <see langword="true"/>. 
  /// </value>
  protected override bool IsDirtyStateTrackingEnabled
  {
    get { return IsAbortEnabled && base.IsDirtyStateTrackingEnabled; }
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

  /// <summary> Gets the value returned by <see cref="AreStatusMessagesEnabled"/>. </summary>
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

}

}
