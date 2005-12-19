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

namespace Rubicon.Web.UI
{

/// <summary> Specifies the client side events supported for registration by the <see cref="ISmartPage"/>. </summary>
public enum SmartPageEvents
{
  /// <summary> Rasied when the document has finished loading. Signature: <c>void Function (hasSubmitted, isCached)</c> </summary>
  OnLoad,
  /// <summary> Raised when the user posts back to the server. Signature: <c>void Function (eventTargetID, eventArgs)</c> </summary>
  OnPostBack,
  /// <summary> Raised when the user leaves the page. Signature: <c>void Function (hasSubmitted, isCached)</c> </summary>
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

public interface ISmartPage: IPage
{
  /// <summary> Gets the post back data for the page. </summary>
  NameValueCollection GetPostBackCollection ();

  /// <summary>
  ///   Gets or sets a flag that determines whether to display a confirmation dialog before aborting the session. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to display the confirmation dialog. </value>
  bool IsAbortConfirmationEnabled { get; }

  /// <summary> Gets the message displayed when the user attempts to abort the WXE Function. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  string AbortMessage { get; }

  /// <summary> Gets the message displayed when the user attempts to submit while the page is already submitting. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  string StatusIsSubmittingMessage { get; }

  /// <summary> 
  ///   Gets a flag whether the is submitting status messages will be displayed when the user tries to postback while 
  ///   a request is being processed.
  /// </summary>
  bool IsStatusIsSubmittingMessageEnabled { get; }

  /// <summary> 
  ///   Registers Java Script functions to be executed when the respective <paramref name="pageEvent"/> is raised.
  /// </summary>
  /// <include file='doc\include\UI\ISmartPage.xml' path='ISmartPage/RegisterClientSidePageEventHandler/*' />
  void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function);

  /// <summary> Gets or sets the <see cref="HtmlForm"/> of the ASP.NET page. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }
}

}
