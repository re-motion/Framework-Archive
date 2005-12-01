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

/// <summary> Specifies the client side events supported for registration by the <see cref="IWxePage"/>. </summary>
public enum WxePageEvents
{
  /// <summary> Rasied when the document has finished loading. Signature: <c>void Function ()</c> </summary>
  OnLoad,
  /// <summary> Raised when the user posts back to the server. Signature: <c>void Function (eventTargetID, eventArgs</c> </summary>
  OnPostBack,
  /// <summary> Raised when the user leaves the page. Signature: <c>void Function ()</c> </summary>
  OnAbort,
  /// <summary> Raised when the user scrolls the page. Signature: <c>void Function ()</c> </summary>
  OnScroll,
  /// <summary> Raised when the user resizes the page. Signature: <c>void Function ()</c> </summary>
  OnResize,
  /// <summary> 
  ///   Raised before the request to load a new page (or reload the current page) is executed. Not supported in Opera.
  ///   Signature: <c>void Function ()</c>
  /// </summary>
  OnBeforeUnload,
  /// <summary> Raised before the page is removed from the window. Signature: <c>void Function ()</c> </summary>
  OnUnload
}

/// <summary> This interface represents a page that can be used in a <see cref="WxePageStep"/>. </summary>
/// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/Class/*' />
public interface IWxePage: IPage, IWxeTemplateControl
{
  /// <summary> Gets the post back data for the page. </summary>
  /// <remarks> Application developers should only rely on this collection for accessing the post back data. </remarks>
  NameValueCollection GetPostBackCollection ();

  /// <summary> End this page step and continue with the WXE function. </summary>
  void ExecuteNextStep ();

  /// <summary> Executes the <paramref name="function"/> in the current. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function"]' />
  void ExecuteFunction (WxeFunction function);

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
  void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl);

  /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  void ExecuteFunction (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString);

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
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString);
  
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
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString);

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
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString);
  
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
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString);

  /// <summary> Gets a flag describing whether this post back has been triggered by returning from a WXE function. </summary>
  bool IsReturningPostBack { get; }

  /// <summary> Gets the WXE function that has been executed in the current page. </summary>
  WxeFunction ReturningFunction { get; }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display a confirmation dialog before aborting the session. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to display the confirmation dialog. </value>
  bool IsAbortConfirmationEnabled { get; }

  /// <summary>
  ///   Gets or sets a flag that determines whether abort the session upon closing the window. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to abort the session upon navigtion away from the page. </value>
  bool IsAbortEnabled { get; }

  /// <summary> Gets the message displayed when the user attempts to abort the WXE Function. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  string AbortMessage { get; }

  /// <summary> 
  ///   Gets a flag whether the status messages (i.e. is submitting, is aborting) will be displayed when the user
  ///   tries to e.g. postback while a request is being processed.
  /// </summary>
  bool AreStatusMessagesEnabled { get; }

  /// <summary> Gets the message displayed when the user attempts to submit while the page is already submitting. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  string StatusIsSubmittingMessage { get; }

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

  /// <summary> Registers a Java Script function to be executed when the page is aborted. </summary>
  /// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/RegisterClientSidePageEventHandler/*' />
  void RegisterClientSidePageEventHandler (WxePageEvents pageEvent, string key, string function);

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

  /// <summary> Gets or sets the <see cref="HtmlForm"/> of the ASP.NET page. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }

}

/// <summary>
///   <b>WxePage</b> is the default implementation of the <see cref="IWxePage"/> interface. Use this type
///   a base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/Class/*' />
/// <seealso cref="IWxePage"/>
/// <seealso cref="ISmartNavigablePage"/>
public class WxePage: Page, IWxePage, ISmartNavigablePage, IWindowStateManager
{
  /// <summary> Obsolete. Executes the <paramref name="function"/> in another window or frame. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
  [Obsolete ("Use ExecuteFunctionExternal (WxeFunction, string, Control, bool) instead.")]
  public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    ExecuteFunctionExternal (function, target, sender, returningPostback);
  }

  /// <summary> Obsolete. Executes the <paramref name="function"/> in another window or frame. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
  [Obsolete ("Use ExecuteFunctionExternal (WxeFunction, string, string Control, bool) instead.")]
  public void ExecuteFunction (
      WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    ExecuteFunctionExternal (function, target, features, sender, returningPostback);
  }

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
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunction/param[@name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  public void ExecuteFunction (
      WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString)
  {
    _wxeInfo.ExecuteFunction (function, createPermaUrl, useParentPermaUrl, permaUrlQueryString);
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
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender, createPermaUrl, useParentPermaUrl, permaUrlQueryString);
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
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString)
  {
    _wxeInfo.ExecuteFunctionNoRepost (
        function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, permaUrlQueryString);
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
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString)
  {
    _wxeInfo.ExecuteFunctionExternal (
        function, target, sender, returningPostback, createPermaUrl, useParentPermaUrl, permaUrlQueryString);
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
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlQueryString"]' />
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlQueryString)
  {
    _wxeInfo.ExecuteFunctionExternal (
        function, target, features, sender, returningPostback,  createPermaUrl, useParentPermaUrl, permaUrlQueryString);
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

  /// <summary> Registers a Java Script function to be executed when the page is aborted. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/RegisterClientSidePageEventHandler/*' />
  public void RegisterClientSidePageEventHandler (WxePageEvents pageEvent, string key, string function)
  {
    _wxeInfo.RegisterClientSidePageEventHandler (pageEvent, key, function);
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
  private ValidatableControlInitializer _validatableControlInitializer;
  private PostLoadInvoker _postLoadInvoker;
  private bool disposed;
  private NaBooleanEnum _enableAbortConfirmation = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableAbort = NaBooleanEnum.Undefined;
  private string _abortMessage;
  private NaBooleanEnum _enableStatusMessages = NaBooleanEnum.Undefined;
  private string _statusIsSubmittingMessage = string.Empty;
  private string _statusIsAbortingMessage = string.Empty;
  private string _statusIsCachedMessage = string.Empty;
  private NaBooleanEnum _enableSmartScrolling = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableSmartFocusing = NaBooleanEnum.Undefined;

  // protected HtmlForm Form; - won't work in VS 2005

  public WxePage ()
  {
    _wxeInfo = new WxePageInfo (this);
    _validatableControlInitializer = new ValidatableControlInitializer (this);
    _postLoadInvoker = new PostLoadInvoker (this);
    disposed = false;
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = _wxeInfo.EnsurePostBackModeDetermined (Context);
    _wxeInfo.Initialize (Context);
    OnPreInit();
    OnBeforeInit();
    return result;
  }

  /// <summary> Called before the initialization phase of the page. </summary>
  /// <remarks> 
  ///   In ASP.NET 1.1 this method is called by <b>DeterminePostBackMode</b>. Therefor you should not use
  ///   the postback collection during pre init.
  /// </remarks>
  protected virtual void OnPreInit ()
  {
  }

  [Obsolete ("Use OnPreInit instead.") ]
  protected virtual void OnBeforeInit()
  {
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

  /// <summary> Makes sure that PostLoad is called on all controls that support <see cref="ISupportsPostLoadControl"/>. </summary>
  public void EnsurePostLoadInvoked ()
  {
    _postLoadInvoker.EnsurePostLoadInvoked();
  }

  /// <summary> Makes sure that all validators are registered with their <see cref="IValidatableControl"/> controls. </summary>
  public void EnsureValidatableControlsInitialized ()
  {
    _validatableControlInitializer.EnsureValidatableControlsInitialized ();
  }

  /// <summary>
  ///   Call this method before validating when using <see cref="Rubicon.Web.UI.Controls.FormGridManager"/> 
  ///   and <see cref="M:Rubicon.ObjectBinding.Web.Controls.IBusinessObjectDataSourceControl.Validate()"/>.
  /// </summary>
  public void PrepareValidation()
  {
    EnsurePostLoadInvoked();
    EnsureValidatableControlsInitialized();
  }

  /// <summary> Gets the post back data for the page. </summary>
  public NameValueCollection GetPostBackCollection ()
  {
    return _wxeInfo.EnsurePostBackModeDetermined (Context);
  }

  protected override void OnPreRender(EventArgs e)
  {
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

  /// <summary> Gets or sets the <b>HtmlForm</b> of this page. </summary>
  /// <remarks> Redirects the call to the <see cref="HtmlForm"/> property. </remarks>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm IWxePage.HtmlForm
  {
    get { return HtmlForm; }
    set { HtmlForm = value; }
  }

  /// <summary> Gets or sets the <b>HtmlForm</b> of this page. </summary>
  /// <remarks>
  ///   <note type="inheritinfo"> 
  ///     Override this property you do not wish to rely on automatic detection of the <see cref="HtmlForm"/>
  ///     using reflection.
  ///   </note>
  /// </remarks>
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected virtual HtmlForm HtmlForm
  {
    get { return _wxeInfo.HtmlForm; }
    set { _wxeInfo.HtmlForm = value; }
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


  /// <summary> 
  ///   Gets or sets the flag that determines whether to display a confirmation dialog before aborting the session. 
  /// </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to display a confirmation dialog. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.False"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsAbortConfirmationEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a confirmation dialog before aborting the session. Undefined is interpreted as false.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableAbortConfirmation
  {
    get { return _enableAbortConfirmation; }
    set { _enableAbortConfirmation = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableAbortConfirmation"/> property. </summary>
  /// <value> <see langowrd="true"/> if <see cref="EnableAbortConfirmation"/> is <see cref="NaBooleanEnum.True"/>. </value>
  protected virtual bool IsAbortConfirmationEnabled
  {
    get { return _enableAbortConfirmation == NaBooleanEnum.True; }
  }

  /// <summary> Implementation of <see cref="IWxePage.IsAbortConfirmationEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsAbortConfirmationEnabled"/>. </value>
  bool IWxePage.IsAbortConfirmationEnabled
  {
    get { return IsAbortConfirmationEnabled; }
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

  /// <summary> Gets or sets the message displayed when the user attempts to abort the WXE Function. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to abort the WXE Function.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public string AbortMessage 
  {
    get { return _abortMessage; }
    set { _abortMessage = StringUtility.NullToEmpty (value); }
  }

  /// <summary> 
  ///   Gets or sets the flag that determines whether to display a message when the user tries to start a second
  ///   request.
  /// </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to enable the status messages. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="AreStatusMessagesEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a status message when the user attempts to start a second request. Undefined is interpreted as true.")]
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
  ///   Gets or sets the message displayed when the user attempts to submit while the page is already submitting. 
  /// </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to submit while the page is already submitting.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string StatusIsSubmittingMessage
  {
    get { return _statusIsSubmittingMessage; }
    set { _statusIsSubmittingMessage = StringUtility.NullToEmpty (value); }
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

  /// <summary> Gets or sets the flag that determines whether to use smart scrolling. </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to use smart scrolling. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartScrollingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart scrolling. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableSmartScrolling
  {
    get { return _enableSmartScrolling; }
    set { _enableSmartScrolling = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartScrolling"/> property. </summary>
  /// <value> 
  ///   <see langowrd="false"/> if <see cref="EnableSmartScrolling"/> is <see cref="NaBooleanEnum.False"/>
  ///   or the <see cref="SmartNavigationConfiguration.EnableScrolling"/> configuration setting is 
  ///   <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartScrollingEnabled
  {
    get
    {
      if (! WebConfiguration.Current.SmartNavigation.EnableScrolling)
        return false;
      return _enableSmartScrolling != NaBooleanEnum.False; 
    }
  }

  /// <summary> Implementation of <see cref="ISmartNavigablePage.IsSmartScrollingEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsSmartScrollingEnabled"/>. </value>
  bool ISmartNavigablePage.IsSmartScrollingEnabled
  {
    get { return IsSmartScrollingEnabled; }
  }

  /// <summary> Gets or sets the flag that determines whether to use smart navigation. </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to use smart navigation. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartFocusingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart navigation. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableSmartFocusing
  {
    get { return _enableSmartFocusing; }
    set { _enableSmartFocusing = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartFocusing"/> property. </summary>
  /// <value> 
  ///   <see langowrd="false"/> if <see cref="EnableSmartFocusing"/> is <see cref="NaBooleanEnum.False"/>
  ///   or the <see cref="SmartNavigationConfiguration.EnableFocusing"/> configuration setting is 
  ///   <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartFocusingEnabled
  {
    get
    {
      if (! WebConfiguration.Current.SmartNavigation.EnableFocusing)
        return false;
      return _enableSmartFocusing != NaBooleanEnum.False; 
    }
  }

  /// <summary> Implementation of <see cref="ISmartNavigablePage.IsSmartFocusingEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsSmartFocusingEnabled"/>. </value>
  bool ISmartNavigablePage.IsSmartFocusingEnabled
  {
    get { return IsSmartFocusingEnabled; }
  }

  /// <summary> Clears scrolling and focus information on the page. </summary>
  public void DiscardSmartNavigationData ()
  {
    _wxeInfo.DiscardSmartNavigationData();
  }

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> 
  ///   The <see cref="IFocusableControl"/> to assign the focus to. Must no be <see langword="null"/>.
  /// </param>
  public void SetFocus (IFocusableControl control)
  {
    _wxeInfo.SetFocus (control);
  }

  /// <summary> Sets the focus to the passed control ID. </summary>
  /// <param name="id"> 
  ///   The client side ID of the control to assign the focus to. Must no be <see langword="null"/> or empty. 
  /// </param>
  public void SetFocus (string id)
  {
    _wxeInfo.SetFocus (id);
  }
}

}
