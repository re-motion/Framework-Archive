using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Collections;

using Rubicon.Web.UI.Utilities;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
/// Provides common features for page navigation and session state handling.
/// </summary>
public class NavigablePage : MultiLingualPage, INavigablePage, IPostBackEventHandler
{
  // types

  protected enum ShowBackLinkType 
  {
    Always,
    Never,
    Default
  }

  // static members and constants

  private const string c_eventNameShowMessageBoxResult = "ShowMessageBoxResult:";
  private const string c_eventNameNavigationRequest = "NavigablePageNavigationRequest";
  private const string c_viewStateNavigationUrl = "NavigationRequest:NavigateToUrl";

  /// <summary>
  /// returns a formatted GUID string
  /// </summary>
  /// <returns></returns>
  public static string GetUniqueToken ()
  {
    return PageUtility.GetUniqueToken ();
  }

  /// <summary>
  /// Returns the correct page's URL even if cookieless mode is activated.
  /// </summary>
  protected static string GetPhysicalPageUrl (Page page)
  {
    return PageUtility.GetPhysicalPageUrl (page);
  }

  protected static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      bool returnToThisPage, 
      PageUtility.ShowNavigationBar showNavBar)
  {
    CallPage (sourcePage, destinationUrl, null, returnToThisPage, showNavBar);
  }

  /// <summary>
  /// redirects to the page identified by the given URL 
  /// </summary>
  /// <param name="destinationUrl">URL redirecting to</param>
  /// <param name="parameters">parameters for the page redirected to</param>
  protected static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      IDictionary parameters, 
      bool returnToThisPage,
      PageUtility.ShowNavigationBar showNavBar)
  {
    PageUtility.CallPage (sourcePage, destinationUrl, parameters, returnToThisPage, showNavBar);
  }

  /// <summary>
  /// redirects to the page identified by the given URL 
  /// </summary>
  /// <param name="destinationUrl">URL redirecting to</param>
  /// <param name="parameters">parameters for the page redirected to</param>
  protected static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      IDictionary parameters, 
      bool returnToThisPage,
      PageUtility.ShowNavigationBar showNavBar,
      string referrerUrl)
  {
    PageUtility.CallPage (sourcePage, destinationUrl, parameters, returnToThisPage, showNavBar, referrerUrl);
  }


  // member fields


  // construction and disposing

	protected override void OnInit(EventArgs e)
	{
    base.OnInit(e);

    if (! IsPostBack)
    {
      string cleanupToken = this.Request.QueryString["cleanupToken"];
      CleanupSession (cleanupToken);
      PageUtility.DeleteOutdatedSessions( this );
    }
	}

  public NavigablePage()
  {
    this.ID = "PageIDNotSet";
  }


  // methods and properties

  protected object GetSessionValue (string key)
  {
    return GetSessionValue (key, true);
  }
  
  protected object GetSessionValue (string key, bool isRequired)
  {
    return GetSessionValue (this.Token, key, isRequired);
  }

  protected void SetSessionValue (string key, object sessionValue)
  {
    SetSessionValue (this.Token, key, sessionValue);
  }

  protected void ClearSessionValue (string key)
  { 
    ClearSessionValue (this.Token, key);
  }
  
  public object GetGlobalSessionValue (string key, bool isRequired)
  {
    object sessionValue = Page.Session[key];

    if (isRequired && sessionValue == null)
    {
      if (Session.IsNewSession)
        throw new SessionTimeoutException ();
      else
        throw new SessionVariableNotFoundException (key);
    }

    return sessionValue;
  }

  public void SetGlobalSessionValue (string key, object sessionValue)
  {
    Page.Session[key] = sessionValue;
  }

  public void ClearGlobalSessionValue (string key)
  { 
    PageUtility.ClearSessionValue (this, key);
  }
  
  protected object GetSessionValue (string token, string key, bool required)
  {
    if (token == null) token = string.Empty;
    return PageUtility.GetSessionValue (this, token, key, required);
  }

  protected void SetSessionValue (string token, string key, object sessionValue)
  {
    if (token == null) token = string.Empty;
    PageUtility.SetSessionValue (this, token, key, sessionValue);
  }

  protected void ClearSessionValue (string token, string key)
  { 
    PageUtility.ClearSessionValue (this, token, key);
  }

  protected void CleanupSession ()
  {
    CleanupSession (this.Token);
  }
  
  protected void CleanupSession (string token)
  {
    PageUtility.ClearSession (this, token);
  }


  public string Token
  {
    get {return PageUtility.GetToken (this);}
  }
  
  public string GetParentToken()
  {
    return PageUtility.GetParentToken (this);
  }

  /// <summary>
  /// Returns an IDictionary with the parameters the page was called with
  /// and copies the parameters to the new token
  /// </summary>
  /// <param name="requireParameters">If false, no exception is thrown if parameters are not found.</param>
  /// <exception cref="NoPageParametersException">Thrown when a page is called without parameters.</exception>
  /// <exception cref="SessionTimeout">Thrown when the session is timed out.</exception>
  /// <returns>IDictionary containing parameters</returns>
  public IDictionary GetCallParameters (bool requireParameters)
  {
    return PageUtility.GetCallParameters (this, requireParameters);    
  }
  
  public IDictionary GetCallParameters ()
  {
    return GetCallParameters (true);
  }

  public virtual bool AllowImmediateClose
  {
    get { return true; }
  }

  public virtual bool CleanupOnImmediateClose 
  { 
    get { return true; }
  }
  
  public virtual bool NavigationRequest (string url)
  {
    return true;
  }

  public virtual bool AutoDeleteSessionVariables 
  { 
    get { return true; }
  }

  public virtual void NavigateTo (string url, bool returnToThisPage)
  {
    // remove session variables
    if (! returnToThisPage)
    {
      string pageToken = this.Request.QueryString["pageToken"];
      CleanupSession (pageToken);
    }

    // copy certain parameters to new url
    string[] parameters = {"navSelectedTab", "navSelectedMenu"};
    foreach (string parameter in parameters)
    {
      bool urlHasParameter =     url.IndexOf ("?" + parameter + "=") != -1
                              || url.IndexOf ("&" + parameter + "=") != -1;
      if (! urlHasParameter)
      {
        string parameterValue = this.Request.QueryString[parameter];
        if (parameterValue != null && parameterValue != string.Empty)
          url = PageUtility.AddUrlParameter (url, parameter, parameterValue);
      }
    }

    PageUtility.Redirect (this.Response, url);
  }


  public void RegisterMessageBox (string message, string eventName)
  {
    if (this.ID == null || this.ID == string.Empty)
      throw new InvalidOperationException ("Page must have ID in order to use RegisterMessageBox.");

    string script = string.Format (
        "<script language=\"javascript\"> \n"
            + "if (confirm (\"{0}\")) \n"
            + "  " + this.GetPostBackClientEvent (this, c_eventNameShowMessageBoxResult + eventName + ":OK") + "\n"
            + "else \n"
            + "  " + this.GetPostBackClientEvent (this, c_eventNameShowMessageBoxResult + eventName + ":Cancel") + "\n" 
            + "</script>",
        message);

    this.RegisterStartupScript ("NavigablePage_ShowMessageBoxScript", script);
  }

  public void InitiateConditionalNavigation (string url, string message)
  {
    ViewState[c_viewStateNavigationUrl] = url;
    RegisterMessageBox (message, c_eventNameNavigationRequest);
  }

  public virtual void RaisePostBackEvent (string eventArgument)
  {
    if (eventArgument.StartsWith (c_eventNameShowMessageBoxResult))
    {
      // eventArgument example: "ShowMessageBoxResult:EventName:OK"
      string eventArgPart = eventArgument.Substring (c_eventNameShowMessageBoxResult.Length);
      // eventArgPart example: "EventName:OK"
      int colonPos = eventArgPart.IndexOf (":");
      string eventName = eventArgPart.Substring (0, colonPos);
      string resultString = eventArgPart.Substring (colonPos + 1);
      bool result = resultString == "OK";
      HandleMessageBoxResult (eventName, result);
    }
    else if (eventArgument == "NavigablePageBackLink")
    {
      CallBackLink();
    }
  }

  protected virtual void HandleMessageBoxResult (string eventName, bool result)
  {
    if (eventName == c_eventNameNavigationRequest)
    {
      if (result)
      {
        string url = (string) ViewState[c_viewStateNavigationUrl];
        NavigateTo (url, false);
      }
    }
  }

  /// <summary>
  /// converts a given key value to a page specific value
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  private string GetUniqueKey (string key)
  {
    return PageUtility.GetUniqueKey (this, key);
  }

  protected virtual ShowBackLinkType GetShowBackLinkType ()
  {
    return ShowBackLinkType.Default;
  }

  protected virtual bool IsServerSideBackLink ()
  {
    return false;
  }

  public bool ShowBackLink ()
  {
    ShowBackLinkType showBackLink = GetShowBackLinkType ();
    if (showBackLink == ShowBackLinkType.Always)
      return true;
    else if (showBackLink == ShowBackLinkType.Never)
      return false;

    IDictionary parameters = GetCallParameters (false);
    if (parameters == null)
      return false;
    return parameters ["Referrer"] != null;
  }

  public virtual string GetBackLinkUrl ()
  {
    if (IsServerSideBackLink())
      return "javascript:" + GetPostBackClientEvent (this, "NavigablePageBackLink");

    IDictionary parameters = GetCallParameters (false);
    if (parameters != null)
      return (string) parameters ["Referrer"];

    string backLinkUrl = (string) GetSessionValue ( "referrerUrl", false);
    if (backLinkUrl == null)
    {
      if (!IsPostBack)
      {
        backLinkUrl = Request.UrlReferrer.AbsoluteUri;
        SetSessionValue (this.Token, "referrerUrl", backLinkUrl);
      }
      else
      {
        throw new SessionVariableNotFoundException ("referrerUrl");
      }
    }
    return backLinkUrl;
  }

  protected virtual void CallBackLink ()
  {
  }
}
}
