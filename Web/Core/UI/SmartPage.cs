using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Rubicon.Collections;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI
{

public interface IModifiableControl: IControl
{
  /// <summary>
  ///   Specifies whether the value of the control has been changed on the Client since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> during loading
  ///   and saving values. Resetting <c>IsDirty</c> during saving is not implemented by all controls.
  /// </remarks>
  // TODO: redesign IsDirty semantics!
  bool IsDirty { get; set; }
  string[] GetTrackedClientIDs();
}

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

/// <summary>
///   This interface represents a page that has a dirty-state and can prevent multiple postbacks.
/// </summary>
/// <include file='doc\include\UI\ISmartPage.xml' path='ISmartPage/Class/*' />
public interface ISmartPage: IPage
{
  /// <summary> Gets the post back data for the page. </summary>
  NameValueCollection GetPostBackCollection ();

  void RegisterForDirtyStateTracking (IModifiableControl control);

  bool IsDirty { get; }
  bool IsDirtyStateTrackingEnabled { get; }
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
  /// <summary>
  ///   Regisiters a Java Script function used to evaluate whether to continue with the submit.
  ///   Signature: <c>bool Function (isAborting, hasSubmitted, hasUnloaded, isCached)</c>
  /// </summary>
  string CheckFormStateFunction { get; set; }

  /// <summary> Gets or sets the <see cref="HtmlForm"/> of the ASP.NET page. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }
}

/// <summary>
///   <b>SmartPage</b> is the default implementation of the <see cref="ISmartPage"/> interface. Use this type
///   a base class for pages that should supress multiple postbacks, require smart navigation, or have a dirty-state.
/// </summary>
/// <include file='doc\include\UI\SmartPage.xml' path='SmartPage/Class/*' />
public class SmartPage: Page, ISmartPage, ISmartNavigablePage
{
  #region ISmartPage Implementation

  /// <summary> 
  ///   Registers Java Script functions to be executed when the respective <paramref name="pageEvent"/> is raised.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/RegisterClientSidePageEventHandler/*' />
  public void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function)
  {
    _smartPageInfo.RegisterClientSidePageEventHandler (pageEvent, key, function);
  }


  /// <summary> Implementation of <see cref="ISmartPage.CheckFormStateFunction"/>. </summary>
  string ISmartPage.CheckFormStateFunction
  {
    get { return _smartPageInfo.CheckFormStateFunction; }
    set { _smartPageInfo.CheckFormStateFunction = value; }
  }

  /// <summary> Gets or sets the message displayed when the user attempts to leave the page. </summary>
  /// <remarks> 
  ///   In case of <see cref="String.Empty"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to leave the page.")]
  [Category ("Appearance")]
  [DefaultValue ("")]
  public virtual string AbortMessage 
  {
    get { return _smartPageInfo.AbortMessage; }
    set { _smartPageInfo.AbortMessage = value; }
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
    get { return _smartPageInfo.StatusIsSubmittingMessage; }
    set { _smartPageInfo.StatusIsSubmittingMessage = value; }
  }

  public void RegisterForDirtyStateTracking (IModifiableControl control)
  {
    _smartPageInfo.RegisterForDirtyStateTracking (control);
  }

  #endregion

  #region ISmartNavigablePage Implementation

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

  #endregion

  private SmartPageInfo _smartPageInfo;
  private ValidatableControlInitializer _validatableControlInitializer;
  private PostLoadInvoker _postLoadInvoker;
  private bool _isDirty = false;
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
    if (string.Compare (Request.HttpMethod, "POST", true) == 0)
      return Request.Form;
    else
      return Request.QueryString;
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



  /// <summary> Gets or sets a flag describing whether the page is dirty. </summary>
  /// <value> <see langowrd="true"/> if the page requires saving. Defaults to <see langword="false"/>.  </value>
  protected virtual bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary> Implementation of <see cref="ISmartPage.IsDirty"/>. </summary>
  /// <value> The value returned by <see cref="IsDirty"/>. </value>
  bool ISmartPage.IsDirty
  {
    get { return IsDirty; }
  }

  /// <summary> Gets the evaluated value for the <see cref="IsDirtyStateTrackingEnabled"/> property. </summary>
  /// <value> <see langowrd="true"/> if <see cref="IsAbortConfirmationEnabled"/> is <see langowrd="true"/>. </value>
  protected virtual bool IsDirtyStateTrackingEnabled
  {
    get { return IsAbortConfirmationEnabled; }
  }

  /// <summary> Implementation of <see cref="ISmartPage.IsDirtyStateTrackingEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsDirtyStateTrackingEnabled"/>. </value>
  bool ISmartPage.IsDirtyStateTrackingEnabled
  {
    get { return IsDirtyStateTrackingEnabled; }
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
  /// <value> The value returned by <see cref="IsStatusIsSubmittingMessageEnabled"/>. </value>
  bool ISmartPage.IsStatusIsSubmittingMessageEnabled
  {
    get { return IsStatusIsSubmittingMessageEnabled; }
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


  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    _isDirty = (bool)  values[1];
  }

  protected override object SaveViewState()
  {
    object[] values = new object[2];
    values[0] = base.SaveViewState();
    values[1] = _isDirty;
    return values;
  }
}

}
