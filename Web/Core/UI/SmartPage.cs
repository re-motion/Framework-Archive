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
  string CheckFormStateMethod { get; set; }

  /// <summary> Gets or sets the <see cref="HtmlForm"/> of the ASP.NET page. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }
}

public class SmartPage: Page, ISmartPage, ISmartNavigablePage
{
  private SmartPageInfo _smartPageInfo;
  private ValidatableControlInitializer _validatableControlInitializer;
  private PostLoadInvoker _postLoadInvoker;
  private string _abortMessage;
  private string _statusIsSubmittingMessage = string.Empty;
  private NaBooleanEnum _enableAbortConfirmation = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableStatusIsSubmittingMessage;
  private NaBooleanEnum _enableSmartScrolling = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableSmartFocusing = NaBooleanEnum.Undefined;


  public SmartPage()
  {
    _smartPageInfo = new SmartPageInfo (this);
    _validatableControlInitializer = new ValidatableControlInitializer (this);
    _postLoadInvoker = new PostLoadInvoker (this);
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = base.DeterminePostBackMode();

#if NET11
    OnPreInit();
#endif

    return result;
  }

#if NET11
  /// <summary> Called before the initialization phase of the page. </summary>
  /// <remarks> 
  ///   In ASP.NET 1.1 this method is called by <b>DeterminePostBackMode</b>. Therefor you should not use
  ///   the postback collection during pre init.
  /// </remarks>
  protected virtual void OnPreInit ()
  {
  }
#endif

  /// <summary> Gets the post back data for the page. </summary>
  NameValueCollection ISmartPage.GetPostBackCollection ()
  {
    if (string.Compare (Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture) == 0)
      return Request.Form;
    else
      return Request.QueryString;
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
    _smartPageInfo.PreRender();
  }


  /// <summary> Gets or sets the <b>HtmlForm</b> of this page. </summary>
  /// <remarks> Redirects the call to the <see cref="HtmlForm"/> property. </remarks>
  HtmlForm ISmartPage.HtmlForm
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
    get { return _smartPageInfo.HtmlForm; }
    set { _smartPageInfo.HtmlForm = value; }
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
  ///   Registers Java Script functions to be executed when the respective <paramref name="pageEvent"/> is raised.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/RegisterClientSidePageEventHandler/*' />
  public void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function)
  {
    _smartPageInfo.RegisterClientSidePageEventHandler (pageEvent, key, function);
  }

  protected string CheckFormStateMethod
  {
    get { return _smartPageInfo.CheckFormStateMethod; }
    set { _smartPageInfo.CheckFormStateMethod = value; }
  }

  string ISmartPage.CheckFormStateMethod
  {
    get { return _smartPageInfo.CheckFormStateMethod; }
    set { _smartPageInfo.CheckFormStateMethod = value; }
  }


  /// <summary> Gets or sets the message displayed when the user attempts to leave the page. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to leave the page.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public string AbortMessage 
  {
    get { return _abortMessage; }
    set { _abortMessage = StringUtility.NullToEmpty (value); }
  }

  /// <summary> 
  ///   Gets or sets the flag that determines whether to display a confirmation dialog before leaving the page. 
  /// </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to display a confirmation dialog. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.False"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsAbortConfirmationEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a confirmation dialog before leaving the page. "
             + "Undefined is interpreted as false.")]
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

  /// <summary> Implementation of <see cref="ISmartPage.IsAbortConfirmationEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsAbortConfirmationEnabled"/>. </value>
  bool ISmartPage.IsAbortConfirmationEnabled
  {
    get { return IsAbortConfirmationEnabled; }
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
  ///   Use <see cref="IsStatusIsSubmittingMessageEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a status message when the user attempts to start a "
             + "second request. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableStatusIsSubmittingMessage
  {
    get { return _enableStatusIsSubmittingMessage; }
    set { _enableStatusIsSubmittingMessage = value; }
  }

  /// <summary> 
  ///   Gets a flag whether a status message  will be displayed when the user tries to postback while a request is 
  ///   being processed.
  /// </summary>
  protected virtual bool IsStatusIsSubmittingMessageEnabled
  {
    get { return _enableStatusIsSubmittingMessage != NaBooleanEnum.False; }
  }

  /// <summary> Implementation of <see cref="ISmartPage.IsStatusIsSubmittingMessageEnabled"/>. </summary>
  /// <value> The value returned by <see cref="AreStatusMessagesEnabled"/>. </value>
  bool ISmartPage.IsStatusIsSubmittingMessageEnabled
  {
    get { return IsStatusIsSubmittingMessageEnabled; }
  }

  /// <summary> 
  ///   Gets or sets the message displayed when the user attempts to submit while the page is already submitting. 
  /// </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to submit while the page is already submitting.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string StatusIsSubmittingMessage
  {
    get { return _statusIsSubmittingMessage; }
    set { _statusIsSubmittingMessage = StringUtility.NullToEmpty (value); }
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
    _smartPageInfo.DiscardSmartNavigationData();
  }

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> 
  ///   The <see cref="IFocusableControl"/> to assign the focus to. Must no be <see langword="null"/>.
  /// </param>
  public void SetFocus (IFocusableControl control)
  {
    _smartPageInfo.SetFocus (control);
  }

  /// <summary> Sets the focus to the passed control ID. </summary>
  /// <param name="id"> 
  ///   The client side ID of the control to assign the focus to. Must no be <see langword="null"/> or empty. 
  /// </param>
  public void SetFocus (string id)
  {
    _smartPageInfo.SetFocus (id);
  }
}

}
